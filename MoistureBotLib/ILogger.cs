using System;
using Mono.Addins;

namespace MoistureBot
{

    public interface ILogger
    {
        void Debug(string message);

        void Info(string message);

        void Warn(string message);

        void Error(string message, Exception e);

        void Fatal(string message);
    }
}

