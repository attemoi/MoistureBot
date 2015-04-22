using System;
using MoistureBot;
using MoistureBot.Model;
using Mono.Addins;
using System.Linq;

namespace MoistureBot
{
    public class HelpCommand : IChatCommand
    {

        private IMoistureBot Bot;
        private ILogger Logger;

        [Provide]
        public HelpCommand(IContext context)
        {
            this.Bot = context.GetBot();
            this.Logger = context.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public void Execute(Command command) {

            if (command.Arguments.Count> 0)
            {
                var commandNode = AddinManager
                    .GetExtensionNodes<ChatCommandNode>("/MoistureBot/ChatCommand/IChatCommand")
                    .FirstOrDefault((node) => node.CommandName.Equals(command.Arguments[0]));

                if (commandNode != null)
                {
                    try
                    {
                        Logger.Info("Displaying help for command.");
                        ((IChatCommand)commandNode.CreateInstance()).Help(command);
                    } catch (Exception e) {
                        Logger.Error("Error while displaying help for command.", e);
                    }
                }
            }
            else
            {
                Bot.SendChatMessage( getAvailableCommandsMessage(), command.SenderId);
            }

        }
               
        private String getAvailableCommandsMessage() {

            return "Available commands:\n"
                + String.Join("\n", 
                    AddinManager
                        .GetExtensionNodes<ChatCommandNode>("/MoistureBot/ChatCommand/IChatCommand")
                        .Select(c => "    !" + c.CommandName + " - " + c.CommandDescription))
                + "\n\n    Type '!help <command>' to get help for a single command.";

        }

        public void Help(Command command) {
            Bot.SendChatMessage(getHelpMessage(), command.SenderId);
        }

        // Display help into private chat
        private String getHelpMessage() {
            return @"Provides help for chat commands.

!help
    displays list of available commands.

!help <command_name> 
    displays help for a specific command.
";

        }
    }
}

