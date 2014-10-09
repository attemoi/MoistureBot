using System;
using Mono.Addins;
using System.Text.RegularExpressions;
using System.Linq;

[assembly:Addin("Moikkaaja", "1.0")]
[assembly:AddinDependency ("MoistureBot", "1.0")]
[assembly:AddinAuthor ("Atte Moisio")]
[assembly:AddinDescription ("Responds to a number of finnish greetings.")]
[assembly:AddinName("Moikkaaja")]
[assembly:AddinUrl("") ]
namespace MoistureBot
{

	[Extension (typeof(IChatRoomAddin))]
	[Extension (typeof(IChatFriendAddin))]
	public class Moikkaaja: IChatRoomAddin, IChatFriendAddin
	{
	
		private IMoistureBot Bot = MoistureBotComponentProvider.GetBot();
		private ILogger Logger = MoistureBotComponentProvider.GetLogger();

		public void MessageReceived (ChatRoomMessage message)
		{
			var reply = CreateReply (message);
			if (!String.IsNullOrEmpty(reply))
				Bot.SendChatRoomMessage (reply, message.ChatId);
		}

		public void MessageReceived (ChatMessage message)
		{
			var reply = CreateReply (message);
			if (!String.IsNullOrEmpty(reply))
				Bot.SendChatMessage (reply, message.ChatterId);
		}

		string CreateReply (ChatMessage message)
		{
			string[] greetings = {
				"Moikka taas", "Moikkelis", "Moi kaikki", "Moikkamoi", 
				"Moikka", "Moips", "Moiks", "Moik", "Moi", "Terve", 
				"Hello", "Tsau", "Heippa", "Heips", "Hei",
				"Hoi", "Hola", "Iltaa", "Päivää", "Huomenta", "Moro",
				"Hyvää yötä", "Hyvää päivää", "Hyvää huomenta", "Hyvää iltaa", 
				"Näkemiin", "Hei hei", "Moikka moi"
			};

			Regex rgx = new Regex("[^a-zA-Z0-9 -]");
			string strippedMsg = rgx.Replace(message.Message, "").ToLower();
			string msg = Array.Find(greetings, t => t.Equals(strippedMsg, StringComparison.InvariantCultureIgnoreCase));

			if (msg != null && !msg.Equals("") ){
				Logger.Info("Moikkaaja: Greeting received, replying");
				msg += " " + Bot.GetUserName(message.ChatterId) + "!";
			}

			return msg;
		}
	}
}