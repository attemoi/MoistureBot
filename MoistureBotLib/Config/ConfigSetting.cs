using System;

namespace MoistureBot.Config
{
    public enum ConfigSetting
    {
        [Section("bot_settings")]
        [Key("online_status")]
        [DefaultValue("online")]
        STATUS,

        [Section("steam")]
        [Key("cell_id")]
        [DefaultValue("0")]
        CELL_ID,
    }
}

