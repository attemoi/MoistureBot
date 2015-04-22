using NUnit.Framework;
using System;
using MoistureBot;
using System.Collections.Generic;
using MoistureBot.Model;
using Mono.Addins;

namespace MoistureBotTest
{
    [TestFixture()]
    public class Test
    {
        [Test()]
        public void TestChatCommandParsing()
        {
            TestContext context = new TestContext();
            ChatCommand cc = new ChatCommand(context);

            cc.MessageReceived(new FriendChatMessage("message", 123));
            Assert.AreEqual(0, context.Invokations);
            cc.MessageReceived(new FriendChatMessage("!", 123));
            Assert.AreEqual(0, context.Invokations);
            cc.MessageReceived(new FriendChatMessage("!command", 123));
            Assert.AreEqual(1, context.Invokations);
        }
    }

    public class TestContext : IContext 
    {

        public int Invokations;
        
        #region IContext implementation

        public void InvokeAddins<AddinType>(Action<AddinType> onNext)
        {
            
        }

        public void InvokeAddins<AddinType>(string path, Action<AddinType> onNext)
        {
            
        }

        public void InvokeAddins<AddinType, NodeType>(string path, Func<NodeType, bool> predicate, Action<AddinType> onNext)
        {
            Invokations++;
        }

        public T GetInstanceWithContext<T>(Type type)
        {
            throw new NotImplementedException();
        }

        public IMoistureBot GetBot()
        {
            return new TestBot();
        }

        public IConfig GetConfig()
        {
            return new TestConfig();
        }

        public ILogger GetLogger(Type type)
        {
            return new TestLogger();
        }

        #endregion


    }

    public class TestBot : IMoistureBot {
        
        #region IMoistureBot implementation
        public bool IsConnected()
        {
            return true;
        }
        public void Connect(string username, string password)
        {
            
        }
        public void Disconnect()
        {
            
        }
        public void JoinChatRoom(ulong id)
        {
            
        }
        public void LeaveChatRoom(ulong id)
        {
            
        }
        public void SendChatMessage(string message, ulong userId)
        {
            
        }
        public void SendChatRoomMessage(string message, ulong chatRoomId)
        {
            
        }
        public string GetPersonaName(ulong id)
        {
            return "name";
        }
        public void Terminate()
        {
            
        }
        public void KickChatMember(ulong roomId, ulong userId)
        {
            
        }
        public void BanChatMember(ulong roomId, ulong userId)
        {
           
        }
        public void UnbanChatMember(ulong roomId, ulong userId)
        {
            
        }
        public MoistureBot.Model.OnlineStatus GetOnlineStatus()
        {
            return MoistureBot.Model.OnlineStatus.ONLINE;
        }
        public void SetOnlineStatus(MoistureBot.Model.OnlineStatus state)
        {
            
        }
        public void SetOnlineStatus(string status)
        {
            
        }
        public string Username { get; internal set; }
        public string Password {
            get;
            internal set;
        }
        public string PersonaName {
            get;
            set;
        }
        #endregion
        
    }

    public class TestConfig : IConfig {
        #region IConfig implementation
        public void CreateConfig()
        {
            
        }
        public bool ConfigExists()
        {
            return true;
        }
        public System.Collections.Generic.Dictionary<string, ulong> GetFavoriteUsers()
        {
            var dict = new Dictionary<string, ulong>();
            dict.Add("key", 123);
            return dict;
        }
        public string GetFavoriteUserId(string key)
        {
            return "id";
        }
        public System.Collections.Generic.Dictionary<string, ulong> GetFavoriteChatRooms()
        {
            var dict = new Dictionary<string, ulong>();
            dict.Add("key", 123);
            return dict;
        }
        public string GetFavoriteChatRoomId(string key)
        {
            return "id";
        }
        public bool AddFavoriteUser(string key, ulong userId)
        {
            return true;
        }
        public bool AddFavoriteChatRoom(string key, ulong chatRoomId)
        {
            return true;
        }
        public bool RemoveFavoriteUser(string key)
        {
            return true;
        }
        public void RemoveAllFavoriteUsers()
        {
            
        }
        public bool RemoveFavoriteChatRoom(string key)
        {
            return true;
        }
        public void RemoveAllFavoriteChatRooms()
        {
            
        }
        public void SetSetting(string section, string key, string value)
        {
            
        }
        public void SetSetting(MoistureBot.Config.ConfigSetting setting, string value)
        {
            
        }
        public string GetSetting(MoistureBot.Config.ConfigSetting setting)
        {
            return "setting";
        }
        public string GetSetting(string section, string key)
        {
            return "setting";
        }
        #endregion
        
    }

    public class TestLogger : ILogger 
    {
        #region ILogger implementation
        public void Debug(string message)
        {
           
        }
        public void Info(string message)
        {
            
        }
        public void Warn(string message)
        {
            
        }
        public void Error(string message, Exception e)
        {
            
        }
        public void Fatal(string message)
        {
            
        }
        #endregion
    }

}

