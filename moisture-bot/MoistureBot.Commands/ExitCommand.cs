using System;
using System.Threading;
using Mono.Options;

namespace moisturebot.commands
{
	public class ExitCommand : ICommand
	{

		public string[] Args { get; set; }

		private OptionSet options;

		private bool help;

		public ExitCommand() {
			options = new OptionSet () {
				{ "h|help", "show this message", 
					h => help = h != null }
			};
		}

		public void WriteHelp() {
			ConsoleUtils.WriteHelp(
				"shut down bot and exit program", 
				"exit",
				options);
		}

		public bool Execute(IMoistureBot bot)
		{
			options.Parse (Args);

			if (help) {
				WriteHelp ();
				return false;
			}

			bot.Disconnect ();
			// TODO: remove sleep, wait for disconnect
			Thread.Sleep (1000);
			return true;
		}
	}
}

