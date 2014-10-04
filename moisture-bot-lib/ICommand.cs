using System;

namespace moisturebot
{
	public interface ICommand
	{
		void WriteHelp();
		bool Execute(IMoistureBot bot);
	}
}

