using System;
using Mono.Addins;

namespace moisturebot
{
	[TypeExtensionPoint]
	public interface IChatRoomAddin : IMoistureBotAddin
	{
		/// <summary>
		/// Called when a message has been received in any chat room.
		/// </summary>
		/// <param name="message">Received message</param>
		void MessageReceived(ChatRoomMessage message);
	}
		
}

