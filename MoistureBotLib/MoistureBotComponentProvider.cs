using System;
using MoistureBot;
using Mono.Addins;
using System.Linq;
using MoistureBot.ExtensionPoints;

namespace MoistureBot
{

    // TODO: handle errors

    public static class MoistureBotComponentProvider
    {
        public static ILogger GetLogger()
        {
            return AddinManager.GetExtensionObjects<ILogger>().First();
        }

        public static IMoistureBot GetBot()
        {
            return AddinManager.GetExtensionObjects<IMoistureBot>().First();
        }

        public static IConfig GetConfig()
        {
            return AddinManager.GetExtensionObjects<IConfig>().First();
        }
    }
}

