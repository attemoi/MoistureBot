using System;
using MoistureBot.ExtensionPoints;
using MoistureBot.Steam;

namespace MoistureBot
{
    public class HelpCommand : IGroupChatCommand, IFriendChatCommand
    {

        private IMoistureBot Bot = MoistureBotComponentProvider.GetBot();
        private ILogger Logger = MoistureBotComponentProvider.GetLogger();

        public void Execute(Command command, GroupChatMessage message) {
            Bot.SendChatRoomMessage("Sorry, I can't help you!", message.ChatId);
        }

        public void Execute(Command command, FriendChatMessage message) {
            Bot.SendChatMessage("Sorry, I can't help you!", message.ChatterId);
        }
            
    }
}

