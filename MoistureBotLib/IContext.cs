using System;
using MoistureBot;
using Mono.Addins;

namespace MoistureBot
{
    /// <summary>
    /// Interface for accessing application-specific resources and classes, and provides methods for invoking addins. 
    /// The implementation will be provided by the core application.
    /// </summary>
    /// <remarks> 
    /// An instance of <see cref="IContext"/> can be passed to addins by using the [<see cref="Provide"/>] attribute with a single parameter constructor.
    /// </remarks>
    public interface IContext
    {

        /// <summary>
        /// Invokes all addins that are registered to extend the interface defined by AddinType. 
        /// Extension point path will be set to "MoistureBot/AddinTypeName", where AddinTypeName is the class name of AddinType.
        /// </summary>
        /// <returns><c>true</c>, if one or more addins were invoked, <c>false</c> otherwise.</returns>
        /// <param name="onNext">Action to be executed for each addin.</param>
        /// <typeparam name="AddinType">Type of the extensions.</typeparam>
        bool InvokeAddins<AddinType>(Action<AddinType> onNext);
      
        /// <summary>
        /// Invokes the addins in the given path and extension type.
        /// </summary>
        /// <returns><c>true</c>, if one or more addins were invoked, <c>false</c> otherwise.</returns>
        /// <param name="path">Path of extension point.</param>
        /// <param name="onNext">Action to be executed for each addin.</param>
        /// <typeparam name="AddinType">Type of the extensions.</typeparam>
        bool InvokeAddins<AddinType>(string path, Action<AddinType> onNext);

        bool InvokeAddins<AddinType, NodeType>(string path, Func<NodeType, bool> predicate, Action<AddinType> onNext);

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

