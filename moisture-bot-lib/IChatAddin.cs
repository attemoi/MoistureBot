using System;
using Mono.Addins;

namespace moisturebot
{
	[TypeExtensionPoint]
	public interface IChatAddin : IMoistureBotAddin
	{
		void MessageReceived(ChatMessage message);
	}
		
}

