using System;
using Mono.Addins;
using MoistureBot.Model;

namespace MoistureBot
{

    public interface IChatCommand
    {
        /// <summary>
        /// Called when the bot receives a command in friend chat.
        /// </summary>
        /// <param name="command">Received Command.</param>
        void Execute(Command command);

        /// <summary>
        /// Used to display help, when the user types "!help <commandname>
        /// </summary>
        /// <param name="command">Received Command.</param>
        void Help(Command command);
    }
}

