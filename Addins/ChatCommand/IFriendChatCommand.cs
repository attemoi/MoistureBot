using System;
using Mono.Addins;

namespace MoistureBot
{

    [TypeExtensionPoint("/MoistureBot/UrlInfo/IReceiveFriendChatCommand")]
    public interface IFriendChatCommand
    {
        /// <summary>
        /// Called when the bot receives a command in friend chat.
        /// </summary>
        /// <param name="command">Received Command.</param>
        String Execute(Command command);
    }
}

