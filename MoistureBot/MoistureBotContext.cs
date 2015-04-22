using System;
using MoistureBot;
using Mono.Addins;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace MoistureBot
{
    public class MoistureBotContext : IContext
    {

        private IMoistureBot Bot;
        private IConfig Config;
        private ILogger Logger;

        public MoistureBotContext() {
            Logger = GetLogger(typeof(MoistureBotContext));
        }

        public IMoistureBot GetBot()
        {
            if (Bot == null)
                Bot = new MoistureBotCore(this);
            return Bot;
        }

        public ILogger GetLogger(Type type)
        {
            return new MoistureBotLogger(type);
        }

        public IConfig GetConfig()
        {
            if (Config == null)
                Config = new MoistureBotConfig(this);
            return Config;
        }

        public void InvokeAddins<AddinType>(Action<AddinType> onNext)
        {
            String path = "/MoistureBot/" + typeof(AddinType).Name;
            InvokeAddins<AddinType>(path, onNext);
        }

        public void InvokeAddins<AddinType>(string path, Action<AddinType> onNext)
        {
            InvokeAddins<AddinType, TypeExtensionNode>(path, node => true, onNext);
        }
            
        public void InvokeAddins<AddinType, NodeType>(string path, Func<NodeType, bool> predicate, Action<AddinType> onNext )
        {

            foreach (var node in AddinManager.GetExtensionNodes(path))
            {
                var typeNode = node as TypeExtensionNode;
                if (typeNode != null && typeof(AddinType).IsAssignableFrom(typeNode.Type) && predicate.Invoke((NodeType)node) == true)
                {
                    AddinType addin = GetInstanceWithContext<AddinType>(typeNode.Type);

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
    
        public T GetInstanceWithContext<T>(Type type)
        {

            // Check if addin has a constructor with [Provide] a attribute.
            ConstructorInfo ctor = type
                .GetConstructors()
                .Where(c => c.IsDefined(typeof(ProvideAttribute), false))
                .SingleOrDefault();

            // If constructor was found, inject context object.
            if (ctor != null)
            {
                var parameters = ctor.GetParameters();

                var values = new List<object>();
                foreach (var parameter in parameters)
                {
                    if (parameter.ParameterType == typeof(IContext))
                    {
                        values.Add(this);  
                    }
                    else
                    {
                        values.Add(default(T));
                    }
                         
                }

                return (T)ctor.Invoke(values.ToArray());
            }
            else
            {
                return (T)Activator.CreateInstance<T>();
            }
        }

    }
}

