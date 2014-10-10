using System;

namespace MoistureBot.Steam
{
	public class Invite
	{
		ulong ChatRoomId { get; set; }
		ulong InviterId { get; set; }

		public Invite( ulong chatRoomId, ulong inviterId) 
		{
			ChatRoomId = chatRoomId;
			InviterId = inviterId;
		}
	}

	public class CommunityGroupChatInvite : Invite
	{
		string ChatRoomName { get; set; }

		public  CommunityGroupChatInvite( ulong chatRoomId, string chatRoomName, ulong inviterId) 
			: base(chatRoomId, inviterId)
		{
			ChatRoomName = chatRoomName;
		}
	}

	public class FriendGroupChatInvite : Invite
	{
		public  FriendGroupChatInvite( ulong chatRoomId, ulong inviterId ) 
			: base(chatRoomId, inviterId)
		{
		}
	}

	public class GameLobbyInvite: Invite
	{
		ulong GameId { get; set; }

		public GameLobbyInvite( ulong chatRoomId, ulong inviterId, ulong gameId) 
			: base(chatRoomId, inviterId)
		{
			GameId = gameId;
		}
	}
}

