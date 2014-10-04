using System;
using Mono.Options;
using System.Collections.Generic;

namespace moisturebot
{
	public class StatusCommand : ICommand
	{
		public string[] Args { get; set; }

		private OptionSet options;

		private bool help;

		public StatusCommand() {
			options = new OptionSet () {
				{ "h|help", "show this message", 
					h => help = h != null }
			};
		}

		public void WriteHelp() {
			ConsoleUtils.WriteHelp(
				"show bot status", 
				"status",
				options);
		}

		public bool Execute (IMoistureBot bot)
		{

			options.Parse(Args);

			if (help) {
				WriteHelp ();
				return false;
			}
				
			Console.WriteLine ();
			Console.WriteLine ("Connection status: {0}", bot.IsConnected () ? "online" : "offline");
			Console.WriteLine ();

			Console.WriteLine ("Active chatrooms:");
			bot.GetActiveChatRooms().ForEach( id => Console.WriteLine("  " + id));

			return false;
		}
	}
}

