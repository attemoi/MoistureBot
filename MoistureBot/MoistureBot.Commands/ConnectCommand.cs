﻿using System;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;

namespace MoistureBot.Commands
{
	public class ConnectCommand : ICommand
	{

		private static readonly log4net.ILog log = log4net.LogManager.GetLogger
			(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public string[] Args { get; set; }

		private OptionSet options;

		private bool help;

		public ConnectCommand() {
			options = new OptionSet () {
				{ "h|help", "show this message", 
					h => help = h != null },
			};
		}

		public void WriteHelp() {
			ConsoleUtils.WriteHelp(
				"Connect and sign in to Steam.", 
				"connect [<username> [<password>]]",
				options);
		}

		public bool Execute (IMoistureBot bot)
		{

			log.Debug ("Executing command...");
		
			List<string> extra = options.Parse (Args);

			if (help || extra.Count > 2) {
				WriteHelp ();
				return false;
			}

			string user = null;
			string pass = null;

			if (extra.Count > 0)
				user = extra.ElementAt(0);
			if (extra.Count > 1)
				pass = extra.ElementAt(1);

			if (String.IsNullOrEmpty (user)) {
				Console.Write ("username:");
				user = Console.ReadLine ();
			}
			if (String.IsNullOrEmpty (pass)) {
				Console.Write ("password:");
				pass = ConsoleUtils.ReadPassword ();
			}

			log.Info("Logging in as " + user + "...");
			log.Info("Type 'status' to see bot status.");
			bot.Connect(user, pass);

			return false;
		}

	}
}
