using System;
using System.Linq;
using System.Text;
using SteamKit2;
using System.Text.RegularExpressions;
using Mono.Addins;
using System.Threading;
using System.Collections.Generic;
using IniParser.Model;

namespace MoistureBot
{

	public class ChatBot : IMoistureBot
	{

		private static readonly log4net.ILog log = log4net.LogManager.GetLogger
			(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	
		private SteamClient steamClient;
		private CallbackManager manager;

		private SteamUser steamUser;
		private SteamFriends steamFriends;

		private static EventWaitHandle _connectWaitHandle = new AutoResetEvent (false);
		private static EventWaitHandle _disconnectWaitHandle = new AutoResetEvent (false);

		private bool isRunning;

		// Bot properties
		private string user;
		private string pass;
		private List<ulong> activeChatRooms = new List<ulong> ();

		public ChatBot ()
		{

			// create our steamclient instance
			steamClient = new SteamClient();
			// create the callback manager which will route callbacks to function calls
			manager = new CallbackManager( steamClient );

			// get the steamuser handler, which is used for logging on after successfully connecting
			steamUser = steamClient.GetHandler<SteamUser>();
			steamFriends = steamClient.GetHandler<SteamFriends>();

			// register a few callbacks we're interested in
			// these are registered upon creation to a callback manager, which will then route the callbacks
			// to the functions specified
			new Callback<SteamClient.ConnectedCallback>( ConnectedCallback, manager );
			new Callback<SteamClient.DisconnectedCallback>( DisconnectedCallback, manager );

			// User
			new Callback<SteamUser.LoggedOnCallback>( LoggedOnCallback, manager );
			new Callback<SteamUser.LoggedOffCallback>( LoggedOffCallback, manager );
			new Callback<SteamUser.AccountInfoCallback>( AccountInfoCallback, manager );

			// Friends
			new Callback<SteamFriends.ChatEnterCallback> ( ChatEnterCallback, manager);
			new Callback<SteamFriends.ChatMsgCallback> ( ChatMsgCallback, manager);
			new Callback<SteamFriends.ChatInviteCallback> ( ChatInviteCallback, manager);

			new Callback<SteamFriends.FriendMsgCallback> ( FriendMsgCallback, manager);

		}

		public void Connect(string username, string password)
		{
			log.Info( "Connecting to Steam..." );
			this.user = username;
			this.pass = password;
			steamClient.Connect();
			Start ();
		}

		private void Start ()
		{
			Thread t = new Thread( CallbackWaitThread );

			try
			{
				t.Start();
			}
			catch (ThreadStateException e)
			{
				log.Error ("Error in bot callback thread", e);
			}
			catch (ThreadInterruptedException e)
			{
				log.Error ("Bot thread interrupted", e);
			}
		}

		private void CallbackWaitThread()
		{
			isRunning = true;
			while ( isRunning )
			{
				try {
					manager.RunWaitCallbacks( TimeSpan.FromSeconds( 5 ) );
				} catch (InvalidOperationException e)  {
					// There is probably a bug in SteamKit2 causing this exception sometimes when connecting
					// TODO: might even want to disable logging on this one
					log.Error ("Error in bot callback thread", e);
				} catch (Exception e){
					log.Error ("Error in bot callback thread", e);
				}
			}
		}

		public void Disconnect()
		{
			steamClient.Disconnect();
		}


		private void ConnectedCallback( SteamClient.ConnectedCallback callback )
		{
			if ( callback.Result != EResult.OK )
			{
				Console.WriteLine( "Unable to connect to Steam: {0}", callback.Result );
				_connectWaitHandle.Set ();
				BlockUntilDisconnected ();
				return;
			}

			log.Info ("Connected to Steam!");
			log.Info( "Logging in '" + user + "'..." );

			steamUser.LogOn( new SteamUser.LogOnDetails
			{
					Username = user,
					Password = pass,
			} );
		}

		private void AccountInfoCallback( SteamUser.AccountInfoCallback callback )
		{
			// before being able to interact with friends, you must wait for the account info callback
			// this callback is posted shortly after a successful logon

			// at this point, we can go online on friends, so lets do that
			steamFriends.SetPersonaState( EPersonaState.Online );
		}

		private void DisconnectedCallback( SteamClient.DisconnectedCallback callback )
		{
			log.Info( "Disconnected from Steam" );
			_disconnectWaitHandle.Set ();
		}

		private void LoggedOnCallback( SteamUser.LoggedOnCallback callback )
		{

			if ( callback.Result != EResult.OK )
			{
				if ( callback.Result == EResult.AccountLogonDenied )
				{
					// if we recieve AccountLogonDenied or one of it's flavors (AccountLogonDeniedNoMailSent, etc)
					// then the account we're logging into is SteamGuard protected
					// see sample 6 for how SteamGuard can be handled

					log.Info( "Unable to logon to Steam: This account is SteamGuard protected." );
					_connectWaitHandle.Set ();
					BlockUntilDisconnected ();
					return;
				}
					
				log.Info( "Unable to logon to Steam: " + callback.ExtendedResult );
				_connectWaitHandle.Set ();
				BlockUntilDisconnected ();
				return;
			}
		
			log.Info( "Successfully logged on!" );
			_connectWaitHandle.Set ();

		}

		private void ChatEnterCallback( SteamFriends.ChatEnterCallback callback )
		{
			switch (callback.EnterResponse) {
			case EChatRoomEnterResponse.Success:
				log.Info ("Successfully joined chat!");
				activeChatRooms.Add (callback.ChatID.ConvertToUInt64 ());
				break;
			default:
				log.Info ("Failed to join chat: " + callback.EnterResponse);
				break;
			}
				
		}



		private void ChatMsgCallback( SteamFriends.ChatMsgCallback callback )
		{

			string message = callback.Message;
			ulong chatterId = callback.ChatterID.ConvertToUInt64();
			ulong chatId = callback.ChatRoomID.ConvertToUInt64();

			if (callback.ChatMsgType == EChatEntryType.ChatMsg) {
				foreach (IChatRoomAddin addin in AddinManager.GetExtensionObjects<IChatRoomAddin> ())
				{
					try {
						addin.MessageReceived(new ChatRoomMessage(message, chatterId, chatId));
					} catch(Exception e) {
						Console.WriteLine ();
						log.Error ("Error in addin", e);
					}

				}
			}

		}

		private void FriendMsgCallback( SteamFriends.FriendMsgCallback callback )
		{

			string message = callback.Message;
			ulong chatterId = callback.Sender.ConvertToUInt64 ();

			if (callback.EntryType == EChatEntryType.ChatMsg) {
				foreach (IChatFriendAddin addin in AddinManager.GetExtensionObjects<IChatFriendAddin> ())
				{
					try {
						addin.MessageReceived(new ChatMessage(message, chatterId));
					} catch(Exception e) {
						Console.WriteLine ();
						log.Error ("Error in addin", e);
					}
				}
			}

		}

		private void ChatInviteCallback( SteamFriends.ChatInviteCallback callback )
		{
			// TODO create extension point
		}

		private void LoggedOffCallback( SteamUser.LoggedOffCallback callback )
		{
			log.Info( "Logged off of Steam: " + callback.Result );
		}

		#region PUBLIC

		public void JoinChat(ulong id) {
			steamFriends.JoinChat (new SteamID(id));
		}

		public void SendChatRoomMessage(String message, ulong chatRoomId) {

			if (string.IsNullOrEmpty (message)) {
				log.Debug ("Unable to send empty message to chat room");
				return;
			}
				
			try {
				steamFriends.SendChatRoomMessage (
					new SteamID (chatRoomId), 
					EChatEntryType.ChatMsg,
					message
				);
			} catch (Exception e) {
				log.Error ("Failed to send message to chat room " + chatRoomId, e);
			}
		}

		public void SendChatMessage(String message, ulong userId) {

			if (string.IsNullOrEmpty (message)) {
				log.Debug ("Unable to send empty message to user");
				return;
			}
			try {
				steamFriends.SendChatMessage (
					new SteamID (userId), 
					EChatEntryType.ChatMsg,
					message
				);
			} catch (Exception e) {
				log.Error ("Failed to send message to user " + userId, e);
			}
		}

		public string GetUserName (ulong id)
		{
			return steamFriends.GetFriendPersonaName (new SteamID(id));
		}

		public Boolean IsConnected()
		{
			return steamClient.IsConnected;
		}

		public List<ulong> GetActiveChatRooms() {
			return activeChatRooms;
		}

		public void BlockUntilConnected() {
			log.Debug ("Blocking bot until connected");
			if (IsConnected())
				return;

			_connectWaitHandle.WaitOne();
			log.Debug ("Bot connected, releasing block");
		}

		public void BlockUntilDisconnected() {
			log.Debug ("Blocking bot until disconnected");
			if (!IsConnected())
				return;

			_disconnectWaitHandle.WaitOne();
			log.Debug ("Bot disconnected, releasing block");
		}

		#endregion

	}
}

