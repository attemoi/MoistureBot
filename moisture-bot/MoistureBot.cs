using System;
using System.Linq;
using System.Text;

using SteamKit2;
using System.Text.RegularExpressions;

namespace moisturebot
{
	public class MoistureBot
	{
		private SteamClient steamClient;
		private CallbackManager manager;

		private SteamUser steamUser;
		private SteamFriends steamFriends;

		private bool isRunning;

		// Bot properties
		public string user { get; set; }
		public string pass { get; set; }
		public ulong chatId { get; set; }

		// Events
		public delegate void ChatMsgHandler (object sender, ChatMsgEventArgs data);
		public event ChatMsgHandler chatMsgHandler;

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
			new Callback<SteamFriends.ChatMsgCallback> ( OnChatMsgReceived, manager);

		}

		public void start()
		{
			isRunning = true;

			Console.WriteLine( "Connecting to Steam..." );

			steamClient.Connect();

			// create our callback handling loop
			while ( isRunning )
			{
				// in order for the callbacks to get routed, they need to be handled by the manager
				manager.RunWaitCallbacks( TimeSpan.FromSeconds( 1 ) );
			}

		}

		public void stop()
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

			Console.WriteLine( "Connected to Steam! Logging in '{0}'...", user );

			steamUser.LogOn( new SteamUser.LogOnDetails
				{
					Username = user,
					Password = pass,
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
			Console.WriteLine( "Joining chat room '{0}'...", chatId );
			steamFriends.JoinChat (new SteamID(chatId));
		}

		private void OnChatEnter( SteamFriends.ChatEnterCallback callback )
		{
			Console.WriteLine( "Successfully joined chat!" );
		}

		protected void OnChatMsg (object sender, ChatMsgEventArgs data)
		{
			if (chatMsgHandler != null) {
				chatMsgHandler (this, data);
			}
		}

		private void OnChatMsgReceived( SteamFriends.ChatMsgCallback callback )
		{

			string message = callback.Message;
			string sender = steamFriends.GetFriendPersonaName(callback.ChatterID);

			OnChatMsg(this, new ChatMsgEventArgs( callback ));

			string[] greetings = {
				"Moi", "Moikka", "Terve", "Hello", "Tsau",
				"Hei", "Moi kaikki", "Moikka taas", "Moikkamoi",
				"Heippa", "Heips", "Moips", "Moik", "Hoi",
				"Hola", "Iltaa", "Päivää", "Huomenta",
				"Moi Moisture-bot"
				};

			Regex rgx = new Regex("[^a-zA-Z0-9 -]");
			string originalMsg = callback.Message;
			string strippedMsg = rgx.Replace(callback.Message, "").ToLower();
			// Find message index, ignore case
			int msgIndex = Array.FindIndex(greetings, t => t.IndexOf(strippedMsg, StringComparison.InvariantCultureIgnoreCase) >=0);

			if (!strippedMsg.Equals("") && msgIndex > -1) {

				Console.WriteLine ("Greeting received, replying");
				steamFriends.SendChatRoomMessage (
					new SteamID (chatId), 
					EChatEntryType.ChatMsg,
					greetings [msgIndex] + " " + sender + "!"
				);
			}

			if (strippedMsg.StartsWith ("miksi") || strippedMsg.StartsWith ("miksei")) {
				steamFriends.SendChatRoomMessage (
					new SteamID (chatId), 
					EChatEntryType.ChatMsg,
					"En tiedä :("
				);
			}

		}

		private void OnLoggedOff( SteamUser.LoggedOffCallback callback )
		{
			Console.WriteLine( "Logged off of Steam: {0}", callback.Result );
		}
	}
}

