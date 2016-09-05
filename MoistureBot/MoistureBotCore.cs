using System;
using System.Linq;
using System.Text;
using SteamKit2;
using System.Text.RegularExpressions;
using Mono.Addins;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;
using MoistureBot;
using MoistureBot.Model;
using MoistureBot.Config;
using MoistureBot.Extensions;
using System.IO;
using System.Net;
using SteamKit2.Internal;
using SteamKit2.Discovery;

namespace MoistureBot
{

    public class MoistureBotCore : IMoistureBot
    {

        private ILogger Logger;
        private IConfig Config;

        private IContext context;
	
        private volatile bool terminated;

        private SteamClient steamClient;
        private CallbackManager manager;

        private SteamUser steamUser;
        private SteamFriends steamFriends;

        // Bot steam user properties
        private volatile string user;
        private volatile string pass;

        private volatile bool isLoggedIn = false;

        public MoistureBotCore(IContext context)
        {

            this.context = context;
            Logger = context.GetLogger(typeof(MoistureBotCore));
            Config = context.GetConfig();

            steamClient = new SteamClient();

            steamUser = steamClient.GetHandler<SteamUser>();
            steamFriends = steamClient.GetHandler<SteamFriends>();

            manager = new CallbackManager(steamClient);

            manager.Subscribe<SteamClient.ConnectedCallback>(ConnectedCallback);
            manager.Subscribe<SteamClient.ConnectedCallback>(ConnectedCallback);
            manager.Subscribe<SteamClient.DisconnectedCallback>(DisconnectedCallback);

            // User
            manager.Subscribe<SteamUser.LoggedOnCallback>(LoggedOnCallback);
            manager.Subscribe<SteamUser.LoggedOffCallback>(LoggedOffCallback);
            manager.Subscribe<SteamUser.AccountInfoCallback>(AccountInfoCallback);

            // Friends
            manager.Subscribe<SteamFriends.ChatEnterCallback>(ChatEnterCallback);
            manager.Subscribe<SteamFriends.ChatMsgCallback>(ChatMsgCallback);
            manager.Subscribe<SteamFriends.ChatInviteCallback>(ChatInviteCallback);
            manager.Subscribe<SteamFriends.ChatActionResultCallback>(ChatActionResultCallback);
            manager.Subscribe<SteamFriends.ChatMemberInfoCallback>(ChatMemberInfoCallback);
            manager.Subscribe<SteamFriends.FriendsListCallback>(FriendsListCallback);
            manager.Subscribe<SteamFriends.PersonaStateCallback>(PersonaStateCallback);
            manager.Subscribe<SteamFriends.ProfileInfoCallback>(ProfileInfoCallback);

            manager.Subscribe<SteamFriends.FriendMsgCallback>(FriendMsgCallback);

        }

        private void BotThread()
        {
            Logger.Info("Initializing Steam server list.");
            ConfigureServers();
            while (!terminated)
            {
                try
                {
                    manager.RunWaitCallbacks(TimeSpan.FromSeconds(3));
                }
                catch(InvalidOperationException e)
                {
                    // There is probably a bug in SteamKit2 causing this exception sometimes when connecting
                    Logger.Error("Error in bot callback thread",e);
                }
                catch(Exception e)
                {
                    Logger.Error("Error in bot callback thread",e);
                }
            }
        }

        private void ConfigureServers()
        {

            CMClient.Servers.ServerListProvider = new FileStorageServerListProvider("servers.bin");


            var cellid = 0u;
            try
            {
                cellid = UInt32.Parse(Config.GetSetting(ConfigSetting.CELL_ID));
            }
            catch
            {
                Logger.Warn("Failed to parse cell id from settings. Using default value 0u.");
            }

            var loadTask = SteamDirectory.Initialize(cellid);
            loadTask.Wait();

            if (loadTask.IsFaulted)
            {
                Logger.Error("Error while loading server list from Steam directory.", loadTask.Exception);
                return;
            }

        }

        private string GetPersonaName(SteamID id)
        {
            return steamFriends.GetFriendPersonaName(id);
        }

