using System;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;
using Mono.Addins;
using MoistureBot;
using MoistureBot.Utils;
using MoistureBot.Extensions;

namespace MoistureBot.ConsoleCommands
{

   
    public class ConnectCommand : IConsoleCommand
    {
	
        IMoistureBot Bot;
        ILogger Logger;

        [Provide]
        public ConnectCommand(IContext context)
        {
            this.Bot = context.GetBot();
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
		
            List<string> extra = Options.Parse(args);

            string user = null;
            string pass = null;

            if (extra.Count > 0)
                user = extra.ElementAt(0);
            if (extra.Count > 1)
                pass = extra.ElementAt(1);

            if (extra.Count > 2)
                Console.WriteLine(ConsoleMessage.InvalidNumberOfParameters("connect"));

            if (String.IsNullOrEmpty(user))
            {
                Console.Write("username:");
                user = Console.ReadLine();
            }
            if (String.IsNullOrEmpty(pass))
            {
                Console.Write("password:");
                pass = ConsoleHelper.ReadPassword();
            }

            Console.WriteLine("Logging in as " + user + "...");
            Console.WriteLine(ConsoleMessage.SEE_STATUS);
            Bot.Connect(user, pass);

            return false;
        }

    }
}

