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
        [Method(TextDocumentNames.Declaration, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(DeclarationRegistrationOptions))]
        [Capability(typeof(DeclarationCapability))]
        public partial record DeclarationParams : TextDocumentPositionParams, IWorkDoneProgressParams,
                                                  IPartialItemsRequest<LocationOrLocationLinks?, LocationOrLocationLink>;

        [GenerateRegistrationOptions(nameof(ServerCapabilities.DeclarationProvider))]
        [RegistrationName(TextDocumentNames.Declaration)]
        public partial class DeclarationRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions, IStaticRegistrationOptions
        {
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.Declaration))]
        public partial class DeclarationCapability : LinkSupportCapability
        {
        }
    }

    namespace Document
    {
    }
}
