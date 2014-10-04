﻿using System;
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

			switch (commandName) {
			case "exit":
				return new ExitCommand ();
			case "quit":
				return new ExitCommand ();
			case "disconnect":
				return new DisconnectCommand ();
			}

			ICommandWithArgs command;
			switch (commandName) {
			case "help":
				command = new HelpCommand ();
				command.Args = args;
				return command;
			case "connect": 
				command = new ConnectCommand ();
				command.Args = args;
				return command;
			case "join":
				command = new JoinChatCommand ();
				command.Args = args;
				return command;
			case "msg":
				command = new SendMessageCommand ();
				command.Args = args;
				return command;
			}

			return null;
		}
	}
}

