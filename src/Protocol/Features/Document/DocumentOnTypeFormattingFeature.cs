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
            /// The position around which the on type formatting should happen.
            /// This is not necessarily the exact position where the character denoted
            /// by the property `ch` got typed.
            /// </summary>
            public Position Position { get; init; } = null!;

            /// <summary>
            /// The character that has been typed that triggered the formatting
            /// on type request.That is not necessarily the last character that
            /// got inserted into the document since the client could auto insert
            /// characters as well(e.g.like automatic brace completion).
            /// </summary>
            [JsonProperty("ch")]
            public string Character { get; init; } = null!;

            /// <summary>
            /// The formatting options.
            /// </summary>
            public FormattingOptions Options { get; init; } = null!;
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.DocumentOnTypeFormattingProvider))]
        [RegistrationName(TextDocumentNames.OnTypeFormatting)]
        public partial class DocumentOnTypeFormattingRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions
        {
            /// <summary>
            /// A character on which formatting should be triggered, like `{`.
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
