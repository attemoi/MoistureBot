using System;
using MoistureBot;
using Mono.Addins;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using MoistureBot.Extensions;

namespace MoistureBot
{

    /// <summary>
    /// A central part of MoistureBot's core functionality.
    /// </summary>
    public class MoistureBotContext : IContext
    {

        private IMoistureBot Bot;
        private IConfig Config;
        private ILogger Logger;

        public MoistureBotContext()
        {
            Logger = GetLogger(typeof(MoistureBotContext));
        }

        public IMoistureBot GetBot()
        {
            if (Bot == null)
            {
                try
                {
                    Bot = new MoistureBotCore(this);
                    return Bot;
                }
                catch(Exception e)
                {
                    Logger.Error("Failed to instantiate MoistureBot core.",e);
                }
            }

            return Bot;
       
        }

        public ILogger GetLogger(Type type)
        {
            return new MoistureBotLogger(type);
        }

        public IConfig GetConfig()
        {
            if (Config == null)
            {
                try
                {
                    Config = new MoistureBotConfig(this, new IniParser.FileIniDataParser());
                    return Config;
                }
                catch(Exception e)
                {
                    Logger.Error("Failed to instantiate MoistureBot configurator.",e);
                }
                    
            }

            return Config;
        }

        public bool InvokeAddins<AddinType>(Action<AddinType> onNext)
        {
            String path = "/MoistureBot/" + typeof(AddinType).Name;
            return InvokeAddins<AddinType>(path,onNext);
        }

        public bool InvokeAddins<AddinType>(string path, Action<AddinType> onNext)
        {
            return InvokeAddins<AddinType, MoistureBotExtensionNode>(path,node => true,onNext);
        }

        public bool InvokeAddins<AddinType, NodeType>(string path, Func<NodeType, bool> predicate, Action<AddinType> onNext)
        {

            bool invoked = false;

            try
            {
                
                foreach (var node in AddinManager.GetExtensionNodes(path, typeof(NodeType)))
                {
                    if (predicate.Invoke((NodeType)node))
                    {
                        var moistureBotNode = node as MoistureBotExtensionNode;
                        if (typeof(AddinType).IsAssignableFrom(moistureBotNode.Type))
                        {
                            invoked = true;
                            Object addin = ((MoistureBotExtensionNode)node).GetInstance();
                            try
                            {
                                onNext.Invoke((AddinType)addin);
                            }
                            catch(Exception e)
                            {
                                Logger.Error("Error when invoking instance of " + addin.GetType().Name + ": " + e.Message,e);
                            }
                        }
                    }
                }

            }
            catch(Exception e)
            {
                Logger.Error("Error while trying to invoke addins",e);
            }

            return invoked;

        }

    }
}

