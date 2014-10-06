using System;
using Mono.Addins;

namespace MoistureBot
{
	[TypeExtensionPoint]
	public interface IChatFriendAddin : IMoistureBotAddin
	{
		/// <summary>
		/// Called when a message has been received from a friend.
		/// </summary>
		/// <param name="message">Received message</param>
		void MessageReceived(ChatMessage message);
	}
		
}

