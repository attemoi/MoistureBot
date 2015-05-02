using System;
using Mono.Addins;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace MoistureBot.Extensions
{
    public class MoistureBotExtensionNode : TypeExtensionNode
    {

        /// <summary>
        /// Creates a new extension object.
        /// </summary>
        /// <returns>
        /// The extension object
        /// </returns>
        public override object CreateInstance ()
        {
            // Check if addin has a constructor with [Provide] attribute.
            ConstructorInfo ctor = Type
                .GetConstructors()
                .Where(c => c.IsDefined(typeof(ProvideAttribute), false))
                .SingleOrDefault();

            // If constructor was found, inject context object.
            if (ctor != null)
            {
                var parameters = ctor.GetParameters();

                var values = new List<object>();
                foreach (var parameter in parameters)
                {
                    
                    if (parameter.ParameterType == typeof(IContext))
                    {
                        values.Add(MoistureBotAddinManager.Context);  
                    }
                    else
                    {
                        values.Add(default(Type));
                    }     
                }

                return ctor.Invoke(values.ToArray());

            }
            else
            {
                return Activator.CreateInstance(Type);
            }
                

        }
    }
        
}

