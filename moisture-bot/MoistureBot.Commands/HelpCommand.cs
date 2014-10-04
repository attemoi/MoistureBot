using System;
using Mono.Options;
using System.Collections.Generic;

namespace moisturebot.commands
{
	public class HelpCommand : ICommandWithArgs
	{
		public string[] Args { get; set; }

		public Boolean help;
		public string command;

		private OptionSet options;

		public HelpCommand() {
			options = new OptionSet () {
				{ "h|help", "show this message", 
					h => help = h != null },
				{ "c=|command=", "command name" ,
					c => command = c}
			};
		}

		public void WriteHelp() {
			ConsoleUtils.WriteHelp(
				"print help for a command.", 
				"help command=<command_name>",
				options);
		}

		public bool Execute(IMoistureBot bot)
		{

			List<string> extra = options.Parse (Args);

			if (help) {
				Console.WriteLine ();
				Console.WriteLine ("Usage: help -command=<command_name>");
				options.WriteOptionDescriptions (Console.Out);
			} else if (command != null) {
				switch (command) {
				case "help":
					new HelpCommand ().WriteHelp ();
					break;
				case "connect":
					new ConnectCommand ().WriteHelp ();
					break;
				case "disconnect":
					new DisconnectCommand ().WriteHelp ();
					break;
				case "join":
					new JoinChatCommand ().WriteHelp ();
					break;
				default:
					if (String.IsNullOrWhiteSpace (command));
					Console.WriteLine ("Unknown command: '{0}'", command);
					break;
				}
			} else {
				Console.WriteLine ();
				Console.WriteLine ("Available commands: ");
				Console.WriteLine ();
				Console.WriteLine ("  help [OPTIONS]+        - show this message");
				Console.WriteLine ("  connect [OPTIONS]+     - connect to steam and log in");
				Console.WriteLine ("  disconnect             - disconnect from steam");
				Console.WriteLine ("  join [OPTIONS]+        - join chat");
				Console.WriteLine ();
				Console.WriteLine ("Type 'help -command <command_name>' to get help for a specific command");
			}
			return false;
		}

	}
}

