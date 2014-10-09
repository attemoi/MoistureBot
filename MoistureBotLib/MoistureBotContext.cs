using System;
using MoistureBot.Config;
using Mono.Addins;
using System.Linq;

namespace MoistureBot
{
	public static class MoistureBotContext
	{
		public static ILogger GetLogger() {
			return AddinManager.GetExtensionObjects<ILogger> (false).First ();
		}

		public static IMoistureBot GetBot() {
			return AddinManager.GetExtensionObjects<IMoistureBot> (true).First ();
		}

		public static IConfig GetConfig() {
			return AddinManager.GetExtensionObjects<IConfig> (true).First ();
		}
	}
}

