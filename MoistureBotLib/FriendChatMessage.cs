using System;

namespace MoistureBot
{
	public class FriendChatMessage
	{
		public string Message { get; internal set; }

		public ulong ChatterId { get; internal set; }

		public FriendChatMessage(string message, ulong chatterId)
		{
			this.Message = message;
			this.ChatterId = chatterId;
		}
	}
}

