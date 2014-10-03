using System;
using Mono.Addins;

namespace moisturebot
{
	[TypeExtensionPoint]
	public interface IGroupChatAddin : IMoistureBotAddin
	{
		void MessageReceived(ChatMessage message);
	}
		
}

