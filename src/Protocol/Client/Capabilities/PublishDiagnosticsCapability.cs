using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
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
}
