using System;
using Mono.Addins;
using System.Text.RegularExpressions;

[assembly:Addin]
[assembly:AddinDependency ("moisturebot", "1.0")]

namespace moisturebot
{

	[Extension]
	public class Moikkaaja: IGroupChatAddin
	{

		public override void MessageReceived (ChatMessage message)
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
				Console.WriteLine ("moikkaaja: Greeting received, replying");
				msg += " " + message.Sender + "!";

				Bot.SendChatRoomMessage (msg, message.ChatRoomId);
			}
		}
	}
}