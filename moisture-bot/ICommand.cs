using System;

namespace moisturebot
{
	public interface ICommand
	{
		string[] Args { get; set; }
		void WriteHelp();
		bool Execute(IMoistureBot bot);
	}
}