        void ProfileInfoCallback(SteamFriends.ProfileInfoCallback obj)
        {
            Logger.Info("Profile info callback fired.");
        }

        void PersonaStateCallback(SteamFriends.PersonaStateCallback obj)
        {
            Logger.Debug("Persona state callback fired with state " + obj.StatusFlags);
        }

        void FriendsListCallback(SteamFriends.FriendsListCallback obj)
        {
            Logger.Info("Friends list callback fired.");
        }

        void ChatActionResultCallback(SteamFriends.ChatActionResultCallback obj)
        {
            // TODO: This one never gets called. SteamKit2 problem?
            Logger.Info("Chat action callback fired.");

            switch(obj.Action)
            {
                case EChatAction.Ban:
                    if (obj.Result != EChatActionResult.Success)
                        Logger.Info("User " + obj.ChatterID.ConvertToUInt64() + " banned from " + obj.ChatRoomID.ConvertToUInt64());
                    else
                        Logger.Info("Failed to ban user: " + obj.Result);
                    break;
                case EChatAction.Kick:
                    if (obj.Result != EChatActionResult.Success)
                        Logger.Info("User " + obj.ChatterID.ConvertToUInt64() + " kicked from " + obj.ChatRoomID.ConvertToUInt64());
                    else
                        Logger.Info("Failed to kick user: " + obj.Result);
                    break;
                case EChatAction.CloseChat:
                    if (obj.Result != EChatActionResult.Success)
                        Logger.Info("Chat room " + obj.ChatRoomID.ConvertToUInt64() + " closed.");
                    else
                        Logger.Info("Failed to close chat room: " + obj.Result);
                    break;
                case EChatAction.InviteChat:
                    if (obj.Result != EChatActionResult.Success)
                        Logger.Info("User " + obj.ChatterID.ConvertToUInt64() + " invited to " + obj.ChatRoomID.ConvertToUInt64());
                    else
                        Logger.Info("Failed to close invite user: " + obj.Result);
                    break;
                case EChatAction.UnBan:
                    if (obj.Result != EChatActionResult.Success)
                        Logger.Info("User " + obj.ChatterID.ConvertToUInt64() + " unbanned from " + obj.ChatRoomID.ConvertToUInt64());
                    else
                        Logger.Info("Failed to unban user: " + obj.Result);
                    break;
            } 
        }

        private void ConnectedCallback(SteamClient.ConnectedCallback callback)
        {

            Logger.Info("Connected callback fired with result: " + callback.Result);

            if (callback.Result != EResult.OK)
            {
                Logger.Info("Unable to connect to Steam: " + callback.Result.ToString());
                return;
            }
            Logger.Info("Connected to Steam!");
            Logger.Info("Logging in '" + user + "'...");

            if (!isLoggedIn)
            {
                isLoggedIn = true;
                steamUser.LogOn(new SteamUser.LogOnDetails {
                    Username = user,
                    Password = pass,
                });
            }
            else
            {
                Logger.Info("User already logged in!");
            }
        }

        void ChatMemberInfoCallback(SteamFriends.ChatMemberInfoCallback callback)
        {
            Logger.Info("Chat member info callback fired");

            Logger.Debug("Chat info type: " + callback.Type);
            Logger.Debug("Chatter acted by: " + callback.StateChangeInfo.ChatterActedBy.ConvertToUInt64());
            Logger.Debug("Chatter acted on  " + callback.StateChangeInfo.ChatterActedOn.ConvertToUInt64());
            Logger.Debug("State change: " + callback.StateChangeInfo.StateChange);

            if ((callback.StateChangeInfo.StateChange & EChatMemberStateChange.Entered) == EChatMemberStateChange.Entered)
            {
                // TODO: Extension point
                Logger.Info("User entered chat");
            }

            if ((callback.StateChangeInfo.StateChange & EChatMemberStateChange.Left) == EChatMemberStateChange.Left)
            {
                // TODO: Extension point
                Logger.Info("User left chat");
            }

            if ((callback.StateChangeInfo.StateChange & EChatMemberStateChange.Disconnected) == EChatMemberStateChange.Disconnected)
            {
                // TODO: Extension point
                Logger.Info("User disconnected chat");
            }

            if ((callback.StateChangeInfo.StateChange & EChatMemberStateChange.Kicked) == EChatMemberStateChange.Kicked)
            {
                // TODO: Extension point
                Logger.Info("User kicked from chat");
            }

            if ((callback.StateChangeInfo.StateChange & EChatMemberStateChange.Banned) == EChatMemberStateChange.Banned)
            {
                // TODO: Extension point
                Logger.Info("User kicked from chat");
            }

        }

