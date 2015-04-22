using System;
using MoistureBot;

namespace MoistureBot
{
    public interface IContext
    {
        void InvokeAddins<AddinType>(Action<AddinType> onNext);

        void InvokeAddins<AddinType>(string path, Action<AddinType> onNext);

        T GetInstanceWithContext<T>(Type type);

        IMoistureBot GetBot();
        IConfig GetConfig();
        ILogger GetLogger(Type type);

    }
}

