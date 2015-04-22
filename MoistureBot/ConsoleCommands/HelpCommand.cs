using System;
using Mono.Options;
using System.Collections.Generic;
using System.Linq;
using Mono.Addins;
using MoistureBot.ExtensionPoints;
using MoistureBot.ExtensionAttributes;

namespace MoistureBot.ConsoleCommands
{

    [ConsoleCommandAttribute(
        Name = "help",
        ShortDescription = "Print help for a command.",
        ShortUsage = "help <command>",
        Description = "Print help for a command.",
        Usage = "help <command>"
    )]
    public class HelpCommand : IConsoleCommand
    {
    
        private ILogger Logger = new MoistureBotFactory().GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string command;

        public OptionSet Options {
            get { return new OptionSet(); }
        }

        public bool Execute(string[] args)
        {

            Logger.Info("Executing command...");

            List<string> extra = Options.Parse(args);

            if (extra.Count > 0)
            {
                command = extra.First();

                foreach (TypeExtensionNode<ConsoleCommandAttribute> node in AddinManager.GetExtensionNodes (typeof(IConsoleCommand)))
                {
                    if (command.Equals(node.Data.Name))
                    {
                        Console.WriteLine();
                        Console.WriteLine("Description:");
                        Console.WriteLine();
                        // TODO Wrap to 80 chars
                        Console.WriteLine("  {0}", node.Data.Description);
                        Console.WriteLine();
                        Console.WriteLine("Usage:");
                        Console.WriteLine();
                        Console.WriteLine("  {0}", node.Data.Usage);
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

                ExtensionNodeList commands = AddinManager.GetExtensionNodes(typeof(IConsoleCommand));

                foreach (TypeExtensionNode<ConsoleCommandAttribute> node in commands)
                {
                    Console.WriteLine("  {0} - {1}", node.Data.ShortUsage.PadRight(35), node.Data.ShortDescription.PadRight(35));
                }

            }
            return false;
        }

    }
}

