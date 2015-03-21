using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MoistureBot.ExtensionPoints;
using MoistureBot.Steam;
using Mono.Addins;
using System.Xml;
using System.Xml.Linq;

[assembly:Addin("Moikkaaja", "1.0")]
[assembly:AddinDependency("MoistureBot", "1.0")]
[assembly:AddinAuthor("Atte Moisio")]
[assembly:AddinDescription("Greeter addon, responds to greetings defined in Greetings.xml")]
[assembly:AddinName("Moikkaaja")]
[assembly:AddinUrl("")]
namespace MoistureBot
{

	[Extension(typeof(IReceiveFriendChatMessages))]
	[Extension(typeof(IReceiveGroupChatMessages))]
	public class Moikkaaja: IReceiveFriendChatMessages, IReceiveGroupChatMessages
	{

		private IMoistureBot Bot = MoistureBotComponentProvider.GetBot();
		private ILogger Logger = MoistureBotComponentProvider.GetLogger();

		Dictionary<String, String> replyDict;

		public Moikkaaja() {

			// Convert greetings to a dictionary with a stripped lowercase string as the key.
			replyDict = readGreetings()
				.Select((str) => new { 
					Value = str, 
					Key = stripMessage(str)
				})
				.ToDictionary(
					x => x.Key,
					x => x.Value
				);
		}

		public void MessageReceived(GroupChatMessage message)
		{
			var reply = CreateReply(message);
			if (!String.IsNullOrEmpty(reply))
				Bot.SendChatRoomMessage(reply,message.ChatId);
		}

		public void MessageReceived(FriendChatMessage message)
		{
			var reply = CreateReply(message);
			if (!String.IsNullOrEmpty(reply))
				Bot.SendChatMessage(reply,message.ChatterId);
		}

		string CreateReply(FriendChatMessage message)
		{
			string key = stripMessage(message.Message);
			string reply;
			if (replyDict.TryGetValue(key, out reply))
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

		private IEnumerable<String> readGreetings() {

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

		private string stripMessage(String value) {
			return new Regex("[^a-zäöA-ZÄÖ0-9\\s-]").Replace(value,"").ToLower();
		}
	}
}
