﻿using System.Collections.Generic;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using MoistureBot.Model;

namespace MoistureBot
{

    public class Moikkaaja : IReceiveFriendChatMessages, IReceiveGroupChatMessages
    {
        
        Dictionary<String, String> ReplyDict = new Dictionary<String, String>();

        IMoistureBot Bot;
        ILogger Logger;

        [Provide]
        public Moikkaaja(IContext context)
        {
            this.Bot = context.GetBot();
            this.Logger = context.GetLogger(typeof(Moikkaaja));

            Logger.Info("Reading greetings from xml file.");

            try 
            {
                // Convert greetings to a dictionary with a stripped lowercase string as the key.
                ReplyDict = ReadGreetings()
                    .Select((str) => new { 
                        Value = str, 
                        Key = stripMessage(str)
                    })
                    .GroupBy(e => e.Key) // This is needed to handle duplicates
                    .ToDictionary(
                        x => x.First().Key,
                        x => x.First().Value
                    );
            }
            catch (Exception e)
            {
                Logger.Error("Failed to read greetings.", e);
            }

            Logger.Info("Found " + ReplyDict.Count + " greetings.");

        }

        public void MessageReceived(GroupChatMessage message)
        {
            var reply = CreateReply(message);
            if (!String.IsNullOrEmpty(reply))
                Bot.SendChatRoomMessage(reply, message.ChatId);
        }

        public void MessageReceived(FriendChatMessage message)
        {
            var reply = CreateReply(message);
            if (!String.IsNullOrEmpty(reply))
                Bot.SendChatMessage(reply, message.ChatterId);
        }

        string CreateReply(FriendChatMessage message)
        {
            string key = stripMessage(message.Message);
            string reply;
            if (ReplyDict.TryGetValue(key, out reply))
            {
                Logger.Info("Moikkaaja: Greeting received, replying");
                reply += " " + Bot.GetPersonaName(message.ChatterId) + "!";
                return reply;
            }
            else
            {
                return null;
            }

        }

        private IEnumerable<String> ReadGreetings()
        {

            //     Greetings.xml example data:
            //
            //     <?xml version="1.0" encoding="UTF-8" ?>
            //     <greetings>
            //     
            //         <greeting>Hello</greeting>
            //		   <greeting>Hi</greeting>
            //         <greeting>Hola</greeting>
            //     
            //     </greetings>

            XDocument xdoc = XDocument.Load("addins/Greetings.xml");

            return from greeting in xdoc.Root.Descendants("greeting")
                            select greeting.Value;
        }

        private string stripMessage(String value)
        {
            char c = (char)720; // This odd character is used to wrap emoticons
            var stripped = new Regex("(" + c + ".*" + c + ")").Replace(value, "").ToLower();
            stripped = stripped.Replace(Bot.PersonaName.ToLower(), "");
            return new Regex("[^a-zäöA-ZÄÖ0-9]").Replace(stripped, "");
        }
    }
}
