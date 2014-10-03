using System;
using Mono.Addins;

namespace moisturebot.lib
{
	[TypeExtensionPoint]
	public interface IChatCommand
	{
		string ReplyToMessage(ChatMessage message);
	}
}

