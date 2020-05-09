using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    [Parallel, Method(WorkspaceNames.WorkspaceConfiguration)]
    public interface IConfigurationHandler : IJsonRpcRequestHandler<ConfigurationParams, Container<JToken>> { }

    public abstract class ConfigurationHandler : IConfigurationHandler
    {
        public abstract Task<Container<JToken>> Handle(ConfigurationParams request, CancellationToken cancellationToken);
    }

    public static class ConfigurationHandlerExtensions
    {
        public static IDisposable OnConfiguration(this ILanguageClientRegistry registry, Func<ConfigurationParams, Task<Container<JToken>>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : ConfigurationHandler
        {
            private readonly Func<ConfigurationParams, Task<Container<JToken>>> _handler;

            public DelegatingHandler(Func<ConfigurationParams, Task<Container<JToken>>> handler)
            {
                _handler = handler;
            }

            public override Task<Container<JToken>> Handle(ConfigurationParams request, CancellationToken cancellationToken) => _handler.Invoke(request);
        }
    }
}
