using System;
using MoistureBot;
using MoistureBot.Model;
using Mono.Addins;
using System.Text.RegularExpressions;
using System.Linq;
using MoistureBot.Extensions;

namespace MoistureBot
{

    public class ChatCommand : IReceiveFriendChatMessages, IReceiveGroupChatMessages
    {

        private IMoistureBot Bot;
        private IContext Context;

        [Provide]
        public ChatCommand(IContext context)
        {
            this.Context = context;
            this.Bot = context.GetBot();
        }

        const string COMMAND_REGEX = @"^![^!]+";

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

            bool invoked = Context.InvokeAddins<IChatCommand, ChatCommandNode>(
                "/MoistureBot/ChatCommand/IChatCommand",
                node => node.CommandName.Equals(command.Name),
                addin => {
                    if (command.HasArguments() && command.FirstArgument.Equals("help"))
                        addin.Help(command);
                    else
                        addin.Execute(command);
                } 
            );

            if (!invoked)
            {
                Bot.SendChatMessage("Command not recognized. Type !help for a list of commands.", command.SenderId);
            }

        }

        private bool isValidCommand(string message)
        {
            return message.Equals("help") || Regex.IsMatch(message, COMMAND_REGEX);
        }

        private Command parseCommand(String input) 
        {

            var command = new Command();
            var commandParts = input.Split(' ');
            command.Name = commandParts[0].TrimStart('!');
            command.Arguments = commandParts.Skip(1).ToList();

            return command;
        }
    }
}

