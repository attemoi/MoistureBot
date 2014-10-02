using System;
using SteamKit2;

namespace moisturebot
{
	public class ChatMsgEventArgs : EventArgs
	{
		public SteamFriends.ChatMsgCallback Callback { get; internal set; }
		public ChatMsgEventArgs(SteamFriends.ChatMsgCallback callback)
		{
			this.Callback = callback;
		}
	}
}

