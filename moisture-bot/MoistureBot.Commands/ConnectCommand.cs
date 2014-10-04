using System;

namespace moisturebot.commands
{
	public class ConnectCommand : ICommandWithArgs
	{

		public string[] Args { get; set; }



		public bool Execute (IMoistureBot bot)
		{
			string user, pass;

			Console.Write ("username:");
			user = Console.ReadLine ();
			Console.Write ("password:");
			pass = ConsoleUtils.ReadPassword ();

			bot.Connect(user, pass);

			return false;
		}

	}
}

