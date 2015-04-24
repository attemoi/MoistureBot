using System;
using Mono.Addins;

namespace MoistureBot
{

    /// <summary>
    /// Interface to application logging. This class will be available through the <see cref="IContext"/> interface.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log a message object with DEBUG level.
        /// </summary>
        void Debug(string message);

        /// <summary>
        /// Log a message object with INFO level.
        /// </summary>
        void Info(string message);

        /// <summary>
        /// Log a message object with WARN level.
        /// </summary>
        void Warn(string message);

        /// <summary>
        /// Log a message object with ERROR level including the stack trace of the Exception passed as a parameter.
        /// </summary>
        void Error(string message, Exception e);

        /// <summary>
        /// Log a message object with FATAL level.
        /// </summary>
        void Fatal(string message);
    }
}

