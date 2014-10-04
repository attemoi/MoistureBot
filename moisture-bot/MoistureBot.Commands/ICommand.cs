using System;

namespace moisturebot.commands
{
	public interface ICommand
	{
		bool Execute(IMoistureBot bot);
	}
}

