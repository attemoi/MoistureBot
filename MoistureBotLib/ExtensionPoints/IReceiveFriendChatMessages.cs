using System;
using Mono.Addins;
using MoistureBot.Steam;

namespace MoistureBot.ExtensionPoints
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

