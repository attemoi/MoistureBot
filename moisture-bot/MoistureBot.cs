using System;
using System.Linq;
using System.Text;

using SteamKit2;
using System.Text.RegularExpressions;
using Mono.Addins;

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
		public string User { get; set; }
		public string Pass { get; set; }
		public ulong ChatId { get; set; }

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
			new Callback<SteamClient.ConnectedCallback>( OnConnected, manager );
			new Callback<SteamClient.DisconnectedCallback>( OnDisconnected, manager );

			// User
			new Callback<SteamUser.LoggedOnCallback>( OnLoggedOn, manager );
			new Callback<SteamUser.LoggedOffCallback>( OnLoggedOff, manager );
			new Callback<SteamUser.AccountInfoCallback>( OnAccountInfo, manager );

			// Friends
			new Callback<SteamFriends.ChatEnterCallback> ( OnChatEnter, manager);
			new Callback<SteamFriends.ChatMsgCallback> ( OnChatMsg, manager);

		}

		public void connect()
		{
			isRunning = true;

			Console.WriteLine( "Connecting to Steam..." );

			steamClient.Connect();

			while ( isRunning )
			{
				manager.RunWaitCallbacks( TimeSpan.FromSeconds( 3 ) );
			}

			string line;
			while ((line = Console.ReadLine()) != null)
				Console.WriteLine(line);

		}

		public void disconnect()
		{
			isRunning = false;
			steamClient.Disconnect();
		}


		private void OnConnected( SteamClient.ConnectedCallback callback )
		{
			if ( callback.Result != EResult.OK )
			{
				Console.WriteLine( "Unable to connect to Steam: {0}", callback.Result );

				isRunning = false;
				return;
			}

			Console.WriteLine( "Connected to Steam! Logging in '{0}'...", User );

			steamUser.LogOn( new SteamUser.LogOnDetails
				{
					Username = User,
					Password = Pass,
				} );
		}

		private void OnAccountInfo( SteamUser.AccountInfoCallback callback )
		{
			// before being able to interact with friends, you must wait for the account info callback
			// this callback is posted shortly after a successful logon

			// at this point, we can go online on friends, so lets do that
			steamFriends.SetPersonaState( EPersonaState.Online );
		}

		private void OnDisconnected( SteamClient.DisconnectedCallback callback )
		{
			Console.WriteLine( "Disconnected from Steam" );

			isRunning = false;
		}

		private void OnLoggedOn( SteamUser.LoggedOnCallback callback )
		{
			if ( callback.Result != EResult.OK )
			{
				if ( callback.Result == EResult.AccountLogonDenied )
				{
					// if we recieve AccountLogonDenied or one of it's flavors (AccountLogonDeniedNoMailSent, etc)
					// then the account we're logging into is SteamGuard protected
					// see sample 6 for how SteamGuard can be handled

					Console.WriteLine( "Unable to logon to Steam: This account is SteamGuard protected." );

					isRunning = false;
					return;
				}

				Console.WriteLine( "Unable to logon to Steam: {0} / {1}", callback.Result, callback.ExtendedResult );

				isRunning = false;
				return;
			}

			Console.WriteLine( "Successfully logged on!" );
			// at this point, we'd be able to perform actions on Steam
			Console.WriteLine( "Joining chat room '{0}'...", ChatId );
			steamFriends.JoinChat (new SteamID(ChatId));
		}

		private void OnChatEnter( SteamFriends.ChatEnterCallback callback )
		{
			Console.WriteLine( "Successfully joined chat!" );
		}

		public void SendChatRoomMessage(String message) {
			steamFriends.SendChatRoomMessage (
				new SteamID (ChatId), 
				EChatEntryType.ChatMsg,
				message
			);
		}

		private void OnChatMsg( SteamFriends.ChatMsgCallback callback )
		{

			string message = callback.Message;
			string sender = steamFriends.GetFriendPersonaName(callback.ChatterID);

			foreach (IGroupChatAddin cmd in AddinManager.GetExtensionObjects<IGroupChatAddin> ())
			{
				cmd.MessageReceived(new ChatMessage(message, sender));
			}

		}

		private void OnLoggedOff( SteamUser.LoggedOffCallback callback )
		{
			Console.WriteLine( "Logged off of Steam: {0}", callback.Result );
		}
	}
}