        private void AccountInfoCallback(SteamUser.AccountInfoCallback callback)
        {
            // before being able to interact with friends, you must wait for the account info callback
            // this callback is posted shortly after a successful logon

            // at this point, we can set online status
            Logger.Info("Account info callback fired.");

            string configState;
            try
            {
                configState = Config.GetSetting(ConfigSetting.STATUS);
            }
            catch(Exception e)
            {
                Logger.Error("Failed to read online status from config file",e);
                return;
            }

            try
            {
                SetOnlineStatus(configState);
            }
            catch(ArgumentException)
            {
                Logger.Info("Invalid value for online status in config, setting to default value.");
                SetOnlineStatus(ConfigUtils.GetValue<DefaultValueAttribute>(ConfigSetting.STATUS));
            }

        }

        private void DisconnectedCallback(SteamClient.DisconnectedCallback callback)
        {
            Logger.Info("Disconnected from Steam.");
            isLoggedIn = false;
        }

        private void LoggedOnCallback(SteamUser.LoggedOnCallback callback)
        {

            Logger.Info("Logged on event received");

            if (callback.Result != EResult.OK)
            {
                isLoggedIn = false;
                if (callback.Result == EResult.AccountLogonDenied)
                {
                    // if we recieve AccountLogonDenied or one of it's flavors (AccountLogonDeniedNoMailSent, etc)
                    // then the account we're logging into is SteamGuard protected
                    // see sample 6 for how SteamGuard can be handled

                    Logger.Warn("Unable to logon to Steam: This account is SteamGuard protected.");
                    return;
                }

                Logger.Info("Unable to logon to Steam: " + callback.ExtendedResult);
               
                return;
            }

            Config.SetSetting(ConfigSetting.CELL_ID, callback.CellID.ToString());
            Logger.Info("Successfully logged on!");

        }

        private void ChatEnterCallback(SteamFriends.ChatEnterCallback callback)
        {
            Logger.Info("Chat enter callback fired");

            switch(callback.EnterResponse)
            {
                case EChatRoomEnterResponse.Success:
                    Logger.Info("Successfully joined chat!");
                    break;
                case EChatRoomEnterResponse.Limited:
                    Logger.Info("Failed to join chat: " + callback.EnterResponse);
                    Logger.Info("The user account needs to have one game in it's library in order to join chat rooms");
                    break;
                default:
                    Logger.Info("Failed to join chat: " + callback.EnterResponse);
                    break;
            }

        }

        private void ChatMsgCallback(SteamFriends.ChatMsgCallback callback)
        {

            // NOTE: Only the EChatEntryType.ChatMsg callbacks seem to be getting called

            Logger.Info("Chat room message callback fired");

            string message = callback.Message;
            ulong chatterId = callback.ChatterID.ConvertToUInt64();
            ulong chatId = callback.ChatRoomID.ConvertToUInt64();

            switch(callback.ChatMsgType)
            {
                case EChatEntryType.ChatMsg:

                    context.InvokeAddins<IReceiveGroupChatMessages>(
                        addin => addin.MessageReceived(new GroupChatMessage(message, chatterId, chatId))
                    );

                    break;
                case EChatEntryType.LeftConversation:
                    Logger.Info(chatterId + " left " + chatId);
                    break;
                case EChatEntryType.Disconnected:
                    Logger.Info(chatterId + " disconnected from " + chatId);
                    break;
                case EChatEntryType.WasBanned:
                    Logger.Info(chatterId + " was banned from " + chatId);
                    break;
                case EChatEntryType.WasKicked:
                    Logger.Info(chatterId + " was kicked from " + chatId);
                    break;
                case EChatEntryType.Entered:
                    Logger.Info(chatterId + " entered room " + chatId);
                    break;
            }
        }

