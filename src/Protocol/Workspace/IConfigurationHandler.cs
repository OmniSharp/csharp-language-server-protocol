using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
    [Parallel, Method(WorkspaceNames.Configuration, Direction.ServerToClient)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(IWorkspaceLanguageServer), typeof(ILanguageServer))]
    public interface IConfigurationHandler : IJsonRpcRequestHandler<ConfigurationParams, Container<JToken>> { }

    public abstract class ConfigurationHandler : IConfigurationHandler
    {
        public abstract Task<Container<JToken>> Handle(ConfigurationParams request, CancellationToken cancellationToken);
    }
}
