using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    [Parallel, Method(WorkspaceNames.WorkspaceConfiguration)]
    public interface IConfigurationHandler : IJsonRpcRequestHandler<ConfigurationParams, Container<JsonElement>> { }

    public abstract class ConfigurationHandler : IConfigurationHandler
    {
        public abstract Task<Container<JsonElement>> Handle(ConfigurationParams request, CancellationToken cancellationToken);
    }

    public static class ConfigurationHandlerExtensions
    {
        public static IDisposable OnConfiguration(this ILanguageClientRegistry registry, Func<ConfigurationParams, Task<Container<JsonElement>>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : ConfigurationHandler
        {
            private readonly Func<ConfigurationParams, Task<Container<JsonElement>>> _handler;

            public DelegatingHandler(Func<ConfigurationParams, Task<Container<JsonElement>>> handler)
            {
                _handler = handler;
            }

            public override Task<Container<JsonElement>> Handle(ConfigurationParams request, CancellationToken cancellationToken) => _handler.Invoke(request);
        }
    }
}
