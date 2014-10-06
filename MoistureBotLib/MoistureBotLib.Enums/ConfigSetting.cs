using System;

namespace MoistureBot
{
	public enum ConfigSetting
	{
		[SectionAttribute("bot_settings")]
		[KeyAttribute("status")]
		STATUS,
	}
}

