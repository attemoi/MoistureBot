using System;
using Mono.Addins;
using System.Linq;

namespace MoistureBot
{
    public class AddinUtils
    {
        public static Addin getAddinRoot(String name)
        {
            return AddinManager.Registry.GetAddinRoots().FirstOrDefault(addin => addin.Name.Equals(name));
        }
			
    }
}

