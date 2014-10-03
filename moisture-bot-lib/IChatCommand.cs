using System;
using Mono.Addins;

namespace moisturebot.lib
{
	[TypeExtensionPoint]
	public interface IChatCommand
	{
		void MessageReceived(string content);
	}
}

