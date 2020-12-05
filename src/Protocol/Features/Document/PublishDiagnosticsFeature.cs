using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(TextDocumentNames.PublishDiagnostics, Direction.ServerToClient)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageServer), typeof(ILanguageServer))
        ]
        [Capability(typeof(PublishDiagnosticsCapability))]
        public record PublishDiagnosticsParams : IRequest
        {
            /// <summary>
            /// The URI for which diagnostic information is reported.
            /// </summary>
            public DocumentUri Uri { get; init; }

            /// <summary>
            /// Optional the version number of the document the diagnostics are published for.
            ///
            /// @since 3.15.0
            /// </summary>
            /// <remarks>
            /// <see cref="uint"/> in the LSP spec
            /// </remarks>
            [Optional]
            public int? Version { get; init; }

            /// <summary>
            /// An array of diagnostic information items.
            /// </summary>
            public Container<Diagnostic> Diagnostics { get; init; }
        }
    }

    namespace Document
    {
    }

    namespace Client.Capabilities
    {
        /// <summary>
        /// Capabilities specific to `textDocument/publishDiagnostics`.
        /// </summary>
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.PublishDiagnostics))]
        public class PublishDiagnosticsCapability : ICapability
        {
            /// <summary>
            /// Whether the clients accepts diagnostics with related information.
            /// </summary>
            [Optional]
            public bool RelatedInformation { get; set; }

            /// <summary>
            /// Client supports the tag property to provide meta data about a diagnostic.
            /// Clients supporting tags have to handle unknown tags gracefully.
            ///
            /// @since 3.15.0
            /// </summary>
            [Optional]
            public Supports<PublishDiagnosticsTagSupportCapabilityOptions?> TagSupport { get; set; }

            /// <summary>
            /// Whether the client interprets the version property of the
            /// `textDocument/publishDiagnostics` notification's parameter.
            ///
            /// @since 3.15.0
            /// </summary>
            [Optional]
            public bool VersionSupport { get; set; }

            /// <summary>
            /// Client supports a codeDescription property
            ///
            /// @since 3.16.0 - proposed state
            /// </summary>
            [Optional]
            public bool CodeDescriptionSupport { get; set; }

            /// <summary>
            /// Whether code action supports the `data` property which is
            /// preserved between a `textDocument/publishDiagnostics` and
            /// `textDocument/codeAction` request.
            ///
            /// @since 3.16.0 - proposed state
            /// </summary>
            [Optional]
            public bool DataSupport { get; set; }
        }

        public class PublishDiagnosticsTagSupportCapabilityOptions
        {
            /// <summary>
            /// The tags supported by the client.
            /// </summary>
            public Container<DiagnosticTag> ValueSet { get; set; } = null!;
        }
    }
}
