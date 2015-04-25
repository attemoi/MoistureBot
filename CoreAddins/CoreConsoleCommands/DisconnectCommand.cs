﻿using System;
using Mono.Options;
using System.Collections.Generic;
using Mono.Addins;
using System.Linq;
using MoistureBot;
using MoistureBot.Utils;

namespace MoistureBot.ConsoleCommands
{

    public class DisconnectCommand : IConsoleCommand
    {

        IMoistureBot Bot;
        ILogger Logger;

        [Provide]
        public DisconnectCommand(IContext context)
        {
            this.Bot = context.GetBot();
            this.Logger = context.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public OptionSet Options {
            get { return new OptionSet(); }
        }

        public bool Execute(string[] args)
        {
            Logger.Info("Executing command...");
            List<string> extra = Options.Parse(args);

            if (extra.Count > 0)
                Console.WriteLine(ConsoleMessage.ExtraParametersNotAllowed("disconnect"));
				
            if (!Bot.IsConnected())
            {
                Logger.Info("Bot already offline.");
                return false;
            }

            Console.WriteLine("Disconnecting bot...");
            Bot.Disconnect();
            return false;
        }
    }
}

