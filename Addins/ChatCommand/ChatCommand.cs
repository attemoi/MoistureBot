﻿using System;
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
        private IMoistureBot Bot = MoistureBotComponentProvider.GetBot();

        const string COMMAND_REGEX = @"^!+.";

        public void MessageReceived(FriendChatMessage message)
        {
            if (isValidCommand(message.Message))
                executeCommand(message.Message, Command.CommandSource.FRIEND, 0, message.ChatterId);
        }
           
        public void MessageReceived(GroupChatMessage message)
        {
            if (isValidCommand(message.Message))
                executeCommand(message.Message, Command.CommandSource.GROUPCHAT, message.ChatId, message.ChatterId);
        }

        private void executeCommand(String message, Command.CommandSource source, ulong chatId, ulong senderId) {

            Command command = parseCommand(message);
            if (command == null)
                return;

            command.Source = source;
            command.ChatRoomId = chatId;
            command.SenderId = senderId;

            var commandNode = AddinManager
                .GetExtensionNodes<ChatCommandNode>("/MoistureBot/ChatCommand/IChatCommand")
                .FirstOrDefault((node) => node.CommandName.Equals(command.Name));

            if (commandNode != null)
            {
                try
                {
                    Logger.Info("Chat command received, executing addin.");
                    if (command.Arguments.Length > 0 && command.Arguments[0].Equals("help"))
                        ((IChatCommand)commandNode.CreateInstance()).Help(command);
                    else
                        ((IChatCommand)commandNode.CreateInstance()).Execute(command);
                }
                catch (Exception e)
                {
                    Logger.Error("Error while executing chat command.", e);
                }
            }
            else
            {
                Bot.SendChatMessage("Command not recognized. Type !help for a list of commands.", command.SenderId);
            }

        }

        private bool isValidCommand(string message)
        {
            return message.Equals("help") || Regex.IsMatch(message, COMMAND_REGEX);
        }

        private Command parseCommand(String input) {

            var command = new Command();
            var commandParts = input.Split(' ').ToList();
            command.Name = commandParts[0].TrimStart('!');
            command.Arguments = commandParts.Skip(1).ToArray();

            return command;
        }
    }
}

