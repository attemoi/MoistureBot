using System;
using MoistureBot;
using Mono.Addins;
using System.Linq;
using MoistureBot.ExtensionPoints;

namespace MoistureBot
{

    /// <summary>
    /// Factory for providing object instances. Ideally this should be done using constructor injection
    /// but I haven't figured out a proper way to implement that when using Mono.Addins.
    /// </summary>

    public class MoistureBotFactory
    {

        private static IMoistureBot bot;
        private static IConfig config;

        /// <summary>
        /// Gets an instance of a logger.
        /// </summary>
        /// <returns>The logger instance</returns>
        /// <param name="type">Type of caller.</param>
        /// <example>
        ///     ILogger Logger = new MoistureBotComponentFactory().GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// </example>
        public ILogger GetLogger(Type type)
        {
            return new MoistureBotLogger(type);
        }

        public IMoistureBot GetBot()
        {
            if (bot == null)
                bot = new MoistureBotCore();
            return bot;
        }

        public IConfig GetConfig()
        {
            if (config == null)
                config = new MoistureBotConfig();
            return config;
        }
    }
}

