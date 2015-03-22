using System;
using Mono.Addins;
using MoistureBot.Steam;

namespace MoistureBot.ExtensionPoints
{

	/// <summary>
	/// Extension point for receiving messages in a private chat with a user.
	/// Closely related to <see cref="IReceiveGroupChatMessages"/> 
	/// </summary>
	[TypeExtensionPoint("/MoistureBot/IReceiveFriendChatMessages")]
	public interface IReceiveFriendChatMessages
	{
		/// <summary>
		/// Called when the bot has received a message from a user.
		/// </summary>
		/// <param name="message">Received message</param>
		void MessageReceived(FriendChatMessage message);
	}
		
}

