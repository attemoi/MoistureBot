using System;
using Mono.Options;
using System.Collections.Generic;

namespace moisturebot.commands
{
	public class ConnectCommand : ICommand
	{

		public string[] Args { get; set; }

		private OptionSet options;

		private bool help;
		private string user;
		private string pass;

		public ConnectCommand() {
			options = new OptionSet () {
				{ "h|help", "show this message", 
					h => help = h != null },
				{ "u=|username=", "login username" ,
					u => user = u},
				{ "p=|password=", "login password" ,
					p => pass = p}
			};
		}

		public void WriteHelp() {
			ConsoleUtils.WriteHelp(
				"connect and sign in to Steam", 
				"connect [OPTIONS]+",
				options);
		}

		public bool Execute (IMoistureBot bot)
		{
		
			options.Parse (Args);

			if (help) {
				WriteHelp ();
				return false;
			}

			if (String.IsNullOrEmpty (user) || String.IsNullOrEmpty (pass)) {
				Console.Write ("username:");
				user = Console.ReadLine ();
				Console.Write ("password:");
				pass = ConsoleUtils.ReadPassword ();
			}

			bot.Connect(user, pass);

			return false;
		}

	}
}

