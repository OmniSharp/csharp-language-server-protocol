using System.Collections.Concurrent;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Shared
{
    public class RequestProcessIdentifier : IRequestProcessIdentifier
    {
        private readonly ConcurrentDictionary<Type, RequestProcessType> _cache =
            new ConcurrentDictionary<Type, RequestProcessType>();

        private readonly RequestProcessType _defaultRequestProcessType;

        public RequestProcessIdentifier(RequestProcessType defaultRequestProcessType = RequestProcessType.Serial) => _defaultRequestProcessType = defaultRequestProcessType;

        public RequestProcessType Identify(IHandlerDescriptor descriptor)
        {
            if (descriptor.RequestProcessType.HasValue)
            {
                return descriptor.RequestProcessType.Value;
            }

            if (_cache.TryGetValue(descriptor.HandlerType, out var type)) return type;

            type = _defaultRequestProcessType;
            var processAttribute = descriptor.ImplementationType
                                             .GetCustomAttributes(true)
                                             .Concat(descriptor.HandlerType.GetCustomAttributes(true))
                                             .OfType<ProcessAttribute>()
                                             .FirstOrDefault();
            if (processAttribute != null)
            {
                type = processAttribute.Type;
            }


            _cache.TryAdd(descriptor.HandlerType, type);

            return type;
        }
    }
}
