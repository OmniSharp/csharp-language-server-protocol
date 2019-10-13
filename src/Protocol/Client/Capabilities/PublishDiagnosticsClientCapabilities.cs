using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// Capabilities specific to `textDocument/publishDiagnostics`.
    /// </summary>
    public class PublishDiagnosticsClientCapabilities
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
        public PublishDiagnosticsTagSupportClientCapabilities TagSupport { get; set; }
    }

    public class PublishDiagnosticsTagSupportClientCapabilities
    {
        /// <summary>
        /// The tags supported by the client.
        /// </summary>

        public Container<DiagnosticTag> ValueSet { get; set; }
    }
}
