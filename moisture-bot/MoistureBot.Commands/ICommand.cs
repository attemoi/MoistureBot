using System;

namespace MoistureBot.Commands
{
	public interface ICommand
	{
		string[] Args { get; set; }
		void WriteHelp();
		bool Execute(IMoistureBot bot);
	}
}

