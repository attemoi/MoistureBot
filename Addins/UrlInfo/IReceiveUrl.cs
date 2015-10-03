using System;
using Mono.Addins;
using MoistureBot.Extensions;

namespace MoistureBot
{

    [TypeExtensionPoint("/MoistureBot/UrlInfo/IReceiveUrl", NodeType=typeof(MoistureBotExtensionNode))]
    public interface IReceiveUrl 
    {
        /// <summary>
        /// Called when the bot receives an url in any chat.
        /// </summary>
        /// <returns>Bot reply to the url message. Null for no reply.</returns>
        /// <param name="receivedUrl">Received URL.</param>
        String ReplyToUrl(Uri receivedUrl);
    }
}

