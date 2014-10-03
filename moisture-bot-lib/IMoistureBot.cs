using System;

namespace moisturebot
{
	public interface IMoistureBot
	{

		void connect();
		void start();
		void stop();
		void disconnect();
		void JoinChatRoom(ulong chatRoomId);
		void SendChatRoomMessage(String message);


	}
}

