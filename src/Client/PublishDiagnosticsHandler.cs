using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    /// <summary>
    ///     A handler for diagnostics published by the language server.
    /// </summary>
    /// <param name="documentUri">
    ///     The URI of the document that the diagnostics apply to.
    /// </param>
    /// <param name="diagnostics">
    ///     A list of <see cref="Diagnostic"/>s.
    /// </param>
    /// <remarks>
    ///     The diagnostics should replace any previously published diagnostics for the specified document.
    /// </remarks>
    public delegate void PublishDiagnosticsHandler(DocumentUri documentUri, List<Diagnostic> diagnostics);
}