        private void FriendMsgCallback(SteamFriends.FriendMsgCallback callback)
        {

            Logger.Info("Friend chat message callback fired");

            string message = callback.Message;
            ulong chatterId = callback.Sender.ConvertToUInt64();

            switch(callback.EntryType)
            {
                case EChatEntryType.ChatMsg:
                    Logger.Info("Received message from " + chatterId + ": " + message);

                    context.InvokeAddins<IReceiveFriendChatMessages>(
                        (addin) => addin.MessageReceived(new FriendChatMessage(message, chatterId))
                    );

                    break;
                case EChatEntryType.InviteGame:
                    Logger.Info("Game invite received from user " + chatterId);
                    break;
            }

        }

        private void ChatInviteCallback(SteamFriends.ChatInviteCallback callback)
        {
            Logger.Info("Chat invite callback fired");

            Logger.Debug("Callback chatroom id: " + callback.ChatRoomID);
            Logger.Debug("Callback chatroom name: " + callback.ChatRoomName);
            Logger.Debug("Callback chatroom type: " + callback.ChatRoomType);
            Logger.Debug("Callback chat friend id: " + callback.FriendChatID);
            Logger.Debug("Callback game id: " + callback.GameID);
            Logger.Debug("Callback invited id: " + callback.InvitedID);
            Logger.Debug("Callback patron id: " + callback.PatronID);

            switch(callback.ChatRoomType)
            {
                case EChatRoomType.MUC: // both community and friend group chat
				
                    if (String.IsNullOrEmpty(callback.ChatRoomName))
                    {
                        var friendInvite = new FriendGroupChatInvite(
                                               callback.ChatRoomID.ConvertToUInt64(),
                                               callback.PatronID.ConvertToUInt64()
                                           );

                        context.InvokeAddins<IReceiveFriendGroupChatInvites>(
                            (addin) => addin.InviteReceived(friendInvite)
                        );

                    }
                    else
                    {
                        var communityInvite = new CommunityGroupChatInvite(
                                                  callback.ChatRoomID.ConvertToUInt64(),
                                                  callback.ChatRoomName,
                                                  callback.PatronID.ConvertToUInt64()
                                              );

                        context.InvokeAddins<IReceiveCommunityGroupChatInvites>(
                            (addin) => addin.InviteReceived(communityInvite)
                        );

                    }

                    break;
                case EChatRoomType.Lobby: // Game invite

                    var gameInvite = new GameLobbyInvite(
                                         callback.ChatRoomID.ConvertToUInt64(),
                                         callback.PatronID.ConvertToUInt64(),
                                         callback.GameID.ToUInt64()
                                     );

                    context.InvokeAddins<IReceiveGameLobbyInvites>(
                        (addin) => addin.InviteReceived(gameInvite)
                    );

                    break;
                case EChatRoomType.Friend:
					// What is this, doesn't get called?
                    break;

            }

        }

        private void LoggedOffCallback(SteamUser.LoggedOffCallback callback)
        {
            Logger.Info("Logged off of Steam: " + callback.Result);
            isLoggedIn = false;
            Logger.Info("Disconnecting steam client...");
            steamClient.Disconnect();
        }

        #region PUBLIC

        public string Username { get { return user; } }

        public string Password { get { return pass; } }

        public void Start()
        {
            try
            {
                Logger.Info("Starting bot...");
                Thread t = new Thread(BotThread);
                t.Start();
            }
            catch(ThreadStateException e)
            {
                Logger.Error("Failed to start bot thread",e);
            }
            catch(ThreadInterruptedException e)
            {
                Logger.Error("Bot thread interrupted",e);
            }
        }

        public string PersonaName
        {
            get
            {
                Logger.Info("Getting bot persona name");
                return steamFriends.GetPersonaName();
            }
            set
            {
                Logger.Info("Setting bot persona name to " + value);
                steamFriends.SetPersonaName(value);
            }
        }

