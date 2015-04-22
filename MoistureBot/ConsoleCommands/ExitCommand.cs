using System;
using System.Threading;
using Mono.Options;
using Mono.Addins;
using System.Linq;
using System.Collections.Generic;
using MoistureBot;
using MoistureBot.ExtensionAttributes;
using MoistureBot.Utils;

namespace MoistureBot.ConsoleCommands
{

    [ConsoleCommand(
        Name = "exit",
        Description = "Disconnect from Steam and exit program.",
        ShortDescription = "Disconnect from Steam and exit program.",
        ShortUsage = "exit",
        Usage = "exit"
    )]
    public class ExitCommand : IConsoleCommand
    {
	
        private IMoistureBot Bot;
        private ILogger Logger;

        [Provide]
        public ExitCommand(IContext context)
        {
            this.Bot = context.GetBot();
            this.Logger = context.GetLogger(typeof(ExitCommand));
        }

        public OptionSet Options {
            get { return new OptionSet(); }
        }

        public bool Execute(string[] args)
        {
            Logger.Info("Executing command...");

            List<string> extra = Options.Parse(args);

            if (extra.Count > 0)
                Console.Write(ConsoleMessage.ExtraParametersNotAllowed("exit"));

            Console.WriteLine("Disconnecting from Steam...");
            Bot.Disconnect();
            Bot.Terminate();
            Console.WriteLine("Exiting program...");

            return true;
        }
    }
}

