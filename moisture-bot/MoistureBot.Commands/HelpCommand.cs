using System;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;

namespace moisturebot.commands
{
	public class HelpCommand : ICommand
	{
		public string[] Args { get; set; }

		public Boolean help;
		public string command;

		private OptionSet options;

		public HelpCommand() {
			options = new OptionSet () {
				{ "h|help", "show this message", 
					h => help = h != null }
			};
		}

		public void WriteHelp() {
			ConsoleUtils.WriteHelp(
				"print help for a command.", 
				"help <command_name>",
				options);
		}

		public bool Execute(IMoistureBot bot)
		{

			List<string> extra = options.Parse (Args);

			if (help) {
				Console.WriteLine ();
				Console.WriteLine ("Usage: help <command_name>");
				options.WriteOptionDescriptions (Console.Out);
				return false;
			}
			if (extra.Count > 0 ) {
				command = extra.First ();

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
				case "msg":
					new SendMessageCommand ().WriteHelp ();
					break;
				case "exit":
					new ExitCommand ().WriteHelp ();
					break;
				default:
					if (String.IsNullOrWhiteSpace (command))
						Console.WriteLine ("Unknown command: '{0}'", command);
					break;
				}

			} else {

				Console.WriteLine ();
				Console.WriteLine ("Available commands: ");
				Console.WriteLine ();
				Console.WriteLine ("  help <command>                      - show help for a specific command");
				Console.WriteLine ("  status                              - show bot status");
				Console.WriteLine ("  connect [OPTIONS]+                  - connect to steam and log in");
				Console.WriteLine ("  disconnect                          - disconnect from steam");
				Console.WriteLine ("  join <chatid>                       - join chat");
				Console.WriteLine ("  msg [OPTIONS]+ <chatid> <message>   - disconnect Steam and exit program");
				Console.WriteLine ("  exit <chatid>                       - disconnect Steam and exit program");
				Console.WriteLine ();
				Console.WriteLine ("Type 'help <command>' to get help for a specific command.");

			}
			return false;
		}

	}
}

