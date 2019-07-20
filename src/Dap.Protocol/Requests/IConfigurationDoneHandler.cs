using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(RequestNames.ConfigurationDone)]
    public interface IConfigurationDoneHandler : IJsonRpcRequestHandler<ConfigurationDoneArguments, ConfigurationDoneResponse> { }

    public abstract class ConfigurationDoneHandler : IConfigurationDoneHandler
    {
        public abstract Task<ConfigurationDoneResponse> Handle(ConfigurationDoneArguments request, CancellationToken cancellationToken);
    }

    public static class ConfigurationDoneHandlerExtensions
    {
        public static IDisposable OnConfigurationDone(this IDebugAdapterRegistry registry, Func<ConfigurationDoneArguments, CancellationToken, Task<ConfigurationDoneResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : ConfigurationDoneHandler
        {
            private readonly Func<ConfigurationDoneArguments, CancellationToken, Task<ConfigurationDoneResponse>> _handler;

            public DelegatingHandler(Func<ConfigurationDoneArguments, CancellationToken, Task<ConfigurationDoneResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<ConfigurationDoneResponse> Handle(ConfigurationDoneArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
