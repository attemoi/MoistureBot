using System;
using Mono.Addins;

namespace MoistureBot
{
	[TypeExtensionPoint]
	public interface IChatRoomAddin
	{
		/// <summary>
		/// Called when a message has been received in any chat room.
		/// </summary>
		/// <param name="message">Received message</param>
		void MessageReceived(ChatRoomMessage message);
	}
		
}

