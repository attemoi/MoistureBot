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
			var args = commandParts.Skip(1).ToList(); // the arguments is after the command

			switch(commandName)
			{
			// Create command based on CommandName (and maybe arguments)
			case "exit": return new ExitCommand();
				// .
				// .
				// .
				// .
			}

			return null;
		}
	}
}

