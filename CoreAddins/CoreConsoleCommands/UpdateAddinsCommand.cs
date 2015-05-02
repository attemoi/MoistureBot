using System;
using Mono.Options;
using Mono.Addins;
using System.Linq;
using MoistureBot;
using MoistureBot.Utils;
using MoistureBot.Config;
using MoistureBot.Extensions;

namespace MoistureBot.ConsoleCommands
{

    public class UpdateAddinsCommand : IConsoleCommand
    {

        public OptionSet Options {
            get
            {
                return new OptionSet();
            }
        }

        public bool Execute(string[] args)
        {

            var extra = Options.Parse(args);

            if (extra.Count > 0)
            {
                Console.WriteLine(ConsoleMessage.ExtraParametersNotAllowed("update-addins"));
            }
				
            Console.WriteLine("Updating add-in registry...");
            AddinManager.Registry.Update();
		
            return false;
        }
    }
}

