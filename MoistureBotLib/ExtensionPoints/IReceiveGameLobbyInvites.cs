﻿using System;
using Mono.Addins;
using MoistureBot.Model;

namespace MoistureBot
{
    /// <summary>
    /// Extension point for receiving game lobby invites.
    /// </summary>
    [TypeExtensionPoint("/MoistureBot/IReceiveGameLobbyInvites")]
    public interface IReceiveGameLobbyInvites
    {
        /// <summary>
        /// Called when the bot has received an invite to a game lobby.
        /// </summary>
        /// <param name="message">Received invite</param>
        void InviteReceived(GameLobbyInvite invite);
    }
		
}

