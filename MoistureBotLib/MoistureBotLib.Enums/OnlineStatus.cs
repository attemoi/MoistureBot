using System;

namespace MoistureBot
{
	public enum PersonaState
	{
		[StringAttribute("online")]
		ONLINE,
		[StringAttribute("away")]
		AWAY,
		[StringAttribute("busy")]
		BUSY,
		[StringAttribute("looking_to_play")]
		LOOKING_TO_PLAY,
		[StringAttribute("looking_to_trade")]
		LOOKING_TO_TRADE,
		[StringAttribute("offline")]
		OFFLINE,
		[StringAttribute("snooze")]
		SNOOZE,

	}
}

