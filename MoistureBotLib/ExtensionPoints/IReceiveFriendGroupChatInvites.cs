using System;
using Mono.Addins;
using MoistureBot.Model;

namespace MoistureBot
{
    /// <summary>
    /// Extension point for receiving invites to (temporary) friend group chats.
    /// Closely related to <see cref="IReceiveCommunityGroupChatInvites"/> 
    /// </summary>
    [TypeExtensionPoint("/MoistureBot/IReceiveFriendGroupChatInvites", NodeType=typeof(MoistureBotExtensionNode))]
    public interface IReceiveFriendGroupChatInvites
    {
        /// <summary>
        /// Called when a invite has been received from a (temporary) friend group chat.
        /// </summary>
        /// <param name="message">Received invite</param>
        void InviteReceived(FriendGroupChatInvite invite);
    }
		
}

