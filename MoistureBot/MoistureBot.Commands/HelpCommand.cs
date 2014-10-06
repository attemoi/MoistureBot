using System;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;

namespace MoistureBot.Commands
{
	public class HelpCommand : ICommand
	{

		private static readonly log4net.ILog log = log4net.LogManager.GetLogger
			(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
				"Print help for a command.", 
				"help <command_name>",
				options);
		}

		public bool Execute(IMoistureBot bot)
		{

			log.Debug ("Executing command...");

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
				case "status":
					new StatusCommand ().WriteHelp ();
					break;
				case "favorites":
					new FavoritesCommand ().WriteHelp ();
					break;
				case "set":
					new SetCommand ().WriteHelp ();
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
				Console.WriteLine ("  favorites [OPTIONS]+                - manage favorite rooms and users");
				Console.WriteLine ("  set [OPTIONS]+                      - manage bot variables");
				Console.WriteLine ("  connect [<username> [<password>]]   - connect and log on to Steam");
				Console.WriteLine ("  disconnect                          - disconnect from steam");
				Console.WriteLine ("  join <chatid>                       - join chat room");
				Console.WriteLine ("  msg <type> <chatid> <message>       - send message to user or room");
				Console.WriteLine ("  exit                                - disconnect Steam and exit program");

			}
			return false;
		}

	}
}

