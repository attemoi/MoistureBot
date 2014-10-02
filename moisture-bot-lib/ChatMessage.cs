using System;

namespace moisturebot.contracts
{
	public class ChatMessage
	{
		public string Message { get; internal set; }
		public string SenderName { get; internal set; }

		public ChatMessage(string message, string senderName)
		{
			this.Message = message;
			this.SenderName = senderName;
		}
	}
}

