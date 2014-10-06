using System;
using Mono.Options;
using MoistureBot.Commands;
using MoistureBot.Config;

namespace MoistureBot
{
	public class SetCommand : ICommand
	{

		private static readonly log4net.ILog log = log4net.LogManager.GetLogger
			(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public string[] Args { get; set; }

		private OptionSet options;

		private bool help;
		private string status;

		public SetCommand() {
			options = new OptionSet () {
				{ "h|help", "show this message", 
					v => help = v != null },
				{ "s=|status=", "Bot online status. Allowed values: online, offline, away, busy, looking_to_play, looking_to_trade, snooze", 
					v => status = v }
			};
		}

		public void WriteHelp() {
			ConsoleUtils.WriteHelp(
				"Configure bot", 
				"set [OPTIONS]+",
				options);
		}

		public bool Execute(IMoistureBot bot)
		{
			log.Debug ("Executing command...");

			var extra = options.Parse (Args);

			if (help) {
				WriteHelp ();
				return false;
			}

			if (extra.Count > 0) {
				log.Info ("This command does not allow extra parameters. Type 'set -h' for help.");
			}

			if (!String.IsNullOrEmpty (status)) {
				try {
					bot.SetOnlineStatus (status);
					log.Info("Online status set to '" + new MoistureBotConfig().GetSetting(ConfigSetting.STATUS) + "'");
				} catch (ArgumentException) {
					log.Error ("Invalid value for status. Allowed values: online, offline, away, busy, looking_to_play, looking_to_trade, snooze");
				} catch (Exception e) {
					log.Error ("Error setting online status: ", e);
				}
			}

		return false;
		}
	}
}

