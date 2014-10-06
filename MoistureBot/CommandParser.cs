using System;
using MoistureBot.Commands;
using System.Linq;

namespace MoistureBot
{
	public static class CommandParser
	{

		private static readonly log4net.ILog log = log4net.LogManager.GetLogger
			(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	
		public static ICommand Parse(string commandString) {

			log.Debug("Parsing command '" + commandString + "'");

			// Parse your string and create Command object
			var commandParts = commandString.Split(' ').ToList();
			var commandName = commandParts[0];
			var args = commandParts.Skip(1).ToArray(); // the arguments is after the command

			ICommand command = null;
			switch (commandName) {
			case "exit":
				command = new ExitCommand ();
				break;
			case "quit":
				command = new ExitCommand ();
				break;
			case "disconnect":
				command = new DisconnectCommand ();
				break;
			case "help":
				command = new HelpCommand ();
				break;
			case "connect": 
				command = new ConnectCommand ();
				break;
			case "join":
				command = new JoinChatCommand ();
				break;
			case "msg":
				command = new SendMessageCommand ();
				break;
			case "status":
				command = new StatusCommand ();
				break;
			case "favorites":
				command = new FavoritesCommand ();
				break;
			}

			if (command != null) {
				command.Args = args;
			} else {
				log.Debug ("Failed to parse command: Command not found");
			}
				

			return command;

		}
	}
}

