using System;
using Mono.Addins;
using MoistureBot.ExtensionPoints;
using MoistureBot.Steam;

[assembly:Addin("AutoJoiner", "1.0")]
[assembly:AddinDependency("MoistureBot", "1.0")]
[assembly:AddinAuthor("Atte Moisio")]
[assembly:AddinDescription("Automatically join chat rooms when invited.")]
[assembly:AddinName("AutoJoiner")]
[assembly:AddinUrl("")]
namespace MoistureBot
{
	[Extension(typeof(IReceiveCommunityGroupChatInvites))]
	[Extension(typeof(IReceiveFriendGroupChatInvites))]
	public class AutoJoiner 
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

