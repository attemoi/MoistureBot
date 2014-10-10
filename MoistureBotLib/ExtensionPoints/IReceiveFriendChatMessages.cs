using System;
using Mono.Addins;

namespace MoistureBot
{
	[TypeExtensionPoint]
	public interface IReceiveFriendChatMessages
	{
		/// <summary>
		/// Called when the bot has received a message from a user.
		/// </summary>
		/// <param name="message">Received message</param>
		void MessageReceived(FriendChatMessage message);
	}
		
}

