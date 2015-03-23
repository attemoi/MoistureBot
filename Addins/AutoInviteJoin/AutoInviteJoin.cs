using System;
using MoistureBot.ExtensionPoints;
using MoistureBot.Steam;

namespace MoistureBot
{
    public class AutoInviteJoin 
		: IReceiveCommunityGroupChatInvites, IReceiveFriendGroupChatInvites
    {

        IMoistureBot Bot = MoistureBotComponentProvider.GetBot();
        ILogger Logger = MoistureBotComponentProvider.GetLogger();

        public void InviteReceived(FriendGroupChatInvite invite)
        {
            Logger.Info("Friend group chat invite received.");
            HandleInvite(invite);
        }

        public void InviteReceived(CommunityGroupChatInvite invite)
        {
            Logger.Info("Community group chat invite received.");
            HandleInvite(invite);
        }

        void HandleInvite(Invite invite)
        {
            Logger.Info("Joining chat room " + invite.ChatRoomId);
            Bot.JoinChatRoom(invite.ChatRoomId);
        }
    }
}

