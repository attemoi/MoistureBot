﻿using System;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;
using Mono.Addins;

namespace MoistureBot
{

	[ConsoleCommand(
		Name = "connect",
		ShortDescription = "Connect and sign in to Steam.",
		ShortUsage = "connect [<username> [<password>]]",
		Description = "Connect and sign in to Steam.",
		Usage = "connect [<username> [<password>]]"
	)]
	public class ConnectCommand : IConsoleCommand
	{
	
		private IMoistureBot Bot = MoistureBotComponentProvider.GetBot();
		private ILogger Logger = MoistureBotComponentProvider.GetLogger();

		public OptionSet Options {
			get {
				return new OptionSet ();
			}
		}

		public bool Execute (string[] args)
		{

			Logger.Info ("Executing command...");
		
			List<string> extra = Options.Parse (args);

			string user = null;
			string pass = null;

			if (extra.Count > 0)
				user = extra.ElementAt(0);
			if (extra.Count > 1)
				pass = extra.ElementAt(1);

			if (extra.Count > 2)
				Console.WriteLine (ConsoleMessage.InvalidNumberOfParameters("connect"));

			if (String.IsNullOrEmpty (user)) {
				Console.Write ("username:");
				user = Console.ReadLine ();
			}
			if (String.IsNullOrEmpty (pass)) {
				Console.Write ("password:");
				pass = ConsoleUtils.ReadPassword ();
			}

			Console.WriteLine("Logging in as " + user + "...");
			Console.WriteLine(ConsoleMessage.SEE_STATUS);
			Bot.Connect(user, pass);

			return false;
		}

	}
}

