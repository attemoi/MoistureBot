using System;
using Mono.Addins;
using MoistureBot;
using System.Text.RegularExpressions;
using MoistureBot.Model;

namespace MoistureBot
{

    public class UrlInfo : IReceiveFriendChatMessages, IReceiveGroupChatMessages
    {

        IMoistureBot Bot;
        IContext Context;

        [Provide]
        public UrlInfo(IContext context)
        {
            this.Context = context;
            this.Bot = context.GetBot();
        }
        const string URL_REGEX = @"\b(?:https?://|www\.)\S+\.\S+\b";

        public void MessageReceived(FriendChatMessage message)
        {
            var reply = CreateReply(message.Message);
            if (!String.IsNullOrEmpty(reply))
                Bot.SendChatMessage(reply, message.ChatterId);
				
        }

        public void MessageReceived(GroupChatMessage message)
        {
		
            var reply = CreateReply(message.Message);
            if (!String.IsNullOrEmpty(reply))
                Bot.SendChatRoomMessage(reply, message.ChatId);

        }

        public string CreateReply(string message)
        {

            // Check if message contains urls
            MatchCollection matches = Regex.Matches(message, URL_REGEX);
            if (matches == null)
                return null;

            foreach (Match m in matches)
            {
                try 
                {
                    Uri uri = new Uri(m.Value);             
                    string response = null;
                    Context.InvokeAddins<IReceiveUrl>("MoistureBot/UrlInfo/IReceiveUrl", addin => response = addin.ReplyToUrl(uri));
                    return response;
                } 
                catch (UriFormatException) 
                { 
                    return null; 
                }
            }

            return null;
				
        }
    }
}

