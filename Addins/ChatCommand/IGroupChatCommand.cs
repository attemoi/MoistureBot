using System;
using Mono.Addins;
using MoistureBot.ExtensionPoints;

namespace MoistureBot
{

    [TypeExtensionPoint("/MoistureBot/UrlInfo/IReceiveGroupChatCommand")]
    public interface IGroupChatCommand
    {
        /// <summary>
        /// Called when the bot receives a command in group chat.
        /// </summary>
        /// <param name="command">Received Command.</param>
        void Execute(Command command);
    }
}

