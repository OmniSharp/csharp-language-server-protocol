using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

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
            var handlerType = descriptor.ImplementationType;
            var processAttribute = descriptor.ImplementationType
                .GetCustomAttributes(true)
                .Concat(descriptor.HandlerType.GetCustomAttributes(true))
                .Concat(descriptor.ImplementationType.GetInterfaces().SelectMany(x => x.GetCustomAttributes()))
                .Concat(descriptor.HandlerType.GetInterfaces().SelectMany(x => x.GetCustomAttributes()))
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
