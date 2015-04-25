using System;
using Mono.Addins;

namespace MoistureBot.Extensions
{
  
    [ExtensionNode ("ConsoleCommand")]
    public class ConsoleCommandNode : MoistureBotExtensionNode
    {

        [NodeAttribute("name")]
        public string Name { get; set; }

        [NodeAttribute("description")]
        public string Description { get; set; }

        [NodeAttribute("shortUsage")]
        public string ShortUsage { get; set; }

        [NodeAttribute("shortDescription")]
        public string ShortDescription { get; set; }

        [NodeAttribute("usage")]
        public string Usage { get; set; }

    }
}

