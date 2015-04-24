using System;
using MoistureBot;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace MoistureBotTest
{

    public class CallTracker 
    {
        public List<String> Calls = new List<String>();

        public bool HasCalled(String methodName)
        {
            foreach (String str in Calls)
            {
                Console.WriteLine(str);
            }
            return Calls.Contains(methodName);
        }

        public void TrackMethod() {
            Calls.Add(GetCurrentMethod());
        }

        public void Reset() {
            Calls.Clear();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod ()
        {
            StackTrace st = new StackTrace ();
            StackFrame sf = st.GetFrame (2);

            return sf.GetMethod().Name;
        }
    }

    public class TestContext : IContext 
    {

        public CallTracker Tracker;

        public TestContext(CallTracker tracker) {
            this.Tracker = tracker;
        }

        #region IContext implementation

        public void InvokeAddins<AddinType>(Action<AddinType> onNext)
        {
            Tracker.TrackMethod();
        }

        public void InvokeAddins<AddinType>(string path, Action<AddinType> onNext)
        {
            Tracker.TrackMethod();
        }

        public void InvokeAddins<AddinType, NodeType>(string path, Func<NodeType, bool> predicate, Action<AddinType> onNext)
        {
            Tracker.TrackMethod();
        }

        public T GetInstanceWithContext<T>(Type type)
        {          
            Tracker.TrackMethod();
            return default(T);
        }

        public IMoistureBot GetBot()
        {
            Tracker.TrackMethod();
            return new TestBot(Tracker);
        }

        public IConfig GetConfig()
        {
            Tracker.TrackMethod();
            return new TestConfig(Tracker);
        }

        public ILogger GetLogger(Type type)
        {
            Tracker.TrackMethod();
            return new TestLogger(Tracker);
        }

        #endregion


    }

    public class TestBot : IMoistureBot {

        CallTracker Tracker;

        String _personaName = "name";

        public TestBot(CallTracker tracker)
        {
            this.Tracker = tracker;
        }

        #region IMoistureBot implementation
        public bool IsConnected()
        {
            Tracker.TrackMethod();
            return true;
        }
        public void Connect(string username, string password)
        {
            Tracker.TrackMethod();
        }
        public void Disconnect()
        {
            Tracker.TrackMethod();
        }
        public void JoinChatRoom(ulong id)
        {
            Tracker.TrackMethod();
        }
        public void LeaveChatRoom(ulong id)
        {
            Tracker.TrackMethod();
        }
        public void SendChatMessage(string message, ulong userId)
        {
            Tracker.TrackMethod();
        }
        public void SendChatRoomMessage(string message, ulong chatRoomId)
        {
            Tracker.TrackMethod();
        }
        public string GetPersonaName(ulong id)
        {
            Tracker.TrackMethod();
            return "name";
        }
        public void Terminate()
        {
            Tracker.TrackMethod();
        }
        public void KickChatMember(ulong roomId, ulong userId)
        {
            Tracker.TrackMethod();
        }
        public void BanChatMember(ulong roomId, ulong userId)
        {
            Tracker.TrackMethod();
        }
        public void UnbanChatMember(ulong roomId, ulong userId)
        {
            Tracker.TrackMethod();
        }
        public MoistureBot.Model.OnlineStatus GetOnlineStatus()
        {
            Tracker.TrackMethod();
            return MoistureBot.Model.OnlineStatus.ONLINE;
        }
        public void SetOnlineStatus(MoistureBot.Model.OnlineStatus state)
        {
            Tracker.TrackMethod();
        }
        public void SetOnlineStatus(string status)
        {
            Tracker.TrackMethod();
        }
        public string Username { get; internal set; }
        public string Password {
            get;
            internal set;
        }
        public string PersonaName {
            get { return _personaName; }
            set { _personaName = value; }
        }
        #endregion

    }

    public class TestConfig : IConfig {

        CallTracker Tracker;

        public TestConfig(CallTracker tracker)
        {
            this.Tracker = tracker;
        }

        #region IConfig implementation
        public void CreateConfig()
        {
            Tracker.TrackMethod();
        }
        public bool ConfigExists()
        {
            Tracker.TrackMethod();
            return true;
        }
        public System.Collections.Generic.Dictionary<string, ulong> GetFavoriteUsers()
        {
            Tracker.TrackMethod();
            var dict = new Dictionary<string, ulong>();
            dict.Add("key", 123);
            return dict;
        }
        public string GetFavoriteUserId(string key)
        {
            Tracker.TrackMethod();
            return "id";
        }
        public System.Collections.Generic.Dictionary<string, ulong> GetFavoriteChatRooms()
        {
            Tracker.TrackMethod();
            var dict = new Dictionary<string, ulong>();
            dict.Add("key", 123);
            return dict;
        }
        public string GetFavoriteChatRoomId(string key)
        {
            Tracker.TrackMethod();
            return "id";
        }
        public bool AddFavoriteUser(string key, ulong userId)
        {
            Tracker.TrackMethod();
            return true;
        }
        public bool AddFavoriteChatRoom(string key, ulong chatRoomId)
        {
            Tracker.TrackMethod();
            return true;
        }
        public bool RemoveFavoriteUser(string key)
        {
            Tracker.TrackMethod();
            return true;
        }
        public void RemoveAllFavoriteUsers()
        {
            Tracker.TrackMethod();
        }
        public bool RemoveFavoriteChatRoom(string key)
        {
            Tracker.TrackMethod();
            return true;
        }
        public void RemoveAllFavoriteChatRooms()
        {
            Tracker.TrackMethod();
        }
        public void SetSetting(string section, string key, string value)
        {
            Tracker.TrackMethod();
        }
        public void SetSetting(MoistureBot.Config.ConfigSetting setting, string value)
        {
            Tracker.TrackMethod();
        }
        public string GetSetting(MoistureBot.Config.ConfigSetting setting)
        {
            Tracker.TrackMethod();
            return "setting";
        }
        public string GetSetting(string section, string key)
        {
            Tracker.TrackMethod();
            return "setting";
        }
        #endregion

    }

    public class TestLogger : ILogger 
    {

        CallTracker Tracker = new CallTracker();

        public TestLogger(CallTracker tracker)
        {
            this.Tracker = tracker;
        }

        #region ILogger implementation
        public void Debug(string message)
        {
            Tracker.TrackMethod();
        }
        public void Info(string message)
        {
            Tracker.TrackMethod();
        }
        public void Warn(string message)
        {
            Tracker.TrackMethod();
        }
        public void Error(string message, Exception e)
        {
            Tracker.TrackMethod();
        }
        public void Fatal(string message)
        {
            Tracker.TrackMethod();
        }
        #endregion
    }

}

