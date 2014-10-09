using System;
using MoistureBot;
using Mono.Addins;
using System.Linq;

namespace MoistureBot
{
	public static class MoistureBotComponentProvider
	{
		public static ILogger GetLogger()
		{
			return AddinManager.GetExtensionObjects<ILogger>(true).First();
		}

		public static IMoistureBot GetBot()
		{
			return AddinManager.GetExtensionObjects<IMoistureBot>(true).First();
		}

		public static IConfig GetConfig()
		{
			return AddinManager.GetExtensionObjects<IConfig>(true).First();
		}
	}
}

