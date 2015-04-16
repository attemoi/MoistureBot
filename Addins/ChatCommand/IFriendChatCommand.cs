using System;
using Mono.Addins;
using MoistureBot.Steam;

namespace MoistureBot
{

    public interface IFriendChatCommand
    {
        /// <summary>
        /// Called when the bot receives a command in friend chat.
        /// </summary>
        /// <param name="command">Received Command.</param>
        void Execute(Command command, FriendChatMessage message);
    }
}

