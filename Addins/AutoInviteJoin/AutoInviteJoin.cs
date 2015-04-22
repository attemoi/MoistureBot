using System;
using MoistureBot;
using MoistureBot.Model;

namespace MoistureBot
{
    public class AutoInviteJoin 
		: IReceiveCommunityGroupChatInvites, IReceiveFriendGroupChatInvites
    {

        IMoistureBot Bot;
        ILogger Logger;

        [Provide]
        public AutoInviteJoin(IContext context)
        {
            this.Bot = context.GetBot();
            this.Logger = context.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

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

