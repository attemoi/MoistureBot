using System;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;

namespace moisturebot.commands
{
	public class JoinChatCommand : ICommandWithArgs
	{

		public string[] Args { get; set; }

		public Boolean help;

		private OptionSet options;

		public JoinChatCommand() {
			options = new OptionSet () {
				{ "h|help", "show this message", 
					h => help = h != null }
			};
		}

		public void WriteHelp() {
			ConsoleUtils.WriteHelp(
				"join a chat", 
				"join <chat_id>",
				options);
		}

		public bool Execute (IMoistureBot bot)
		{

			List<string> extra = options.Parse(Args);

			string chatId = null;

			if (extra.Count == 0) {
				chatId = extra.First ();
			} else {
				WriteHelp ();
				return false;
			}

			ulong id = 0;
			try {
				id = UInt64.Parse(chatId);
			} catch {
				Console.WriteLine ("Invalid chat id!");
				return false;
			}

			Console.WriteLine( "Joining chat room '{0}'...", chatId);
			bot.JoinChat(id);

			return false;
		}

	}
}

