using System;
using System.Linq;
using System.Text;
using SteamKit2;
using System.Text.RegularExpressions;
using Mono.Addins;
using System.Threading;

namespace moisturebot
{

	public class MoistureBot : IMoistureBot
	{

		private SteamClient steamClient;
		private CallbackManager manager;

		private SteamUser steamUser;
		private SteamFriends steamFriends;

		private bool isRunning;

		// Bot properties
		private string user;
		private string pass;

		public MoistureBot ()
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

		}

		public void Connect(string username, string password)
		{
			Console.WriteLine( "Connecting to Steam..." );
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
				Console.WriteLine(e);  // Display text of exception
			}
			catch (ThreadInterruptedException e)
			{
				Console.WriteLine(e);  // This exception means that the thread
			}
		}

		private void CallbackWaitThread()
		{
			isRunning = true;
			while ( isRunning )
			{
				manager.RunWaitCallbacks( TimeSpan.FromSeconds( 1 ) );
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
				return;
			}

			Console.WriteLine ("Connected to Steam!");
			Console.WriteLine( "Logging in '{0}'...", user );

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
			Console.WriteLine( "Disconnected from Steam" );
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

					Console.WriteLine( "Unable to logon to Steam: This account is SteamGuard protected." );

					return;
				}
					
				Console.WriteLine( "Unable to logon to Steam: {0} / {1}", callback.Result, callback.ExtendedResult );
				return;
			}
		
			Console.WriteLine( "Successfully logged on!" );

		}

		private void ChatEnterCallback( SteamFriends.ChatEnterCallback callback )
		{
			switch (callback.EnterResponse) {
			case EChatRoomEnterResponse.Success:
				Console.WriteLine ("Successfully joined chat!");
				break;
			default:
				Console.WriteLine ("Failed to join chat: {0}", callback.EnterResponse);
				break;
			}
				
		}

		public void JoinChat(ulong id) {
			steamFriends.JoinChat (new SteamID(id));
		}

		public void SendChatMessage(String message, ulong chatId) {
			steamFriends.SendChatRoomMessage (
				new SteamID (chatId), 
				EChatEntryType.ChatMsg,
				message
			);
		}

		private void ChatMsgCallback( SteamFriends.ChatMsgCallback callback )
		{

			string message = callback.Message;
			string chatterName = steamFriends.GetFriendPersonaName(callback.ChatterID);
			ulong chatterId = callback.ChatterID.ConvertToUInt64();
			ulong chatId = callback.ChatRoomID.ConvertToUInt64();

			foreach (IChatAddin addin in AddinManager.GetExtensionObjects<IChatAddin> ())
			{
				addin.MessageReceived(new ChatMessage(message, chatterId, chatterName, chatId));
			}

		}

		private void ChatInviteCallback( SteamFriends.ChatInviteCallback callback )
		{
			// TODO create extension point
		}

		private void LoggedOffCallback( SteamUser.LoggedOffCallback callback )
		{
			Console.WriteLine( "Logged off of Steam: {0}", callback.Result );
		}

	}
}

