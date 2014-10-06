using System;

namespace MoistureBot
{
	public class ChatMessage
	{
		public string Message { get; internal set; }
		public ulong ChatterId { get; internal set; }

		public ChatMessage(string message, ulong chatterId)
		{
			this.Message = message;
			this.ChatterId = chatterId;
		}
	}
}

