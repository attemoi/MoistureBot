using System;

namespace moisturebot
{
	public interface IMoistureBot
	{
		void Connect(string username, string password);
		void Disconnect();
		void JoinChatRoom(ulong chatRoomId);
		void SendChatRoomMessage(String message, ulong chatId);
	}
}

