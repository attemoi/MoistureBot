using System;
using Mono.Options;

namespace moisturebot.commands
{
	public class DisconnectCommand : ICommand
	{
		public string[] Args { get; set; }

		private OptionSet options;

		private bool help;

		public DisconnectCommand() {
			options = new OptionSet () {
				{ "h|help", "show this message", 
					h => help = h != null }
			};
		}

		public void WriteHelp() {
			ConsoleUtils.WriteHelp(
				"Sign out and disconnect from Steam.", 
				"disconnect",
				options);
		}

		public bool Execute(IMoistureBot bot)
		{
			options.Parse (Args);

			if (help) {
				WriteHelp ();
				return false;
			}
				
			if (!bot.IsConnected ()) {
				Console.WriteLine ("Bot already offline.");
				return false;
			}

			bot.Disconnect ();
			bot.BlockUntilDisconnected ();
			return false;
		}
	}
}

