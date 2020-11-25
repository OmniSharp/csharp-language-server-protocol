using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(TextDocumentNames.References, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document", Name = "References"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(ReferenceRegistrationOptions)), Capability(typeof(ReferenceCapability))]
        public partial class ReferenceParams : TextDocumentPositionParams, IWorkDoneProgressParams, IPartialItemsRequest<LocationContainer, Location>
        {
            public ReferenceContext Context { get; set; } = null!;
        }
        public class ReferenceContext
        {
            /// <summary>
            /// Include the declaration of the current symbol.
            /// </summary>
            public bool IncludeDeclaration { get; set; }
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.ReferencesProvider))]
        public partial class ReferenceRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions { }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.References))]
        public partial class ReferenceCapability : DynamicCapability, ConnectedCapability<IReferencesHandler>
        {
        }
    }

    namespace Document
    {
    }
}
