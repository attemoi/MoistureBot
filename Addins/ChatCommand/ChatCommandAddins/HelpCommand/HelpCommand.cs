using System;
using MoistureBot.ExtensionPoints;
using MoistureBot.Steam;
using Mono.Addins;
using System.Linq;

namespace MoistureBot
{
    public class HelpCommand : IGroupChatCommand, IFriendChatCommand
    {

        private IMoistureBot Bot = MoistureBotComponentProvider.GetBot();
        //private ILogger Logger = MoistureBotComponentProvider.GetLogger();

        public void Execute(Command command, GroupChatMessage message) {
            if (command.Arguments.Length > 0 && command.Arguments[0].Equals("help"))
                return;

            Bot.SendChatMessage(createResponse(), message.ChatterId);
        }

        public void Execute(Command command, FriendChatMessage message) {
            if (command.Arguments.Length > 0 && command.Arguments[0].Equals("help"))
                return;

            Bot.SendChatMessage(createResponse(), message.ChatterId);
        }

        private String createResponse() {
            
            var groupCommands = AddinManager
                .GetExtensionNodes<ChatCommandNode>("/MoistureBot/ChatCommand/IGroupChatCommand");

            var friendCommands = AddinManager
                .GetExtensionNodes<ChatCommandNode>("/MoistureBot/ChatCommand/IFriendChatCommand");

            string message = ".\n\nGroup chat commands:\n\n"; 
            message += String.Join("\n\n    !", groupCommands.Select(c => c.CommandName + " - " + c.CommandDescription));
          
            message += "\n\nFriend chat commands:\n\n";
            message += String.Join("\n\n", friendCommands.Select(c => c.CommandName + " - " + c.CommandDescription));

            return message;
        }

        public void Help(GroupChatMessage message) {
            Bot.SendChatRoomMessage(getHelp(), message.ChatId);
        }

        public void Help(FriendChatMessage message) {
            Bot.SendChatMessage(getHelp(), message.ChatterId);
        }
            
        // Display help into private chat
        private String getHelp() {
            return @"
            
Provides help for chat commands.

!help

    displays list of available commands.

!help command
   
    displays help for a specific command.
";

        }
    }
}

