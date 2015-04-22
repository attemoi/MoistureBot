using System;

namespace MoistureBot
{
    [System.AttributeUsage(
        System.AttributeTargets.Constructor,
        AllowMultiple = false)]
    public class ProvideAttribute : System.Attribute
    {
    }
}

