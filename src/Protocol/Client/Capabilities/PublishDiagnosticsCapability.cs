using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// Capabilities specific to `textDocument/publishDiagnostics`.
    /// </summary>
    public class PublishDiagnosticsCapability: ICapability
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
        public Supports<PublishDiagnosticsTagSupportCapability> TagSupport { get; set; }

        /// <summary>
        /// Whether the client interprets the version property of the
        /// `textDocument/publishDiagnostics` notification's parameter.
        ///
        /// @since 3.15.0
        /// </summary>
        [Optional]
        public bool VersionSupport { get; set; }
    }
}
