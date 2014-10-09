using System;
using Mono.Options;
using MoistureBot;
using MoistureBot.Config;
using Mono.Addins;
using System.Linq;

namespace MoistureBot
{

	[ConsoleCommand(
		Name = "set",
		Description = "Configure Bot.",
		ShortDescription = "Configure Bot.",
		ShortUsage = "set [OPTIONS]+",
		Usage = "set [OPTIONS]+"
	)]
	public class SetCommand : ICommand
	{

		private IConfig Config = AddinManager.GetExtensionObjects<IConfig> ().First ();
		private ILogger Logger = AddinManager.GetExtensionObjects<ILogger> ().First ();
		private IMoistureBot Bot = AddinManager.GetExtensionObjects<IMoistureBot> ().First ();

		private string status;

		public OptionSet Options {
			get {
				return new OptionSet () {
					{ "s=|status=", "Bot online status. Allowed values: online, offline, away, busy, looking_to_play, looking_to_trade, snooze", 
						v => status = v }
				};
			}
		}

		public bool Execute(string[] args)
		{
			Logger.Info ("Executing command...");

			var extra = Options.Parse (args);

			if (extra.Count > 0) {
				Console.WriteLine(ConsoleMessage.InvalidNumberOfParameters("set"));
			}

			if (!String.IsNullOrEmpty (status)) {
				try {
					Bot.SetOnlineStatus (status);
					Console.WriteLine("Online status set to '" + Config.GetSetting(ConfigSetting.STATUS) + "'");
				} catch (ArgumentException) {
					Console.WriteLine("Invalid value for status. Allowed values: online, offline, away, busy, looking_to_play, looking_to_trade, snooze");
				} catch (Exception e) {
					Logger.Error ("Error setting online status: ", e);
					Console.WriteLine ("Failed to set online status, see log files for more details");
				}
			}

		return false;
		}
	}
}

