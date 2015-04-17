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
        private ILogger Logger = MoistureBotComponentProvider.GetLogger();

        public void Execute(Command command, GroupChatMessage message) {

            if (command.Arguments.Length > 0)
            {
                var commandNode = AddinManager
                    .GetExtensionNodes<ChatCommandNode>("/MoistureBot/ChatCommand/IGroupChatCommand")
                    .FirstOrDefault((node) => node.CommandName.Equals(command.Arguments[0]));

                if (commandNode != null)
                {
                    try
                    {
                        Logger.Info("Displaying help for command.");
                        ((IGroupChatCommand)commandNode.CreateInstance()).Help(message);
                    } catch (Exception e) {
                        Logger.Error("Error while displaying help for command.", e);
                    }
                }
            }
            else
            {
                Bot.SendChatMessage(".\n" + getGroupCommandsMessage() + "\n" + getFriendCommandsMessage(), message.ChatterId);
            }

        }

        public void Execute(Command command, FriendChatMessage message) {
            if (command.Arguments.Length > 0)
            {
                var commandNode = AddinManager
                    .GetExtensionNodes<ChatCommandNode>("/MoistureBot/ChatCommand/IFriendChatCommand")
                    .FirstOrDefault((node) => node.CommandName.Equals(command.Arguments[0]));

                if (commandNode != null)
                {
                    try
                    {
                        Logger.Info("Displaying help for command.");
                        ((IFriendChatCommand)commandNode.CreateInstance()).Help(message);
                    } catch (Exception e) {
                        Logger.Error("Error while displaying help for command.", e);
                    }
                }
            }
            else
            {
                Bot.SendChatMessage(getResponse(), message.ChatterId);
            }
        }

        private void displayHelp<CommandType, ChatMessageType>(Command command, String extensionPath, CommandType commandType, ChatMessageType messageType) {
            
        }

        private String getResponse() {
            return ".\n" + getGroupCommandsMessage() + "\n" + getFriendCommandsMessage();
        }
            
        private String getGroupCommandsMessage() {
            var commands = AddinManager
                .GetExtensionNodes<ChatCommandNode>("/MoistureBot/ChatCommand/IGroupChatCommand")
                .Select(c => "    !" + c.CommandName + " - " + c.CommandDescription)
                .Aggregate((a, b) => a + "\n" + b);

            return "Group chat commands:\n" + commands;
        }

        private String getFriendCommandsMessage() {

            var commands = AddinManager
                .GetExtensionNodes<ChatCommandNode>("/MoistureBot/ChatCommand/IFriendChatCommand")
                .Select(c => "    !" + c.CommandName + " - " + c.CommandDescription)
                .Aggregate((a, b) => a + "\n" + b);

            return "Friend chat commands:\n" + commands;
        }

        public void Help(GroupChatMessage message) {
            Bot.SendChatMessage(getHelp(), message.ChatterId);
        }

        public void Help(FriendChatMessage message) {
            Bot.SendChatMessage(getHelp(), message.ChatterId);
        }

        // Display help into private chat
        private String getHelp() {
            return @"Provides help for chat commands.

!help
    displays list of available commands.

!help command 
    displays help for a specific command.
";

        }
    }
}

