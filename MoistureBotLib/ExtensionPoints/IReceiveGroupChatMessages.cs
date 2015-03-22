using System;
using Mono.Addins;
using MoistureBot.Steam;

namespace MoistureBot.ExtensionPoints
{


	/// <summary>
	/// Extension point for receiving messages in a group chat.
	/// Closely related to <see cref="IReceiveFriendChatMessages"/> 
	/// </summary>
	[TypeExtensionPoint("/MoistureBot/IReceiveGroupChatMessages")]
	public interface IReceiveGroupChatMessages
	{
		/// <summary>
		/// Called when the bot has received a message in a group chat.
		/// </summary>
		/// <param name="message">Received message</param>
		void MessageReceived(GroupChatMessage message);
	}
		
}

