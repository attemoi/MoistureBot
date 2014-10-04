﻿using System;

namespace moisturebot.commands
{
	public class ExitCommand : ICommand
	{
		public void WriteHelp() {
			ConsoleUtils.WriteHelp("shut down bot and exit program");
		}

		public bool Execute(IMoistureBot bot)
		{
			bot.Disconnect ();
			return true;
		}
	}
}

