using System;
using Mono.Addins;

namespace MoistureBot
{

    [ExtensionNode ("ChatCommand")]
    public class ChatCommandNode : MoistureBotExtensionNode
    {

        [NodeAttribute("commandName", true)]
        public String CommandName { get; set; }

        [NodeAttribute("commandDescription", false)]
        public String CommandDescription { get; set; }

    }
}

