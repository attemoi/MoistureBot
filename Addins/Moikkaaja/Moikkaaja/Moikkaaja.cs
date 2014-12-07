using System;
using System.Text.RegularExpressions;
using System.Linq;
using MoistureBot.ExtensionPoints;
using MoistureBot.Steam;
using Mono.Addins;

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
			string[] greetings = {
				"Moikka taas", "Moikkelis", "Moi kaikki", "Moikkamoi", 
				"Moikka", "Moips", "Moiks", "Moik", "Moi", "Terve", 
				"Hello", "Tsau", "Heippa", "Heips", "Hei",
				"Hoi", "Hola", "Iltaa", "Päivää", "Huomenta", "Moro",
				"Hyvää yötä", "Hyvää päivää", "Hyvää huomenta", "Hyvää iltaa", 
				"Näkemiin", "Hei hei", "Moikka moi"
			};

			Regex rgx = new Regex("[^a-zäöA-ZÄÖ0-9 -]");
			string strippedMsg = rgx.Replace(message.Message,"").ToLower();
			string msg = Array.Find(greetings,t => t.Equals(strippedMsg,StringComparison.InvariantCultureIgnoreCase));

			if (msg != null && !msg.Equals(""))
			{
				Logger.Info("Moikkaaja: Greeting received, replying");
				msg += " " + Bot.GetPersonaName(message.ChatterId) + "!";
			}

			return msg;
		}
	}
}
