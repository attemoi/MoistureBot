using System;
using Mono.Addins;

namespace MoistureBot.ExtensionAttributes
{
    public class ChatCommandAttribute : CustomExtensionAttribute
    {

        public ChatCommandAttribute( [NodeAttribute("CommandName", true)] string name )
        {
            Name = name;
        }

        [NodeAttribute("CommandName", true)]
        public string Name { get; set; }

    }
}

