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

	public class Core : IMoistureBot
	{

		private static readonly log4net.ILog log = log4net.LogManager.GetLogger
			(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	
		private volatile bool terminated;

		private SteamClient steamClient;
		private CallbackManager manager;

		private SteamUser steamUser;
		private SteamFriends steamFriends;

		private bool isRunning;

		// Bot properties
		private string user;

		public string User { get { return user; } }

		private string pass;
		private List<ulong> activeChatRooms = new List<ulong> ();

		public Core ()
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
			new Callback<SteamFriends.ChatActionResultCallback> (ChatActionResultCallback, manager);

			new Callback<SteamFriends.FriendMsgCallback> ( FriendMsgCallback, manager);

		}

		void ChatActionResultCallback (SteamFriends.ChatActionResultCallback obj)
		{
			switch (obj.Action) {
			case EChatAction.Ban:
				// TODO: extension point
				log.Debug("User " + obj.ChatterID.ConvertToUInt64() + " (" + GetUserName(obj.ChatterID) +") banned from " + obj.ChatRoomID.ConvertToUInt64() );
				break;
			case EChatAction.Kick:
				// TODO: extension point
				log.Debug("User " + obj.ChatterID.ConvertToUInt64() + " (" + GetUserName(obj.ChatterID) +") kicked from " + obj.ChatRoomID.ConvertToUInt64() );
				break;
			case EChatAction.CloseChat:
				// TODO: extension point
				log.Debug("Chat room " + obj.ChatRoomID.ConvertToUInt64() + " closed.");
				activeChatRooms.Remove( obj.ChatRoomID.ConvertToUInt64() );
				break;
			case EChatAction.InviteChat:
				// TODO: extension point
				log.Debug("User " + obj.ChatterID.ConvertToUInt64() + " (" + GetUserName(obj.ChatterID) +") invited to " + obj.ChatRoomID.ConvertToUInt64() );
				break;
			case EChatAction.UnBan:
				// TODO: extension point
				log.Debug("User " + obj.ChatterID.ConvertToUInt64() + " (" + GetUserName(obj.ChatterID) +") unbanned from " + obj.ChatRoomID.ConvertToUInt64() );
				break;
			} 
		}

		private string GetUserName(SteamID id) {
			return steamFriends.GetFriendPersonaName(id);
		}

		public void KickChatMember (ulong roomId, ulong userId) {
			log.Debug ("Kicking user " + userId + " from room " + roomId);
			steamFriends.KickChatMember (new SteamID (roomId), new SteamID(userId));
		}

		public void Connect(string username, string password)
		{
			log.Debug ("Connecting user " + username + " to steam");
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
			while ( !terminated )
			{
				try {
					manager.RunWaitCallbacks();
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
			log.Debug ("Disconnecting client...");
			steamClient.Disconnect();
		}


		private void ConnectedCallback( SteamClient.ConnectedCallback callback )
		{
			if ( callback.Result != EResult.OK )
			{
				log.Debug( "Unable to connect to Steam: " + callback.Result.ToString() );
				return;
			}

			log.Debug ("Connected to Steam!");
			log.Debug( "Logging in '" + user + "'..." );

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
			log.Debug( "Disconnected from Steam" );

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

					log.Debug( "Unable to logon to Steam: This account is SteamGuard protected." );
					return;
				}
					
				log.Debug( "Unable to logon to Steam: " + callback.ExtendedResult );
				return;
			}
		
			log.Debug( "Successfully logged on!" );

		}

		private void ChatEnterCallback( SteamFriends.ChatEnterCallback callback )
		{
			switch (callback.EnterResponse) {
			case EChatRoomEnterResponse.Success:
				log.Debug ("Successfully joined chat!");
				activeChatRooms.Add (callback.ChatID.ConvertToUInt64 ());
				break;
			case EChatRoomEnterResponse.Limited:
				log.Debug ("Failed to join chat: " + callback.EnterResponse);
				log.Debug ("The user account needs to have one game in it's library in order to join chat rooms");
				break;
			default:
				log.Debug ("Failed to join chat: " + callback.EnterResponse);
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
						log.Error ("Error in addin", e);
					}
				}
			}

		}

		private void ChatInviteCallback( SteamFriends.ChatInviteCallback callback )
		{
			log.Debug ("Received chat invite to room " + 
				callback.ChatRoomID.ConvertToUInt64 () + 
				" from user " + callback.FriendChatID.ConvertToUInt64 () 
				+ " (" + GetUserName (callback.FriendChatID) + ")");

			// TODO create extension point
		}

		private void LoggedOffCallback( SteamUser.LoggedOffCallback callback )
		{
			log.Debug( "Logged off of Steam: " + callback.Result );
		}

		#region PUBLIC

		public void JoinChatRoom(ulong roomId) {
			steamFriends.JoinChat (new SteamID(roomId));
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

		public void Terminate ()
		{
			terminated = true;
		}

		public void BanChatMember (ulong roomId, ulong userId)
		{
			log.Debug ("Banning chat member " + userId + " in room " + roomId);
			steamFriends.BanChatMember (new SteamID (roomId), new SteamID (userId));
		}

		public void UnbanChatMember (ulong roomId, ulong userId)
		{
			log.Debug ("Unbanning chat member " + userId + " in room " + roomId);
			steamFriends.UnbanChatMember (new SteamID (roomId), new SteamID (userId));
		}

		#endregion

	}
}

