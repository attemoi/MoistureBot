using System;

namespace moisturebot
{
	public class ChatMessage
	{
		public string Message { get; internal set; }
		public ulong ChatterId { get; internal set; }
		public ulong ChatId { get; internal set; }
		public ChatMessage(string message, ulong chatterId, ulong chatId)
		{
			this.Message = message;
			this.ChatterId = chatterId;
			this.ChatId = chatId;
		}
	}
}

