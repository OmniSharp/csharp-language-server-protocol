using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
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

    namespace Document
    {
    }
}
