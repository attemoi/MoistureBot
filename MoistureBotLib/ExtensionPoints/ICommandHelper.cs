using System;
using Mono.Options;
using System.Collections.Generic;
using MoistureBot;
using System.Linq;
using Mono.Addins;

namespace MoistureBot
{
	[TypeExtensionPoint(Path = "MoistureBot/Core/CommandHelper")]
	public interface ICommandHelper
	{
		void WriteHelp(IConsoleCommand command);
	}

}

