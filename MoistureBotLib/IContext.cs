using System;
using MoistureBot;
using Mono.Addins;

namespace MoistureBot
{
    /// <summary>
    /// Interface to global information and components of MoistureBot. The implementation will be provided by the core application.
    /// <see cref="IContext"/> allows access to application-specific resources and classes, and provides methods for invoking addins. 
    /// </summary>
    /// <remarks> 
    /// An instance of <see cref="IContext"/> can be used within addins by defining a single parameter constructor with a [<see cref="Provide"/>] attribute.
    /// </remarks>
    public interface IContext
    {

        /// <summary>
        /// Invokes all addins that are registered to extend the interface defined by AddinType. 
        /// Extension point path will be set to "MoistureBot/AddinTypeName", where AddinTypeName is the class name of AddinType.
        /// </summary>
        /// <param name="onNext">Action to be executed for each addin.</param>
        /// <typeparam name="AddinType">Type of the extensions.</typeparam>
        void InvokeAddins<AddinType>(Action<AddinType> onNext);
      
        /// <summary>
        /// Invokes the addins in the given path and extension type.
        /// </summary>
        /// <param name="path">Path of extension point.</param>
        /// <param name="onNext">Action to be executed for each addin.</param>
        /// <typeparam name="AddinType">Type of the extensions.</typeparam>
        void InvokeAddins<AddinType>(string path, Action<AddinType> onNext);

        void InvokeAddins<AddinType, NodeType>(string path, Func<NodeType, bool> predicate, Action<AddinType> onNext);
       
        /// <summary>
        /// Creates an instance of an object injected with the context.
        /// </summary>
        /// <returns>The instance with the context injected.</returns>
        /// <param name="type">Type.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        T GetInstanceWithContext<T>(Type type);

        /// <summary>
        /// Gets an instance of <see cref="IMoistureBot"/> for managing actions related to the Steam client.
        /// </summary>
        IMoistureBot GetBot();

        /// <summary>
        /// Gets an instance of <see cref="IConfig"/> for managing configuration files.
        /// </summary>
        IConfig GetConfig();

        /// <summary>
        /// Gets an instance of <see cref="ILogger"/> that can be used for logging.
        /// </summary>
        ILogger GetLogger(Type type);

    }
}

