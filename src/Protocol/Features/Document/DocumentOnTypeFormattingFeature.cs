using MediatR;
using Newtonsoft.Json;
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
        [Method(TextDocumentNames.OnTypeFormatting, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(DocumentOnTypeFormattingRegistrationOptions))]
        [Capability(typeof(DocumentOnTypeFormattingCapability))]
        public partial record DocumentOnTypeFormattingParams : ITextDocumentIdentifierParams, IRequest<TextEditContainer?>
        {
            /// <summary>
            /// The document to format.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; init; } = null!;

            /// <summary>
            /// The position at which this request was sent.
            /// </summary>
            public Position Position { get; init; } = null!;

            /// <summary>
            /// The character that has been typed.
            /// </summary>
            [JsonProperty("ch")]
            public string Character { get; init; } = null!;

            /// <summary>
            /// The format options.
            /// </summary>
            public FormattingOptions Options { get; init; } = null!;
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.DocumentOnTypeFormattingProvider))]
        [RegistrationName(TextDocumentNames.OnTypeFormatting)]
        public partial class DocumentOnTypeFormattingRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions
        {
            /// <summary>
            /// A character on which formatting should be triggered, like `}`.
            /// </summary>
            public string FirstTriggerCharacter { get; set; } = null!;

            /// <summary>
            /// More trigger characters.
            /// </summary>
            [Optional]
            public Container<string>? MoreTriggerCharacter { get; set; }
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.OnTypeFormatting))]
        public partial class DocumentOnTypeFormattingCapability : DynamicCapability
        {
        }
    }

    namespace Document
    {
    }
}
