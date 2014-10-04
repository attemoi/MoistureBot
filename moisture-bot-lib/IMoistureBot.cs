using System;

namespace moisturebot
{
	public interface IMoistureBot
	{
		void Connect(string username, string password);
		void Disconnect();
		void JoinChat(ulong id);
		void SendChatMessage(String message, ulong chatId);
	}
}

