using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// Capabilities specific to `textDocument/publishDiagnostics`.
    /// </summary>
    public class PublishDiagnosticsCapability
    {
        /// <summary>
        /// Whether the clients accepts diagnostics with related information.
        /// </summary>
        [Optional]
        public bool RelatedInformation { get; set; }
    }
}
