using System;
using Mono.Addins;

namespace moisturebot
{
	[TypeExtensionPoint]
	public abstract class IGroupChatAddin : IMoistureBotAddin
	{
		public abstract void MessageReceived(ChatMessage message);
	}
		
}

