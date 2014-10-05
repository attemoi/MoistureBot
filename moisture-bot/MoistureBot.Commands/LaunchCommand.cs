using System;
using Mono.Options;
using System.Collections.Generic;

namespace moisturebot.commands
{
	public class LaunchCommand : ICommand
	{

		public string[] Args { get; set; }

		private OptionSet options;

		private bool help;
		private bool connect;
		private string user;
		private string pass;

		public LaunchCommand() {
			options = new OptionSet () {
				{ "h|help", "show this message", 
					h => help = h != null },
				{ "c|connect", "Connect on start and join favourite rooms" ,
					c => connect = c != null},
				{ "u=|username=", "login username" ,
					u => user = u},
				{ "p=|password=", "login password" ,
					p => pass = p}
			};
		}

		public void WriteHelp() {
			ConsoleUtils.WriteHelp (
				"Spectacular Steam chat bot", 
				"moisture-bot [OPTIONS]+" + Environment.NewLine +
				"  moisture-bot -connect -u <username> -p <password>", 
				options);
		}

		public bool Execute (IMoistureBot bot)
		{
		
			options.Parse (Args);

			if (help) {
				WriteHelp ();
				return true;
			}
				
			if (connect) {
				if (!String.IsNullOrEmpty (user) && !String.IsNullOrEmpty(pass)) {
					bot.Connect (user, pass);
					bot.BlockUntilConnected ();
					return false;
				} else {
					Console.Write ("Missing user information for autoconnect.");
					return true;
				}
			}

			return false;
		}

	}
}

