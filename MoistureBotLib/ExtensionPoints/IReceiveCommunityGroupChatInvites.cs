using System;
using Mono.Addins;

namespace MoistureBot
{
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

