using MediatR;
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
        [Method(TextDocumentNames.RangeFormatting, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(DocumentRangeFormattingRegistrationOptions)), Capability(typeof(DocumentRangeFormattingCapability))]
        public partial record DocumentRangeFormattingParams : ITextDocumentIdentifierParams, IRequest<TextEditContainer>, IWorkDoneProgressParams
        {
            /// <summary>
            /// The document to format.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; init; }

            /// <summary>
            /// The range to format
            /// </summary>
            public Range Range { get; init; }

            /// <summary>
            /// The format options
            /// </summary>
            public FormattingOptions Options { get; init; }
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.DocumentRangeFormattingProvider))]
        [RegistrationName(TextDocumentNames.RangeFormatting)]
        public partial class DocumentRangeFormattingRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions
        {
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.RangeFormatting))]
        public partial class DocumentRangeFormattingCapability : DynamicCapability
        {
        }
    }

    namespace Document
    {
    }
}
