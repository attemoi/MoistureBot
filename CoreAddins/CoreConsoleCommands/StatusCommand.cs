using System;
using Mono.Options;
using System.Collections.Generic;
using Mono.Addins;
using System.Linq;
using MoistureBot.ExtensionPoints;
using MoistureBot.ExtensionAttributes;
using MoistureBot.Config;

namespace MoistureBot.ConsoleCommands
{
	[ConsoleCommand(
		Name = "status",
		Description = "Show bot status.",
		ShortDescription = "Show bot status.",
		ShortUsage = "status [OPTIONS]+",
		Usage = "status [OPTIONS]+"
	)]
	public class StatusCommand : IConsoleCommand
	{

		private IConfig Config = MoistureBotComponentProvider.GetConfig();
		private ILogger Logger = MoistureBotComponentProvider.GetLogger();
		private IMoistureBot Bot = MoistureBotComponentProvider.GetBot();

		private bool showAddins;

		public OptionSet Options
		{
			get
			{
				return new OptionSet() {
					{ "a|addins", "Show activated addins.", 
						v => showAddins = v != null}
					};
			}
		}

		public bool Execute(string[] args)
		{

			Logger.Info("Executing command...");

			Options.Parse(args);

			Console.WriteLine();
			string status = ConfigUtils.GetValue<NameAttribute>(Bot.GetOnlineStatus());
			if (Bot.IsConnected())
			{
				Console.WriteLine("  logged on as {0}",Bot.Username);
				Console.WriteLine("  persona name: '{0}'",Bot.PersonaName);
				Console.WriteLine("  online status: {0}",status);
			}
			else
			{
				Console.WriteLine("  not connected");
				Console.WriteLine("  online status set to {0}",status);
			}

			if (showAddins)
			{
				Console.WriteLine();
				Console.WriteLine("Registered addins:");
				Console.WriteLine();
				var addins = AddinManager.Registry.GetAddins();
				if (addins.Length == 0)
				{
					Console.WriteLine("  -- no addins registered --");
				}
				else
				{
					foreach (Addin addin in addins)
					{
						Console.WriteLine("  {0} {1}",addin.Name,addin.Version);
					}
				}
			}
				
			return false;
		}
	}
}

