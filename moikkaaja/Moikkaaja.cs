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
		private IMoistureBot Bot { get; set; }

		public void Initialize (IMoistureBot bot)
		{
			this.Bot = bot;
		}

		public void MessageReceived (ChatMessage message)
		{
			string[] greetings = {
				"Moikka taas", "Moi kaikki", "Moikkamoi", "Moikka", "Moips", "Moiks", "Moik", "Moi", 
				"Terve", "Hello", "Tsau", "Heippa", "Heips", "Hei",
				"Hoi", "Hola", "Iltaa", "Päivää", "Huomenta", "Moro"
			};

			Regex rgx = new Regex("[^a-zA-Z0-9 -]");
			string strippedMsg = rgx.Replace(message.Message, "").ToLower();
			string msg = Array.Find(greetings, t => t.Equals(strippedMsg, StringComparison.InvariantCultureIgnoreCase));

			if (msg != null && !msg.Equals("") ){
				Console.WriteLine ("Greeting received, replying");
				msg += " " + message.Sender + "!";

				Bot.SendChatRoomMessage (msg);
			}
		}
	}
}