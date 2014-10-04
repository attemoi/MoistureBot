using System;

namespace moisturebot.commands
{
	public class ExitCommand : ICommand
	{

		public static string Name = "exit";



		public bool Execute(IMoistureBot bot)
		{
			bot.Disconnect ();
			return true;
		}
	}
}

