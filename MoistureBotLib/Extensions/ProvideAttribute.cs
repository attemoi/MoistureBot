using System;

namespace MoistureBot.Extensions
{

    /// <summary>
    /// Attribute used to provide an instance of <see cref="IContext"/> for an addin through constructor injection.
    /// </summary>
    [System.AttributeUsage(
        System.AttributeTargets.Constructor,
        AllowMultiple = false)]  
    public class ProvideAttribute : System.Attribute
    {
    }
}

