using System;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;

namespace moisturebot.commands
{
	public class SendMessageCommand : ICommand
	{

		public string[] Args { get; set; }

		public Boolean help;
		public Boolean friend;
		public Boolean room;

		private OptionSet options;

		public SendMessageCommand() {
			options = new OptionSet () {
				{ "h|help", "show this message", 
					h => help = h != null },
				{ "f|friend", "send message to friend", 
					f => friend = f != null },
				{ "r|room", "send message to room", 
					r => room = r != null }
			};
		}

		public void WriteHelp() {
			ConsoleUtils.WriteHelp(
				"send a chat message", 
				"msg [OPTIONS]+ <chat_id> <message>",
				options);
		}

		public bool Execute (IMoistureBot bot)
		{
		
			List<string> extra = options.Parse(Args);

			if (help || extra.Count < 2 || (!friend && !room)) {
				WriteHelp ();
				return false;
			}

			if (!bot.IsConnected ()) {
				Console.WriteLine ("Not connected to Steam.");
				return false;
			}

			string chatId = extra.ElementAt (0);
			// get rest
			string message = string.Join (" ", extra.Skip (1)).Trim ('\"');

			ulong id;
			try {
				id = UInt64.Parse(chatId);
			} catch {
				Console.WriteLine ("Invalid chat id!");
				return false;
			}

			if (friend) {
				Console.WriteLine ("Sending chat message to friend '{0}'...", id);
				bot.SendChatMessage (message, id);
			}

			if (room) {
				Console.WriteLine ("Sending chat message to room '{0}'...", id);
				bot.SendChatMessage (message, id);
			}

			return false;
		}

	}
}

