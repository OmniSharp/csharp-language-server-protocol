using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.PublishDiagnostics, Direction.ServerToClient)]
    public class PublishDiagnosticsParams : IRequest
    {
        /// <summary>
        /// The URI for which diagnostic information is reported.
        /// </summary>
        public DocumentUri Uri { get; set; } = null!;

        /// <summary>
        /// Optional the version number of the document the diagnostics are published for.
        ///
        /// @since 3.15.0
        /// </summary>
        /// <remarks>
        /// <see cref="uint"/> in the LSP spec
        /// </remarks>
        [Optional]
        public int? Version { get; set; }

        /// <summary>
        /// An array of diagnostic information items.
        /// </summary>
        public Container<Diagnostic> Diagnostics { get; set; } = null!;
    }
}
