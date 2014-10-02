using System;
using Mono.Addins;

namespace moisturebot.contracts
{
	[TypeExtensionPoint]
	public interface IChatCommand
	{
		void MessageReceived(string content);
	}
}

