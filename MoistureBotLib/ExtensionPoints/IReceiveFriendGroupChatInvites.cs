using System;
using Mono.Addins;
using MoistureBot.Steam;

namespace MoistureBot.ExtensionPoints
{
	[TypeExtensionPoint]
	public interface IReceiveFriendGroupChatInvites
	{
		/// <summary>
		/// Called when a invite has been received from a (temporary) friend group chat.
		/// </summary>
		/// <param name="message">Received invite</param>
		void InviteReceived(FriendGroupChatInvite invite);
	}
		
}

