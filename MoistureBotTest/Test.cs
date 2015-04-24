using NUnit.Framework;
using System;
using MoistureBot;
using System.Collections.Generic;
using MoistureBot.Model;
using Mono.Addins;
using MoistureBot.ConsoleCommands;

namespace MoistureBotTest
{
    [TestFixture()]
    public class Test
    {

        CallTracker Tracker;
        TestContext Context;

        [SetUp] 
        public void Init()
        {
            this.Tracker = new CallTracker();
            this.Context = new TestContext(Tracker);
        }

        [Test()]
        public void TestChatCommandParsing()
        {

            ChatCommand cc = new ChatCommand(Context);

            cc.MessageReceived(new FriendChatMessage("message", 123));
            cc.MessageReceived(new FriendChatMessage("!", 123));
            cc.MessageReceived(new FriendChatMessage(" !lala!!", 123));
            Assert.IsFalse(Tracker.HasCalled("InvokeAddins"));

            cc.MessageReceived(new FriendChatMessage("!command", 123));
            Assert.IsTrue(Tracker.HasCalled("InvokeAddins"));
        }

        [Test()]
        public void TestMoikkaaja()
        {
            Moikkaaja moikkaaja = new Moikkaaja(Context);

            moikkaaja.MessageReceived(new FriendChatMessage("This is not a greeting!", 123));
            Assert.IsFalse(Tracker.HasCalled("SendChatMessage"));

            moikkaaja.MessageReceived(new FriendChatMessage("Hi", 123));
            Assert.IsTrue(Tracker.HasCalled("SendChatMessage"));

        }

        [Test()]
        public void TestUrlInfo()
        {
            UrlInfo urlInfo = new UrlInfo(Context);

            String[] invalid = { 
                "wwwgithubcom", 
                "https://www", 
                "www.github",
                "www.github.com" 
            };
            String[] valid = { 
                "http://www.github.com", 
                "https://www.github.com",
                "http://www.github.com:4546",
                "http://github.com", 
                "http://github.com/attemoi/" 
            };

            foreach (var url in invalid)
            {
                urlInfo.MessageReceived(new FriendChatMessage(url, 123));
                Assert.IsFalse(Tracker.HasCalled("InvokeAddins"), url);
                Tracker.Reset();
            }

            foreach (var url in valid)
            {
                urlInfo.MessageReceived(new FriendChatMessage(url, 123));
                Assert.IsTrue(Tracker.HasCalled("InvokeAddins"), url);
                Tracker.Reset();
            }
               

        }

    }


}

