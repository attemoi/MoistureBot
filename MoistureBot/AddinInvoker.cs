using System;
using Mono.Addins;
using MoistureBot.ExtensionPoints;
using System.Reflection;
using System.Linq;

namespace MoistureBot
{
    public class AddinInvoker
    {

        ILogger Logger;

        public AddinInvoker(ILogger logger)
        {
            this.Logger = logger;
        }

        public void invoke<AddinType>(Action<AddinType> onNext)
        {
            String path = "/MoistureBot/" + typeof(AddinType).Name;
            invoke(path, onNext);
        }

        public void invoke<AddinType>(string path, Action<AddinType> onNext)
        {
            foreach (AddinType addin in AddinManager.GetExtensionObjects<AddinType>(path))
            {
                try
                {
                    onNext.Invoke(addin);
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to invoke " + addin.GetType().Name, e);
                }
            }
        }
			
    }
}

