using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OmniSharp.Extensions.JsonRpc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method)]
    public class MethodAttribute : Attribute
    {
        public string Method { get; }
        public Direction Direction { get; }

        public MethodAttribute(string method)
        {
            Method = method;
        }

        /// <summary>
        /// Decorate the method given the given direction
        /// </summary>
        /// <param name="method"></param>
        /// <param name="direction"></param>
        public MethodAttribute(string method, Direction direction)
        {
            Method = method;
            Direction = direction;
        }

        public static MethodAttribute From(Type type)
        {
            var attribute = type.GetCustomAttribute<MethodAttribute>(true);
            if (attribute is null)
            {
                attribute = type
                    .GetInterfaces()
                    .Select(t => t.GetTypeInfo().GetCustomAttribute<MethodAttribute>(true))
                    .FirstOrDefault(x => x != null);
            }

            return attribute;
        }

        public static IEnumerable<MethodAttribute> AllFrom(Type type)
        {
            return type.GetCustomAttributes<MethodAttribute>(true)
                .Concat(type
                    .GetInterfaces()
                    .SelectMany(t => t.GetTypeInfo().GetCustomAttributes<MethodAttribute>(true)));
        }
    }
}
