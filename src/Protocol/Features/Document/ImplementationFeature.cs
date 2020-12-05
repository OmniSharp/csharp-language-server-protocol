using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(TextDocumentNames.Implementation, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(ImplementationRegistrationOptions)), Capability(typeof(ImplementationCapability))]
        public partial record ImplementationParams : TextDocumentPositionParams, IWorkDoneProgressParams, IPartialItemsRequest<LocationOrLocationLinks, LocationOrLocationLink> { }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.ImplementationProvider))]
        [RegistrationName(TextDocumentNames.Implementation)]
        public partial class ImplementationRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions, IStaticRegistrationOptions { }

    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.Implementation))]
        public partial class ImplementationCapability : LinkSupportCapability, ConnectedCapability<IImplementationHandler>
        {
        }
    }

    namespace Document
    {
    }
}
