using System;
using System.Threading;
using Mono.Options;

namespace MoistureBot.Commands
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
				"Disconnect bot from Steam and exit program.", 
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
			bot.BlockUntilDisconnected ();

			return true;
		}
	}
}

