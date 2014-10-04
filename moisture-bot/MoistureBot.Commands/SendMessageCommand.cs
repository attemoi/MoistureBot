using System;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;

namespace moisturebot.commands
{
	public class SendMessageCommand : ICommandWithArgs
	{

		public string[] Args { get; set; }

		public Boolean help;

		private OptionSet options;

		public SendMessageCommand() {
			options = new OptionSet () {
				{ "h|help", "show this message", 
					h => help = h != null }
			};
		}

		public void WriteHelp() {
			ConsoleUtils.WriteHelp(
				"send a chat message", 
				"msg <chat_id> <message>",
				options);
		}

		public bool Execute (IMoistureBot bot)
		{

			List<string> extra = options.Parse(Args);

			if (help || extra.Count != 2) {
				WriteHelp ();
			}

			string chatId = extra.ElementAt (0);
			string message = extra.ElementAt (1);

			ulong id;
			try {
				id = UInt64.Parse(chatId);
			} catch {
				Console.WriteLine ("Invalid chat id!");
				return false;
			}

			Console.WriteLine( "Sending chat message '{0}'...", chatId);

			bot.SendChatMessage( message, id );

			return false;
		}

	}
}

