using System;

namespace moisturebot.commands
{
	public class ExitCommand : ICommand
	{
		public bool Execute(IMoistureBot bot)
		{
			bot.Disconnect ();
			return true;
		}
	}
}

