﻿using System;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;
using MoistureBot.Config;
using Mono.Addins;

namespace MoistureBot
{

	[ConsoleCommandAttribute( 
		Name = "help",
		ShortDescription = "Print help for a command.",
		ShortUsage = "help <command>",
		Description = "Print help for a command.",
		Usage = "help <command>"
	)]
	public class HelpCommand : ICommand
	{

		private IMoistureBot Bot = MoistureBotComponentProvider.GetBot ();
		private ILogger Logger = MoistureBotComponentProvider.GetLogger();

		public string command;

		public OptionSet Options {
			get { return new OptionSet (); }
		}

		public bool Execute(string[] args)
		{

			Logger.Info ("Executing command...");

			List<string> extra = Options.Parse (args);

			if (extra.Count > 0) {
				command = extra.First ();

				ExtensionNodeList commands = AddinManager.GetExtensionNodes (typeof(ICommand));

				foreach (TypeExtensionNode<ConsoleCommandAttribute> node in commands) {
					if (command.Equals (node.Data.Name)) {
						Console.WriteLine ();
						Console.WriteLine ("Description:");
						Console.WriteLine ();
						// TODO Wrap to 80 chars
						Console.WriteLine ("  {0}", node.Data.Description);
						Console.WriteLine ();
						Console.WriteLine ("Usage:");
						Console.WriteLine ();
						Console.WriteLine ("  {0}", node.Data.Usage);

						return false;

					}
				}

				Console.WriteLine ("Unknown command: '{0}'", command);

			} else {

				Console.WriteLine ();
				Console.WriteLine ("Available commands: ");
				Console.WriteLine ();

				ExtensionNodeList commands = AddinManager.GetExtensionNodes (typeof(ICommand));

				foreach (TypeExtensionNode<ConsoleCommandAttribute> node in commands) {
					Console.WriteLine ("  {0} - {1}", node.Data.ShortUsage.PadRight(35), node.Data.ShortDescription.PadRight(35));
				}

			}
			return false;
		}

	}
}

