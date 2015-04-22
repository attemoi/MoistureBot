using System;
using Mono.Addins;
using MoistureBot.ExtensionPoints;

namespace MoistureBot
{

    public class MoistureBotLogger : ILogger
    {
        private log4net.ILog log;

        public MoistureBotLogger(Type type) {
            this.log = log4net.LogManager.GetLogger(type /*System.Reflection.MethodBase.GetCurrentMethod().DeclaringType*/);
        }
	
        #region ILogger implementation

        public void Debug(string message)
        {
            log.Debug(message);
        }

        public void Info(string message)
        {
            log.Info(message);
        }

        public void Warn(string message)
        {
            log.Warn(message);
        }

        public void Error(string message, Exception e)
        {
            log.Error(message, e);
        }

        public void Fatal(string message)
        {
            log.Fatal(message);
        }

        #endregion

    }
}

