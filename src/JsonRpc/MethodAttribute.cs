using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MediatR;

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
            return AllFrom(type).FirstOrDefault();
        }

        public static IEnumerable<MethodAttribute> AllFrom(Type type)
        {
            return CollectMethodAttributes(type)
                .Concat(type
                    .GetInterfaces()
                    .SelectMany(CollectMethodAttributes));
        }

        private static IEnumerable<MethodAttribute> CollectMethodAttributes(Type t)
        {
            if (t.IsGenericType && typeof(IRequestHandler<,>) == t.GetGenericTypeDefinition())
            {
                return t.GetTypeInfo().GetCustomAttributes<MethodAttribute>(true).Concat(AllFrom(t.GetGenericArguments()[0]));
            }

            return t.GetTypeInfo().GetCustomAttributes<MethodAttribute>(true);
        }
    }
}
