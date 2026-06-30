using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(TextDocumentNames.RangeFormatting, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(DocumentRangeFormattingRegistrationOptions))]
        [Capability(typeof(DocumentRangeFormattingCapability))]
        public partial record DocumentRangeFormattingParams : ITextDocumentIdentifierParams, IRequest<TextEditContainer>, IWorkDoneProgressParams
        {
            /// <summary>
            /// The document to format.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; init; } = null!;

            /// <summary>
            /// The range to format
            /// </summary>
            public Range Range { get; init; } = null!;

            /// <summary>
            /// The format options
            /// </summary>
            public FormattingOptions Options { get; init; } = null!;
        }

        [Parallel]
        [Method(TextDocumentNames.RangesFormatting, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document", Name = "DocumentRangesFormatting")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(DocumentRangeFormattingRegistrationOptions))]
        [Capability(typeof(DocumentRangeFormattingCapability))]
        public partial record DocumentRangesFormattingParams : ITextDocumentIdentifierParams, IRequest<TextEditContainer?>, IWorkDoneProgressParams
        {
            /// <summary>
            /// The document to format.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; init; } = null!;

            /// <summary>
            /// The ranges to format.
            /// </summary>
            public Container<Range> Ranges { get; init; } = null!;

            /// <summary>
            /// The format options.
            /// </summary>
            public FormattingOptions Options { get; init; } = null!;
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.DocumentRangeFormattingProvider))]
        [RegistrationName(TextDocumentNames.RangeFormatting)]
        public partial class DocumentRangeFormattingRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions
        {
            /// <summary>
            /// Whether the server supports formatting multiple ranges at once.
            ///
            /// @since 3.18.0
            /// </summary>
            [Optional]
            public bool RangesSupport { get; set; }
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.RangeFormatting))]
        public partial class DocumentRangeFormattingCapability : DynamicCapability
        {
            /// <summary>
            /// Whether the client supports formatting multiple ranges at once.
            ///
            /// @since 3.18.0
            /// </summary>
            [Optional]
            public bool RangesSupport { get; set; }
        }
    }

    namespace Document
    {
    }
}
