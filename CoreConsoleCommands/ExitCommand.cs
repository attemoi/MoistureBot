using System;
using System.Threading;
using Mono.Options;
using MoistureBot.Config;
using Mono.Addins;
using System.Linq;
using System.Collections.Generic;

namespace MoistureBot
{

	[ConsoleCommand(
		Name = "exit",
		Description = "Disconnect from Steam and exit program.",
		ShortDescription = "Disconnect from Steam and exit program.",
		ShortUsage = "exit",
		Usage = "exit"
	)]
	public class ExitCommand : IConsoleCommand
	{
	
		private ILogger Logger = MoistureBotComponentProvider.GetLogger();
		private IMoistureBot Bot = MoistureBotComponentProvider.GetBot();

		public OptionSet Options {
			get { return new OptionSet (); }
		}

		public bool Execute(string[] args)
		{
			Logger.Info ("Executing command...");

			List<string> extra = Options.Parse (args);

			if (extra.Count > 0)
				Console.Write(ConsoleMessage.ExtraParametersNotAllowed("exit"));

			Console.WriteLine("Disconnecting from Steam...");
			Bot.Disconnect ();
			Bot.Terminate ();
			Console.WriteLine("Exiting program...");
			return true;
		}
	}
}

