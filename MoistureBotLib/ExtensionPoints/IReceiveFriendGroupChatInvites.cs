using System;
using Mono.Addins;
using MoistureBot.Steam;

namespace MoistureBot.ExtensionPoints
{
    /// <summary>
    /// Extension point for receiving invites to (temporary) friend group chats.
    /// Closely related to <see cref="IReceiveCommunityGroupChatInvites"/> 
    /// </summary>
    [TypeExtensionPoint("/MoistureBot/IReceiveFriendGroupChatInvites")]
    public interface IReceiveFriendGroupChatInvites
    {
        /// <summary>
        /// Called when a invite has been received from a (temporary) friend group chat.
        /// </summary>
        /// <param name="message">Received invite</param>
        void InviteReceived(FriendGroupChatInvite invite);
    }
		
}

