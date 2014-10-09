using System;
using System.Collections.ObjectModel;
using Mono.Options;
using MoistureBot.Config;
using Mono.Addins;

namespace MoistureBot
{

	[TypeExtensionPoint (ExtensionAttributeType=typeof(ConsoleCommandAttribute))]
	public interface ICommand
	{

		OptionSet Options { get; }

		bool Execute(string[] args);
	}
}

