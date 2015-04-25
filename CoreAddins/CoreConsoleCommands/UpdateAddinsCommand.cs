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
	
        ILogger Logger;

        [Provide]
        public UpdateAddinsCommand(IContext context)
        {
            this.Logger = context.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

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

