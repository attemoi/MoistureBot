using System;

namespace moisturebot
{
	public interface IMoistureBot
	{

		void connect();

		void disconnect();

		void SendChatRoomMessage(String message);

	}
}

