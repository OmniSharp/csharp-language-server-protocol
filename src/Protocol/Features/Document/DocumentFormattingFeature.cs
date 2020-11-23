using System.Threading;
using System.Threading.Tasks;
using MediatR;
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
        [Method(TextDocumentNames.DocumentFormatting, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(DocumentFormattingRegistrationOptions)), Capability(typeof(DocumentFormattingCapability))]
        public partial class DocumentFormattingParams : ITextDocumentIdentifierParams, IRequest<TextEditContainer?>, IWorkDoneProgressParams
        {
            /// <summary>
            /// The document to format.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; set; } = null!;

            /// <summary>
            /// The format options.
            /// </summary>
            public FormattingOptions Options { get; set; } = null!;
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.DocumentFormattingProvider))]
        public partial class DocumentFormattingRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions { }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.Formatting))]
        public partial class DocumentFormattingCapability : DynamicCapability, ConnectedCapability<IDocumentFormattingHandler>
        {
        }
    }

    namespace Document
    {
    }
}
