using System;
using moisturebot.commands;
using System.Linq;

namespace moisturebot
{
	public static class CommandParser
	{
	
		public static ICommand Parse(string commandString) {

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
			}

			if (command != null)
				command.Args = args;

			return command;

		}
	}
}

