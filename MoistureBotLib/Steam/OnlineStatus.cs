using System;
using MoistureBot.Config;

namespace MoistureBot.Steam
{
	public enum OnlineStatus
	{
		[Name("online")]
		ONLINE,
		[Name("away")]
		AWAY,
		[Name("busy")]
		BUSY,
		[Name("looking_to_play")]
		LOOKING_TO_PLAY,
		[Name("looking_to_trade")]
		LOOKING_TO_TRADE,
		[Name("offline")]
		OFFLINE,
		[Name("snooze")]
		SNOOZE,

	}
}

