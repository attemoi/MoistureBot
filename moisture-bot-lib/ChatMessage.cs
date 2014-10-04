using System;

namespace moisturebot
{
	public class ChatMessage
	{
		public string Message { get; internal set; }
		public ulong ChatterId { get; internal set; }
		public string ChatterName { get; internal set; }
		public ulong ChatId { get; internal set; }
		public ChatMessage(string message, ulong chatterId, string chatterName, ulong chatId)
		{
			this.Message = message;
			this.ChatterId = chatterId;
			this.ChatterName = chatterName;
			this.ChatId = chatId;
		}
	}
}

