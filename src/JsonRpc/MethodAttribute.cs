using System.Reflection;
using MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method)]
    public class MethodAttribute : Attribute
    {
        public string Method { get; }
        public Direction Direction { get; }

        public MethodAttribute(string method) => Method = method;

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

        public static MethodAttribute? From(Type? type) => AllFrom(type).FirstOrDefault();

        public static IEnumerable<MethodAttribute> AllFrom(Type? type) =>
            CollectMethodAttributes(type)
               .Concat(
                    type
                      ?.GetInterfaces()
                       .SelectMany(CollectMethodAttributes)
                 ?? Enumerable.Empty<MethodAttribute>()
                );

        private static IEnumerable<MethodAttribute> CollectMethodAttributes(Type? type)
        {
            if (type == null) return Enumerable.Empty<MethodAttribute>();
            if (type.IsGenericType && typeof(IRequestHandler<,>) == type.GetGenericTypeDefinition())
            {
                return type.GetTypeInfo().GetCustomAttributes<MethodAttribute>(true).Concat(AllFrom(type.GetGenericArguments()[0]));
            }

            return type.GetTypeInfo().GetCustomAttributes<MethodAttribute>(true);
        }
    }
}
