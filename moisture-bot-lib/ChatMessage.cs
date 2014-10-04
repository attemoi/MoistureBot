using System;

namespace moisturebot
{
	public class ChatMessage
	{
		public string Message { get; internal set; }
		public string Sender { get; internal set; }
		public ulong ChatRoomId { get; internal set; }
		public ChatMessage(string message, string sender, ulong chatId)
		{
			this.Message = message;
			this.Sender = sender;
			this.ChatRoomId = chatId;
		}
	}
}

