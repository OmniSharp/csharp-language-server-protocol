using System.Diagnostics;
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
        [Method(TextDocumentNames.Hover, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(HoverRegistrationOptions)), Capability(typeof(HoverCapability))]
        public partial class HoverParams : TextDocumentPositionParams, IWorkDoneProgressParams, IRequest<Hover?>
        {
        }

        /// <summary>
        /// The result of a hover request.
        /// </summary>
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public partial class Hover
        {
            /// <summary>
            /// The hover's content
            /// </summary>
            public MarkedStringsOrMarkupContent Contents { get; set; } = null!;

            /// <summary>
            /// An optional range is a range inside a text document
            /// that is used to visualize a hover, e.g. by changing the background color.
            /// </summary>
            [Optional]
            public Range? Range { get; set; }

            private string DebuggerDisplay => $"{Contents}{( Range is not null ? $" {Range}" : string.Empty )}";

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.HoverProvider))]
        public partial class HoverRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions { }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.Hover))]
        public partial class HoverCapability : DynamicCapability, ConnectedCapability<IHoverHandler>
        {
            /// <summary>
            /// Client supports the follow content formats for the content property. The order describes the preferred format of the client.
            /// </summary>
            [Optional]
            public Container<MarkupKind>? ContentFormat { get; set; }
        }
    }

    namespace Document
    {
    }
}
