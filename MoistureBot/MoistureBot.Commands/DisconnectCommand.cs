using System;
using Mono.Options;

namespace MoistureBot.Commands
{
	public class DisconnectCommand : ICommand
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger
			(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
			log.Debug ("Executing command...");

			options.Parse (Args);

			if (help) {
				WriteHelp ();
				return false;
			}
				
			if (!bot.IsConnected ()) {
				log.Info ("Bot already offline.");
				return false;
			}

			log.Info("Disconnecting from steam...");
			bot.Disconnect ();
			return false;
		}
	}
}

