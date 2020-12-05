using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(TextDocumentNames.DocumentHighlight, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(DocumentHighlightRegistrationOptions)), Capability(typeof(DocumentHighlightCapability))]
        public partial record DocumentHighlightParams : TextDocumentPositionParams, IWorkDoneProgressParams, IPartialItemsRequest<DocumentHighlightContainer?, DocumentHighlight>
        {
        }

        /// <summary>
        /// A document highlight is a range inside a text document which deserves
        /// special attention. Usually a document highlight is visualized by changing
        /// the background color of its range.
        ///
        /// </summary>
        [GenerateContainer]
        public partial record DocumentHighlight
        {
            /// <summary>
            /// The range this highlight applies to.
            /// </summary>
            public Range Range { get; init; }

            /// <summary>
            /// The highlight kind, default is DocumentHighlightKind.Text.
            /// </summary>
            public DocumentHighlightKind Kind { get; init; }
        }

        /// <summary>
        /// A document highlight kind.
        /// </summary>
        [JsonConverter(typeof(NumberEnumConverter))]
        public enum DocumentHighlightKind
        {
            /// <summary>
            /// A textual occurrence.
            /// </summary>
            Text = 1,

            /// <summary>
            /// Read-access of a symbol, like reading a variable.
            /// </summary>
            Read = 2,

            /// <summary>
            /// Write-access of a symbol, like writing to a variable.
            /// </summary>
            Write = 3,
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.DocumentHighlightProvider))]
        [RegistrationName(TextDocumentNames.DocumentHighlight)]
        public partial class DocumentHighlightRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions
        {
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.DocumentHighlight))]
        public partial class DocumentHighlightCapability : DynamicCapability, ConnectedCapability<IDocumentHighlightHandler>
        {
        }
    }

    namespace Document
    {
    }
}
