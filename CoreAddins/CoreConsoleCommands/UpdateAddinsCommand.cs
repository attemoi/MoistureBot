using System;
using Mono.Options;
using Mono.Addins;
using System.Linq;
using MoistureBot.ExtensionPoints;
using MoistureBot.ExtensionAttributes;
using MoistureBot.Utils;
using MoistureBot.Config;

namespace MoistureBot.ConsoleCommands
{

    [ConsoleCommand(
        Name = "update-addins",
        Description = "Update and reload all addins.",
        ShortDescription = "Update and reload all addins.",
        ShortUsage = "update-addins",
        Usage = "update-addins"
    )]
    public class UpdateAddinsCommand : IConsoleCommand
    {
	
        private ILogger Logger = MoistureBotComponentProvider.GetLogger();

        public OptionSet Options {
            get
            {
                return new OptionSet();
            }
        }

        public bool Execute(string[] args)
        {
            Logger.Info("Executing command...");

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

