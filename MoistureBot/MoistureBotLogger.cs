using System;
using Mono.Addins;
using MoistureBot;

namespace MoistureBot
{

    public class MoistureBotLogger : ILogger
    {
        private log4net.ILog Log;

        public MoistureBotLogger(Type type) {
            this.Log = log4net.LogManager.GetLogger(type);
        }
	
        #region ILogger implementation

        public void Debug(string message)
        {
            Log.Debug(message);
        }

        public void Info(string message)
        {
            Log.Info(message);
        }

        public void Warn(string message)
        {
            Log.Warn(message);
        }

        public void Error(string message, Exception e)
        {
            Log.Error(message, e);
        }

        public void Fatal(string message)
        {
            Log.Fatal(message);
        }

        #endregion

    }
}

