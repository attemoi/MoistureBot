using System;
using Mono.Addins;

namespace moisturebot
{
	[TypeExtensionPoint]
	public interface IGroupChatAddin
	{
		void Initialize (IMoistureBot bot);

		void MessageReceived(ChatMessage message);
	}
		
}

