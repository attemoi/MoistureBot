using System;
using Mono.Addins;
using MoistureBot.ExtensionPoints;
using System.Text.RegularExpressions;
using MoistureBot.Steam;

namespace MoistureBot
{

    public class UrlInfo : IReceiveFriendChatMessages, IReceiveGroupChatMessages
    {

        private IMoistureBot Bot = MoistureBotComponentProvider.GetBot();
        private ILogger Logger = MoistureBotComponentProvider.GetLogger();
        private IConfig Config = MoistureBotComponentProvider.GetConfig();

        const string URL_REGEX = @"\b(?:https?://|www\.)\S+\b";

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
                Uri uri = new Uri(m.Value);

                foreach (IReceiveUrl addin in AddinManager.GetExtensionObjects<IReceiveUrl> ("MoistureBot/UrlInfo/IReceiveUrl"))
                {
                    try
                    {
                        return addin.ReplyToUrl(uri);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Error in UrlInfo extension.", e);
                    }
                }

            }

            return null;
				
        }
    }
}

