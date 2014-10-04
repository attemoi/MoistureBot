using System;
using Mono.Options;
using System.Collections.Generic;

namespace moisturebot.commands
{
	public class SendMessageCommand : ICommandWithArgs
	{

		public string[] Args { get; set; }

		public Boolean help;
		public string chatId;
		public string msg;

		private OptionSet options;

		public SendMessageCommand() {
			options = new OptionSet () {
				{ "h|help", "show this message", 
					h => help = h != null },
				{ "c=|chat=", "Chat id" ,
					c => chatId = c},
				{ "m=|msg=", "Message content" ,
					m => msg = m}
			};
		}

		public void WriteHelp() {
			ConsoleUtils.WriteHelp(
				"send a chat message", 
				"msg -chat=<chat_id>",
				options);
		}

		public bool Execute (IMoistureBot bot)
		{

			List<string> Extra = options.Parse(Args);

			if (help || chatId == null || msg == null) {
				WriteHelp ();
			}

			ulong id = 0;
			try {
				id = UInt64.Parse(chatId);
			} catch {
				Console.WriteLine ("Invalid chat id!");
				return false;
			}

			Console.WriteLine( "Sending chat message '{0}'...", chatId);

			bot.SendChatMessage( msg, id );

			return false;
		}

	}
}

