using System;
using Mono.Addins;
using MoistureBot.Steam;

namespace MoistureBot.ExtensionPoints
{
	[TypeExtensionPoint]
	public interface IReceiveGroupChatMessages
	{
		/// <summary>
		/// Called when the bot has received a message in a group chat.
		/// </summary>
		/// <param name="message">Received message</param>
		void MessageReceived(GroupChatMessage message);
	}
		
}

