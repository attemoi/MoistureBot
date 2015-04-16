using System;
using MoistureBot.ExtensionPoints;
using MoistureBot.Steam;
using Mono.Addins;
using System.Text.RegularExpressions;
using System.Linq;
using MoistureBot.ExtensionAttributes;

namespace MoistureBot
{

    public class ChatCommand : IReceiveFriendChatMessages, IReceiveGroupChatMessages
    {

        private ILogger Logger = MoistureBotComponentProvider.GetLogger();

        const string COMMAND_REGEX = @"^!+.";

        public void MessageReceived(FriendChatMessage message)
        {
            Command command = parseCommand(message.Message);
            if (command == null)
                return;

            var commandNode = AddinManager

                .GetExtensionNodes<ChatCommandNode>("/MoistureBot/ChatCommand/IFriendChatCommand")
                .FirstOrDefault((node) => node.CommandName.Equals(command.Name));

            if (commandNode != null)
            {
                try {
                    Logger.Info("Friend chat command received, executing addin.");
                    ((IFriendChatCommand)commandNode.CreateInstance()).Execute(command, message);
                } catch (Exception e) {
                    Logger.Error("Error while executing friend chat command.", e);
                }
            }

        }

        public void MessageReceived(GroupChatMessage message)
        {

            Command command = parseCommand(message.Message);
            if (command == null)
                return;

            var commandNode = AddinManager
                .GetExtensionNodes<ChatCommandNode>("/MoistureBot/ChatCommand/IGroupChatCommand")
                .FirstOrDefault((node) => node.CommandName.Equals(command.Name));

            if (commandNode != null)
            {
                try {
                    Logger.Info("Group chat command received, executing addin.");
                    ((IGroupChatCommand)commandNode.CreateInstance()).Execute(command, message);
                } catch (Exception e) {
                    Logger.Error("Error while executing group chat command.", e);
                }
            }

        }

        private bool isValidCommand(string message)
        {
            return Regex.IsMatch(message, COMMAND_REGEX);
        }

        private Command parseCommand(String input) {

            if (!isValidCommand(input))
                return null;

            var commandParts = input.Split(' ').ToList();
            var commandName = commandParts[0].Substring(1);
            var args = commandParts.Skip(1).ToArray(); // the arguments is after the command

            return new Command(commandName, args);
        }
    }
}

