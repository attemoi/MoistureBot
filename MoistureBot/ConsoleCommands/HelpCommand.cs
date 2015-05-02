using System;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;
using Mono.Addins;
using MoistureBot;
using MoistureBot.Extensions;

namespace MoistureBot.ConsoleCommands
{

    public class HelpCommand : IConsoleCommand
    {
         
        public string command;

        public OptionSet Options {
            get { return new OptionSet(); }
        }

        public bool Execute(string[] args)
        {

            List<string> extra = Options.Parse(args);

            if (extra.Count > 0)
            {
                command = extra.First();

                foreach (var node in AddinManager.GetExtensionNodes<ConsoleCommandNode>("/MoistureBot/IConsoleCommand"))
                {
                    if (command.Equals(node.Name))
                    {
                        Console.WriteLine();
                        Console.WriteLine("Description:");
                        Console.WriteLine();
                        Console.WriteLine("  {0}", node.Description);
                        Console.WriteLine();
                        Console.WriteLine("Usage:");
                        Console.WriteLine();
                        Console.WriteLine("  {0}", node.Usage);
                        Console.WriteLine();
                        var cmd = (IConsoleCommand)node.CreateInstance();
                        if (cmd.Options.Count > 0)
                        {
                            Console.WriteLine("Options:");
                            Console.WriteLine();
                            cmd.Options.WriteOptionDescriptions(Console.Out);
                        }

                        return false;
                    }
                }

                Console.WriteLine("Unknown command: '{0}'", command);

            }
            else
            {

                Console.WriteLine();
                Console.WriteLine("Available commands: ");
                Console.WriteLine();

                foreach (var node in AddinManager.GetExtensionNodes<ConsoleCommandNode>("/MoistureBot/IConsoleCommand"))
                {
                    Console.WriteLine("  {0} - {1}", node.ShortUsage.PadRight(35), node.ShortDescription.PadRight(35));
                }
                    
            }
            return false;
        }

    }
}

