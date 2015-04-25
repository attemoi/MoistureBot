using System;
using Mono.Addins;
using MoistureBot.Model;

namespace MoistureBot.Extensions
{
    /// <summary>
    /// Extension point for receiving invites to community group chats.
    /// Closely related to <see cref="IReceiveFriendGroupChatInvites"/> 
    /// </summary>
    [TypeExtensionPoint("/MoistureBot/IReceiveCommunityGroupChatInvites", NodeType=typeof(MoistureBotExtensionNode))]
    public interface IReceiveCommunityGroupChatInvites
    {
        /// <summary>
        /// Called when the bot has received a message from a community group chat.
        /// </summary>
        /// <param name="message">Received invite</param>
        void InviteReceived(CommunityGroupChatInvite invite);
    }
		
}

