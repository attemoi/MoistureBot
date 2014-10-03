using System;
using Mono.Addins;
using moisturebot.lib;
using System.Text.RegularExpressions;

[assembly:Addin]
[assembly:AddinDependency ("moisturebot", "1.0")]

namespace moisturebot
{

	[Extension]
	public class Moikkaaja: IChatCommand
	{
		public string ReplyToMessage (ChatMessage message)
		{
			string[] greetings = {
				"Moi", "Moikka", "Terve", "Hello", "Tsau",
				"Hei", "Moi kaikki", "Moikka taas", "Moikkamoi",
				"Heippa", "Heips", "Moips", "Moik", "Hoi",
				"Hola", "Iltaa", "Päivää", "Huomenta",
				"Moi Moisture-bot"
			};

			Regex rgx = new Regex("[^a-zA-Z0-9 -]");
			string originalMsg = message.Message;
			string strippedMsg = rgx.Replace(message.Message, "").ToLower();
			// Find message index, ignore case
			int msgIndex = Array.FindIndex(greetings, t => t.IndexOf(strippedMsg, StringComparison.InvariantCultureIgnoreCase) >=0);

			if (!strippedMsg.Equals("") && msgIndex > -1) {
				Console.WriteLine ("Greeting received, replying");
				return (greetings [msgIndex] + " " + message.Sender + "!");
			}

			if (strippedMsg.StartsWith ("miksi") || strippedMsg.StartsWith ("miksei")) {
				return "En tiedä :(";
			}

			return null;
		}
	}
}