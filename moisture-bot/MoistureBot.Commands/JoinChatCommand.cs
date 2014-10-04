using System;
using Mono.Options;
using System.Collections.Generic;

namespace moisturebot.commands
{
	public class JoinChatCommand : ICommandWithArgs
	{

		public string[] Args { get; set; }

		public Boolean help;
		public string chatId;

		private OptionSet options;

		public JoinChatCommand() {
			options = new OptionSet () {
				{ "h|help", "show this message and exit", 
					h => help = h != null },
				{ "c=|chat=", "Chat id" ,
					c => chatId = c}
			};
		}

		public void WriteHelp() {
			Console.WriteLine ();
			Console.WriteLine ("Usage: join -chat=<chat_id>");
			Console.WriteLine ();
			Console.WriteLine ("Options:");
		}

		public bool Execute (IMoistureBot bot)
		{

			List<string> Extra = options.Parse(Args);

			if (chatId == null) {
				Console.WriteLine ("Missing required option -c=CHAT_ID");
				return false;
			}

			Console.WriteLine( "Joining chat room '{0}'...", chatId);

			ulong id = 0;
			try {
				id = UInt64.Parse(chatId);
			} catch {
				Console.WriteLine ("Invalid chat id!");
				return false;
			}

			bot.JoinChat(id);

			return false;
		}

	}
}

