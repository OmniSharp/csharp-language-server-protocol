using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(TextDocumentNames.Definition, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(DefinitionRegistrationOptions)), Capability(typeof(DefinitionCapability))]
        public partial record DefinitionParams : TextDocumentPositionParams, IWorkDoneProgressParams, IPartialItemsRequest<LocationOrLocationLinks, LocationOrLocationLink>
        {
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.DefinitionProvider))]
        [RegistrationName(TextDocumentNames.Definition)]
        public partial class DefinitionRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions
        {
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.Definition))]
        public partial class DefinitionCapability : LinkSupportCapability
        {
        }
    }

    namespace Document
    {
    }
}
