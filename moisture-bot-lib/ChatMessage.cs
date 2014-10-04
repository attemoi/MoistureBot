﻿using System;

namespace moisturebot
{
	public class ChatRoomMessage : ChatMessage
	{
		public string Message { get; internal set; }
		public ulong ChatterId { get; internal set; }
		public ulong ChatId { get; internal set; }
		public ChatRoomMessage(string message, ulong chatterId, ulong chatId)
		{
			super (message, chatterId);

			this.Message = message;
			this.ChatterId = chatterId;
			this.ChatId = chatId;
		}
	}
}

