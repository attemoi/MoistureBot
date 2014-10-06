using System;

namespace MoistureBot
{
	public class ChatRoomMessage : ChatMessage
	{
		public ulong ChatId { get; internal set; }

		public ChatRoomMessage(string message, ulong chatterId, ulong chatId) : base(message, chatterId)
		{
			this.ChatId = chatId;
		}
	}
}

