using System;

namespace OmniSharp.Extensions.JsonRpc
{
    class ExternalServiceProvider : IExternalServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public ExternalServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object GetService(Type serviceType) => _serviceProvider.GetService(serviceType);
    }
}