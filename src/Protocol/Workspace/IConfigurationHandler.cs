using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
    [Parallel, Method(WorkspaceNames.Configuration, Direction.ServerToClient)]
    public interface IConfigurationHandler : IJsonRpcRequestHandler<ConfigurationParams, Container<JToken>> { }

    public abstract class ConfigurationHandler : IConfigurationHandler
    {
        public abstract Task<Container<JToken>> Handle(ConfigurationParams request, CancellationToken cancellationToken);
    }

    public static class ConfigurationExtensions
    {
        public static IDisposable OnConfiguration(
            this ILanguageClientRegistry registry,
            Func<ConfigurationParams, CancellationToken, Task<Container<JToken>>>
                handler)
        {
            return registry.AddHandler(WorkspaceNames.Configuration, RequestHandler.For(handler));
        }

        public static IDisposable OnConfiguration(
            this ILanguageClientRegistry registry,
            Func<ConfigurationParams, Task<Container<JToken>>> handler)
        {
            return registry.AddHandler(WorkspaceNames.Configuration, RequestHandler.For(handler));
        }

        public static Task<Container<JToken>> RequestConfiguration(this IWorkspaceLanguageServer router, ConfigurationParams @params, CancellationToken cancellationToken = default)
        {
            return router.SendRequest(@params, cancellationToken);
        }
    }
}
