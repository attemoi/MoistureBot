using System;
using System.Collections.ObjectModel;
using Mono.Options;
using MoistureBot;
using Mono.Addins;
using MoistureBot.ExtensionAttributes;

namespace MoistureBot.ExtensionPoints
{

	/// <summary>
	/// This is an extension point for a console command.
	/// </summary>
	[TypeExtensionPoint(ExtensionAttributeType = typeof(ConsoleCommandAttribute))]
	public interface IConsoleCommand
	{

		OptionSet Options { get; }

		bool Execute(string[] args);

	}
}

