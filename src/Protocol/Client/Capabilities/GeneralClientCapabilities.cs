using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// General client capabilities.
    /// </summary>
    public class GeneralClientCapabilities : CapabilitiesBase, IGeneralClientCapabilities
    {
        /// <summary>
        /// Client capabilities specific to regular expressions.
        ///
        /// @since 3.16.0
        /// </summary>
        [Optional]
        public RegularExpressionsClientCapabilities? RegularExpressions { get; set; }

        /// <summary>
        /// Client capabilities specific to the client's markdown parser.
        ///
        /// @since 3.16.0
        /// </summary>
        [Optional]
        public MarkdownClientCapabilities? Markdown { get; set; }

        /// <summary>
        /// Client capability that signals how the client
        /// handles stale requests (e.g. a request
        /// for which the client will not process the response
        /// anymore since the information is outdated).
        ///
        /// @since 3.17.0
        /// </summary>
        [Optional]
        public StaleRequestSupportClientCapabilities? StaleRequestSupport { get; set; }

        /// <summary>
        /// The position encodings supported by the client. Client and server
        /// have to agree on the same position encoding to ensure that offsets
        /// (e.g. character position in a line) are interpreted the same on both
        /// side.
        ///
        /// To keep the protocol backwards compatible the following applies: if
        /// the value 'utf-16' is missing from the array of position encodings
        /// servers can assume that the client supports UTF-16. UTF-16 is
        /// therefore a mandatory encoding.
        ///
        /// If omitted it defaults to ['utf-16'].
        ///
        /// Implementation considerations: since the conversion from one encoding
        /// into another requires the content of the file / line the conversion
        /// is best done where the file is read which is usually on the server
        /// side.
        ///
        /// @since 3.17.0
        /// </summary>
        [Optional]
        public Container<PositionEncodingKind>? PositionEncodings { get; set; }
    }
}
