using System;
using System.Collections.Generic;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IFallbackServiceProvider : IServiceProvider
    {
        void Add<T>(T item);
        void Add(Type type, object item);
    }
    internal class FallbackServiceProvider : IFallbackServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceProvider _fallbackServiceProvider;
        private readonly IDictionary<Type, object> _wellKnownServices;

        public FallbackServiceProvider(IServiceProvider serviceProvider, IServiceProvider fallbackServiceProvider)
        {
            _serviceProvider = serviceProvider;
            _fallbackServiceProvider = fallbackServiceProvider;
            _wellKnownServices = new Dictionary<Type, object>();
        }

        public object GetService(Type serviceType) => _wellKnownServices.TryGetValue(serviceType, out var result) ? result : _serviceProvider.GetService(serviceType) ?? _fallbackServiceProvider?.GetService(serviceType);
        public void Add<T>(T item) => _wellKnownServices.Add(typeof(T), item);
        public void Add(Type type, object item) => _wellKnownServices.Add(type, item);
    }
}
