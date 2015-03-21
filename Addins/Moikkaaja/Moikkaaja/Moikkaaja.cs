using System;
using System.Text.RegularExpressions;
using System.Linq;
using MoistureBot.ExtensionPoints;
using MoistureBot.Steam;
using Mono.Addins;
using System.Collections.Generic;

[assembly:Addin("Moikkaaja", "1.0")]
[assembly:AddinDependency("MoistureBot", "1.0")]
[assembly:AddinAuthor("Atte Moisio")]
[assembly:AddinDescription("Responds to a number of finnish greetings.")]
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

			string[] greetings = {
				"Moikka taas", "Moikkelis", "Moi kaikki", "Moikkamoi", 
				"Moikka", "Moips", "Moiks", "Moik", "Moi", "Terve", 
				"Hello", "Tsau", "Heippa", "Heips", "Hei",
				"Hoi", "Hola", "Iltaa", "Päivää", "Huomenta", "Moro",
				"Hyvää yötä", "Hyvää päivää", "Hyvää huomenta", "Hyvää iltaa", 
				"Näkemiin", "Hei hei", "Moikka moi"
			};

			// Convert the array to a dictionary with a stripped lowercase string as the key.
			replyDict = greetings
				.Select((str) => new { 
					Value = str, 
					Key = stripMessage(str)
				})
				.ToDictionary(
					x => x.Key,
					x => x.Value
				);
		}

		private string stripMessage(String value) {
			return new Regex("[^a-zäöA-ZÄÖ0-9\\s-]").Replace(value,"").ToLower();
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
	}
}
