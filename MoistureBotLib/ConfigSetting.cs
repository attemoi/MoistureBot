using System;

namespace MoistureBot
{
	public enum ConfigSetting
	{
		[Section("bot_settings")]
		[Key("online_status")]
		STATUS,
	}
}

