using System;
using Mono.Addins;
using MoistureBot.Steam;

namespace MoistureBot.ExtensionPoints
{
	/// <summary>
	/// Extension point for receiving invites to community group chats.
	/// Closely related to <see cref="IReceiveFriendGroupChatInvites"/> 
	/// </summary>
	[TypeExtensionPoint]
	public interface IReceiveCommunityGroupChatInvites
	{
		/// <summary>
		/// Called when the bot has received a message from a community group chat.
		/// </summary>
		/// <param name="message">Received invite</param>
		void InviteReceived(CommunityGroupChatInvite invite);
	}
		
}

