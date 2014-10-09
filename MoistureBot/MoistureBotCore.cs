using System;
using System.Linq;
using System.Text;
using SteamKit2;
using System.Text.RegularExpressions;
using Mono.Addins;
using System.Threading;
using System.Collections.Generic;
using IniParser.Model;
using MoistureBot.Config;
using System.Reflection;

namespace MoistureBot
{

	[Extension(typeof(IMoistureBot)) ]
	public class MoistureBotCore : IMoistureBot
	{
	
		private ILogger Logger = AddinManager.GetExtensionObjects<ILogger>().First();
	
		private volatile bool terminated;

		private SteamClient steamClient;
		private CallbackManager manager;

		private SteamUser steamUser;
		private SteamFriends steamFriends;

		// Bot properties
		private string user;

		public string User { get { return user; } }

		private string pass;
		private List<ulong> activeChatRooms = new List<ulong> ();

		public MoistureBotCore ()
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
				if (obj.Result != EChatActionResult.Success)
					Logger.Debug ("User " + obj.ChatterID.ConvertToUInt64 () + " banned from " + obj.ChatRoomID.ConvertToUInt64 ());
				else
					Logger.Debug ("Failed to ban user: " + obj.Result);
				break;
			case EChatAction.Kick:
				if (obj.Result != EChatActionResult.Success)
					Logger.Debug ("User " + obj.ChatterID.ConvertToUInt64 () + " kicked from " + obj.ChatRoomID.ConvertToUInt64 ());
				else
					Logger.Debug ("Failed to kick user: " + obj.Result);
				break;
			case EChatAction.CloseChat:
				if (obj.Result != EChatActionResult.Success) {
					Logger.Debug ("Chat room " + obj.ChatRoomID.ConvertToUInt64 () + " closed.");
					activeChatRooms.Remove (obj.ChatRoomID.ConvertToUInt64 ());
				} else {
					Logger.Debug ("Failed to close chat room: " + obj.Result);
				}
				break;
			case EChatAction.InviteChat:
				if (obj.Result != EChatActionResult.Success)
					Logger.Debug("User " + obj.ChatterID.ConvertToUInt64() + " invited to " + obj.ChatRoomID.ConvertToUInt64() );
				else
					Logger.Debug ("Failed to close invite user: " + obj.Result);
				break;
			case EChatAction.UnBan:
				if (obj.Result != EChatActionResult.Success)
					Logger.Debug("User " + obj.ChatterID.ConvertToUInt64() + " unbanned from " + obj.ChatRoomID.ConvertToUInt64() );
				else
					Logger.Debug ("Failed to unban user: " + obj.Result);
				break;
			} 
		}

		private string GetUserName(SteamID id) {
			return steamFriends.GetFriendPersonaName(id);
		}

		public void KickChatMember (ulong roomId, ulong userId) {
			Logger.Debug ("Kicking user " + userId + " from room " + roomId);
			steamFriends.KickChatMember (new SteamID (roomId), new SteamID(userId));
		}

		public void Connect(string username, string password)
		{
			Logger.Debug ("Connecting user " + username + " to steam");
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
				Logger.Error ("Error in bot callback thread", e);
			}
			catch (ThreadInterruptedException e)
			{
				Logger.Error ("Bot thread interrupted", e);
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
					Logger.Error ("Error in bot callback thread", e);
				} catch (Exception e){
					Logger.Error ("Error in bot callback thread", e);
				}
			}
		}

		public void Disconnect()
		{
			Logger.Debug ("Disconnecting client...");
			steamClient.Disconnect();
		}


		private void ConnectedCallback( SteamClient.ConnectedCallback callback )
		{
			if ( callback.Result != EResult.OK )
			{
				Logger.Debug( "Unable to connect to Steam: " + callback.Result.ToString() );
				return;
			}

			Logger.Debug ("Connected to Steam!");
			Logger.Debug( "Logging in '" + user + "'..." );

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

			// at this point, we can set online status
			Logger.Debug ("Reading online status from config file");

			var config = new MoistureBotConfig ();

			string configState;
			try {
				configState = config.GetSetting (ConfigSetting.STATUS);
			} catch (Exception e) {
				Logger.Error("Failed to read online status from config file", e);
				return;
			}

			try {
				SetOnlineStatus (configState);
			} catch (ArgumentException e) {
				Logger.Debug ("Invalid value for online status in config, setting to default value");
				// TODO: invalid value in config, fix to default
			}

		}

		public void SetOnlineStatus(string status) {

			if (status == null)
				throw new ArgumentException ("Invalid status");

			foreach (OnlineStatus ps in Enum.GetValues(typeof(OnlineStatus))) {
				var str = EnumUtils.GetValue<StringAttribute> (ps);
				if (status.Equals(str)) {
					SetOnlineStatus (ps);
					return;
				}
			}

			throw new ArgumentException ("Invalid status");
		}

		public OnlineStatus GetOnlineStatus () {
			var state = steamFriends.GetPersonaState ();
			switch (state) {
			case EPersonaState.Away:
				return OnlineStatus.AWAY;
			case EPersonaState.Busy:
				return OnlineStatus.BUSY;
			case EPersonaState.LookingToPlay:
				return OnlineStatus.LOOKING_TO_PLAY;
			case EPersonaState.LookingToTrade:
				return OnlineStatus.LOOKING_TO_TRADE;
			case EPersonaState.Offline:
				return OnlineStatus.OFFLINE;
			case EPersonaState.Online:
				return OnlineStatus.ONLINE;
			case EPersonaState.Snooze:
				return OnlineStatus.SNOOZE;
			default:
				return OnlineStatus.OFFLINE;
			}
		}

		public void SetOnlineStatus(OnlineStatus status) {

			Logger.Debug ("Setting online status to " + status);

			switch (status) {
			case OnlineStatus.AWAY:
				steamFriends.SetPersonaState (EPersonaState.Away);
				break;
			case OnlineStatus.BUSY:
				steamFriends.SetPersonaState( EPersonaState.Busy );
				break;
			case OnlineStatus.LOOKING_TO_PLAY:
				steamFriends.SetPersonaState( EPersonaState.LookingToPlay );
				break;
			case OnlineStatus.LOOKING_TO_TRADE:
				steamFriends.SetPersonaState( EPersonaState.LookingToTrade );
				break;
			case OnlineStatus.OFFLINE:
				steamFriends.SetPersonaState( EPersonaState.Offline );
				break;
			case OnlineStatus.ONLINE:
				steamFriends.SetPersonaState( EPersonaState.Online );
				break;
			case OnlineStatus.SNOOZE:
				steamFriends.SetPersonaState( EPersonaState.Snooze );
				break;
			}

			new MoistureBotConfig().SetSetting(
				ConfigSetting.STATUS,
				EnumUtils.GetValue<StringAttribute>(status));
		}

		private void DisconnectedCallback( SteamClient.DisconnectedCallback callback )
		{
			Logger.Debug( "Disconnected from Steam" );
		}

		private void LoggedOnCallback( SteamUser.LoggedOnCallback callback )
		{

			Logger.Info ("Logged on event received");

			if ( callback.Result != EResult.OK )
			{

				if ( callback.Result == EResult.AccountLogonDenied )
				{
					// if we recieve AccountLogonDenied or one of it's flavors (AccountLogonDeniedNoMailSent, etc)
					// then the account we're logging into is SteamGuard protected
					// see sample 6 for how SteamGuard can be handled

					Logger.Debug( "Unable to logon to Steam: This account is SteamGuard protected." );
					return;
				}
					
				Logger.Debug( "Unable to logon to Steam: " + callback.ExtendedResult );
				return;
			}
		
			Logger.Debug( "Successfully logged on!" );

		}

		private void ChatEnterCallback( SteamFriends.ChatEnterCallback callback )
		{
			switch (callback.EnterResponse) {
			case EChatRoomEnterResponse.Success:
				Logger.Debug ("Successfully joined chat!");
				activeChatRooms.Add (callback.ChatID.ConvertToUInt64 ());
				break;
			case EChatRoomEnterResponse.Limited:
				Logger.Debug ("Failed to join chat: " + callback.EnterResponse);
				Logger.Debug ("The user account needs to have one game in it's library in order to join chat rooms");
				break;
			default:
				Logger.Debug ("Failed to join chat: " + callback.EnterResponse);
				activeChatRooms.Remove (callback.ChatID.ConvertToUInt64());
				break;
			}
				
		}

		private void ChatMsgCallback( SteamFriends.ChatMsgCallback callback )
		{
			string message = callback.Message;
			ulong chatterId = callback.ChatterID.ConvertToUInt64();
			ulong chatId = callback.ChatRoomID.ConvertToUInt64();

			switch (callback.ChatMsgType) {
			case EChatEntryType.ChatMsg:
				foreach (IChatRoomAddin addin in AddinManager.GetExtensionObjects<IChatRoomAddin> ())
				{
					try {
						addin.MessageReceived(new ChatRoomMessage(message, chatterId, chatId));
					} catch(Exception e) {
						Logger.Error ("Error in addin", e);
					}

				}
				break;
			case EChatEntryType.LeftConversation:
				Logger.Debug (chatterId + " left " + chatId);
				break;
			case EChatEntryType.Disconnected:
				Logger.Debug (chatterId + " disconnected from " + chatId);
				break;
			case EChatEntryType.WasBanned:
				Logger.Debug (chatterId + " was banned from " + chatId);
				break;
			case EChatEntryType.WasKicked:
				Logger.Debug (chatterId + " was kicked from " + chatId);
				break;
			case EChatEntryType.LobbyGameStart:
				Logger.Debug (chatterId + " started game " + chatId);
				break;
			case EChatEntryType.Entered:
				Logger.Debug (chatterId + " entered room " + chatId);
				break;
			}

		}

		private void FriendMsgCallback( SteamFriends.FriendMsgCallback callback )
		{

			string message = callback.Message;
			ulong chatterId = callback.Sender.ConvertToUInt64 ();

			switch (callback.EntryType) {
			case EChatEntryType.ChatMsg:
				Logger.Debug("Received message from " + chatterId + ": " + message);
				foreach (IChatFriendAddin addin in AddinManager.GetExtensionObjects<IChatFriendAddin> ())
				{
					try {
						addin.MessageReceived(new ChatMessage(message, chatterId));
					} catch(Exception e) {
						Logger.Error ("Error in addin", e);
					}
				}
				break;
			case EChatEntryType.InviteGame:
				Logger.Debug ("Game invite received from " + chatterId);
				break;
			}

		}

		private void ChatInviteCallback( SteamFriends.ChatInviteCallback callback )
		{
			Logger.Debug ("Received invite from " + callback.FriendChatID );
			// TODO create extension point
		}

		private void LoggedOffCallback( SteamUser.LoggedOffCallback callback )
		{
			Logger.Debug( "Logged off of Steam: " + callback.Result );
		}

		#region PUBLIC

		public void JoinChatRoom(ulong roomId) {
			steamFriends.JoinChat (new SteamID(roomId));
		}

		public void SendChatRoomMessage(String message, ulong chatRoomId) {

			if (string.IsNullOrEmpty (message)) {
				Logger.Debug ("Trying to send empty message to chat room " + chatRoomId);
				return;
			}
				
			try {
				steamFriends.SendChatRoomMessage (
					new SteamID (chatRoomId), 
					EChatEntryType.ChatMsg,
					message
				);
			} catch (Exception e) {
				Logger.Error ("Failed to send message to chat room " + chatRoomId, e);
			}
		}

		public void SendChatMessage(String message, ulong userId) {

			if (string.IsNullOrEmpty (message)) {
				Logger.Debug ("Trying to send empty message to user");
				return;
			}
			try {
				steamFriends.SendChatMessage (
					new SteamID (userId), 
					EChatEntryType.ChatMsg,
					message
				);
			} catch (Exception e) {
				Logger.Error ("Failed to send message to user " + userId, e);
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
			Logger.Debug ("Banning chat member " + userId + " in room " + roomId);
			steamFriends.BanChatMember (new SteamID (roomId), new SteamID (userId));
		}

		public void UnbanChatMember (ulong roomId, ulong userId)
		{
			Logger.Debug ("Unbanning chat member " + userId + " in room " + roomId);
			steamFriends.UnbanChatMember (new SteamID (roomId), new SteamID (userId));
		}

		#endregion

	}
}

