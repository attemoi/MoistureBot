using System;
using Mono.Addins;

namespace MoistureBot
{
    public class ChatCommandNode : TypeExtensionNode
    {

        [NodeAttribute("commandName", true)]
        private String commandName;

        public String CommandName {
            get { return commandName; }
        }

    }
}

