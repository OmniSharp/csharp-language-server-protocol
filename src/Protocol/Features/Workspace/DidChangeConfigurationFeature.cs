using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(WorkspaceNames.DidChangeConfiguration, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace"), GenerateHandlerMethods, GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))]
        [Capability(typeof(DidChangeConfigurationCapability))]
        public partial record DidChangeConfigurationParams : IRequest<Unit>
        {
            /// <summary>
            /// The actual changed settings
            /// </summary>
            public JToken? Settings { get; init; }
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.Workspace), nameof(WorkspaceClientCapabilities.Configuration))]
        public partial class DidChangeConfigurationCapability : DynamicCapability
        {
        }
    }
}
