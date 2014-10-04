using System;

namespace moisturebot.commands
{
	public class DisconnectCommand : ICommand
	{
		public void WriteHelp() {
			ConsoleUtils.WriteHelp("sign out and disconnect from Steam");
		}

		public bool Execute(IMoistureBot bot)
		{
			bot.Disconnect ();
			return false;
		}
	}
}

