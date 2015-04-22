using System;
using Mono.Addins;

namespace MoistureBot
{
    /// <summary>
    /// Extension point for initializing addins on program startup.
    /// </summary>
    [TypeExtensionPoint("/MoistureBot/IStartupCommand")]
    public interface IStartupCommand
    {
        /// <summary>
        /// Called when the program has started
        /// </summary>
        void ProgramStarted();
    }
		
}
