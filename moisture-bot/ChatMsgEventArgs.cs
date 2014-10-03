using System;

namespace moisturebot
{
	public class ChatMsgEventArgs : EventArgs
	{
		public string Message { get; internal set; }
		public string Sender { get; internal set; }
		public ChatMsgEventArgs(string message, string sender)
		{
			this.Message = message;
			this.Sender = sender;
		}
	}
}

