using System;
using Mono.Addins;
using MoistureBot.ExtensionPoints;
using System.Text.RegularExpressions;
using MoistureBot.Steam;

using UrlInfo;

[assembly:Addin("UrlInfo", "1.0")]
[assembly:AddinRoot("UrlInfo", "1.0")]
[assembly:AddinDependency("MoistureBot", "1.0")]
[assembly:AddinAuthor("Atte Moisio")]
[assembly:AddinDescription("Logs received messages to sqlite.")]
[assembly:AddinName("UrlInfo")]
[assembly:AddinUrl("")]
namespace MoistureBot
{

	[Extension(typeof(IReceiveFriendChatMessages))]
	[Extension(typeof(IReceiveGroupChatMessages))]
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
				Bot.SendChatMessage(reply,message.ChatterId);
				
		}

		public void MessageReceived(GroupChatMessage message)
		{
		
			var reply = CreateReply(message.Message);
			if (!String.IsNullOrEmpty(reply))
				Bot.SendChatRoomMessage(reply,message.ChatId);

		}

		public string CreateReply(string message) {

			// Check if message contains urls
			MatchCollection matches = Regex.Matches(message,URL_REGEX);
			if (matches == null)
				return null;

			foreach (Match m in matches)
			{
				Uri uri = new Uri(m.Value);

				foreach (IReceiveUrl addin in AddinManager.GetExtensionObjects<IReceiveUrl> ())
				{
					try
					{
						return addin.ReplyToUrl(uri);
					}
					catch(Exception e)
					{
						Logger.Error("Error in UrlInfo extension.",e);
					}
				}

			}

			return null;
				
		}
	}
}

