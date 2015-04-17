using System;
using MoistureBot.ExtensionPoints;
using MoistureBot.Steam;
using System.Linq;

namespace MoistureBot
{
    public class UrlHistoryCommand : IChatCommand
    {

        private IMoistureBot Bot = MoistureBotComponentProvider.GetBot();
        private ILogger Logger = MoistureBotComponentProvider.GetLogger();

        public void Execute(Command command) {

        }

        public void Help(Command command) {
            Bot.SendChatMessage(getHelpMessage(), command.SenderId);
        }
            
        // Display help into private chat
        private String getHelpMessage() {
            return @"Provides help for chat commands.

!help
    displays list of available commands.

!help command 
    displays help for a specific command.
";

        }
    }
}

