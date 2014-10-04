using System;

namespace moisturebot
{
	public interface ICommandWithArgs : ICommand
	{
		string[] Args { get; set; }
	}
}

