using System;
using Mono.Addins;

namespace moisturebot
{
	[TypeExtensionPoint]
	public interface IChatRoomAddin : IMoistureBotAddin
	{
		void MessageReceived(ChatRoomMessage message);
	}
		
}

