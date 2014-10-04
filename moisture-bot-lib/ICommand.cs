using System;

namespace moisturebot
{
	public interface ICommand
	{
		bool Execute(IMoistureBot bot);
	}
}