        public void KickChatMember(ulong roomId, ulong userId)
        {
            Logger.Info("Kicking user " + userId + " from room " + roomId);
            steamFriends.KickChatMember(new SteamID(roomId),new SteamID(userId));
        }

        public void Connect(string username, string password)
        {
            Logger.Info("Connecting to Steam...");
            user = username;
            pass = password;
            steamClient.Connect();
        }

        public void Disconnect()
        {
            user = null;
            pass = null;
            if (isLoggedIn)
            {
                Logger.Info("Logging out...");
                steamUser.LogOff();
            }
            else
            {
                Logger.Info("Disconnecting Steam client...");
                steamClient.Disconnect();
            }
        }

        public OnlineStatus GetOnlineStatus()
        {
            var state = steamFriends.GetPersonaState();
            return MoistureBotConverter.ToOnlineStatus(state);
        }

        public void SetOnlineStatus(string status)
        {

            if (status == null)
            {
                Logger.Warn("Failed to set online status: Status cannot be null.");
                throw new ArgumentException("Invalid status");
            }

            foreach (OnlineStatus ps in Enum.GetValues(typeof(OnlineStatus)))
            {
                var str = ConfigUtils.GetValue<StringAttribute>(ps);
                if (status.Equals(str))
                {
                    SetOnlineStatus(ps);
                    return;
                }
            }

            Logger.Warn("Failed to set online status: Invalid value '" + status + "'");

            throw new ArgumentException("Invalid status");

        }

        public void SetOnlineStatus(OnlineStatus status)
        {

            Logger.Info("Setting online status to " + status);

            steamFriends.SetPersonaState(MoistureBotConverter.ToEPersonaState(status));

            Config.SetSetting(
                ConfigSetting.STATUS,
                ConfigUtils.GetValue<StringAttribute>(status));
        }

        public void JoinChatRoom(ulong roomId)
        {
            Logger.Info("Joining chat room " + roomId + "...");
            steamFriends.JoinChat(new SteamID(roomId));
        }

        public void LeaveChatRoom(ulong roomId)
        {
            Logger.Info("Leaving chat room " + roomId + "...");
            steamFriends.LeaveChat(new SteamID(roomId));
        }

        public void SendChatRoomMessage(String message, ulong chatRoomId)
        {

            Logger.Info("Sending message to chat room " + chatRoomId);

            if (string.IsNullOrEmpty(message))
            {
                Logger.Info("Trying to send empty message to chat room " + chatRoomId);
                return;
            }
				
            try
            {
                steamFriends.SendChatRoomMessage(
                    new SteamID(chatRoomId), 
                    EChatEntryType.ChatMsg,
                    message
                );
            }
            catch(Exception e)
            {
                Logger.Error("Failed to send message to chat room " + chatRoomId,e);
            }
        }

        public void SendChatMessage(String message, ulong userId)
        {

            if (string.IsNullOrEmpty(message))
            {
                Logger.Info("Trying to send empty message to user " + userId);
                return;
            }

            try
            {
                steamFriends.SendChatMessage(
                    new SteamID(userId), 
                    EChatEntryType.ChatMsg,
                    message
                );
            }
            catch(Exception e)
            {
                Logger.Error("Failed to send message to user " + userId,e);
            }
        }

        public string GetPersonaName(ulong id)
        {
            return steamFriends.GetFriendPersonaName(new SteamID(id));
        }

        public Boolean IsConnected()
        {
            return steamClient.IsConnected;
        }

        public void Terminate()
        {
            Logger.Info("Terminating Steam client.");

            if (steamUser != null)
                steamUser.LogOff();
            if (steamClient != null)
                steamClient.Disconnect();

            terminated = true;
                
        }

        public void BanChatMember(ulong roomId, ulong userId)
        {
            Logger.Info("Banning chat member " + userId + " in room " + roomId);
            steamFriends.BanChatMember(new SteamID(roomId),new SteamID(userId));
        }

        public void UnbanChatMember(ulong roomId, ulong userId)
        {
            Logger.Info("Unbanning chat member " + userId + " in room " + roomId);
            steamFriends.UnbanChatMember(new SteamID(roomId),new SteamID(userId));
        }

        #endregion 

    }
}

