using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public class RequestProcessIdentifier : IRequestProcessIdentifier
    {
        private readonly ConcurrentDictionary<Type, RequestProcessType> _cache = new ConcurrentDictionary<Type, RequestProcessType>();
        private readonly RequestProcessType _defaultRequestProcessType;

        public RequestProcessIdentifier(RequestProcessType defaultRequestProcessType = RequestProcessType.Serial)
        {
            _defaultRequestProcessType = defaultRequestProcessType;
        }

        public RequestProcessType Identify(IHandlerDescriptor descriptor)
        {
            if (_cache.TryGetValue(descriptor.HandlerType, out var type)) return type;

            type = _defaultRequestProcessType;
            var handlerType = descriptor.Handler.GetType().GetTypeInfo();
            var processAttribute = handlerType
                .GetCustomAttributes(true)
                .Concat(descriptor.HandlerType.GetTypeInfo().GetCustomAttributes(true))
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
