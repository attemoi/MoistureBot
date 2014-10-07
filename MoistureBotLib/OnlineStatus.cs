using System;

namespace MoistureBot
{
	public enum OnlineStatus
	{
		[NameAttribute("online")]
		ONLINE,
		[NameAttribute("away")]
		AWAY,
		[NameAttribute("busy")]
		BUSY,
		[NameAttribute("looking_to_play")]
		LOOKING_TO_PLAY,
		[NameAttribute("looking_to_trade")]
		LOOKING_TO_TRADE,
		[NameAttribute("offline")]
		OFFLINE,
		[NameAttribute("snooze")]
		SNOOZE,

	}
}

