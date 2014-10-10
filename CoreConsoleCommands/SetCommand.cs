using System;
using Mono.Options;
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
	public class SetCommand : IConsoleCommand
	{

		private IConfig Config = MoistureBotComponentProvider.GetConfig();
		private ILogger Logger = MoistureBotComponentProvider.GetLogger();
		private IMoistureBot Bot = MoistureBotComponentProvider.GetBot();

		private string status;
		private string name;

		public OptionSet Options {
			get 
			{
				return new OptionSet () {
					{ "s=|status=", "Bot online status: online, offline, away, busy, looking_to_play, looking_to_trade, snooze", 
						v => status = v },
					{ "n=|name=", "Bot persona name.", 
						v => name = v }
					};
			}
		}

		public bool Execute(string[] args)
		{
			Logger.Info("Executing command...");

			var extra = Options.Parse(args);

			if (extra.Count > 0)
			{
				Console.WriteLine(ConsoleMessage.InvalidNumberOfParameters("set"));
			}

			if (!String.IsNullOrEmpty(status))
			{
				try
				{
					Bot.SetOnlineStatus(status);
					Console.WriteLine("Online status set to '" + Config.GetSetting(ConfigSetting.STATUS) + "'");
				}
				catch(ArgumentException)
				{
					Console.WriteLine("Invalid value for status. Allowed values: online, offline, away, busy, looking_to_play, looking_to_trade, snooze");
				}
				catch(Exception e)
				{
					Logger.Error("Error setting online status: ",e);
					Console.WriteLine("Failed to set online status, see log files for more details");
				}
			}

			if (!String.IsNullOrEmpty(name))
			{
				try
				{
					Bot.PersonaName = name;
					Console.WriteLine("Online status set to '" + Config.GetSetting(ConfigSetting.STATUS) + "'");
				}
				catch(ArgumentException)
				{
					Console.WriteLine("Invalid value for status. Allowed values: online, offline, away, busy, looking_to_play, looking_to_trade, snooze");
				}
				catch(Exception e)
				{
					Logger.Error("Error setting persona name: ",e);
					Console.WriteLine("Failed to set persona name, see log files for more details");
				}
			}

			return false;
		}
	}
}

