using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(WorkspaceNames.Configuration, Direction.ServerToClient)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace"), GenerateHandlerMethods, GenerateRequestMethods(typeof(IWorkspaceLanguageServer), typeof(ILanguageServer))]
        public partial record ConfigurationParams : IRequest<Container<JToken>>
        {
            public Container<ConfigurationItem> Items { get; init; }
        }

        public record ConfigurationItem
        {
            [Optional] public DocumentUri? ScopeUri { get; init; }
            [Optional] public string? Section { get; init; }
        }
    }
}
