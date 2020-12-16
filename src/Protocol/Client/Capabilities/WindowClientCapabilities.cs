using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// Window specific client capabilities.
    /// </summary>
    public class WindowClientCapabilities : CapabilitiesBase, IWindowClientCapabilities
    {
        /// <summary>
        /// Whether client supports handling progress notifications.
        /// </summary>
        public Supports<bool> WorkDoneProgress { get; set; }

        /// <summary>
        /// Capabilities specific to the showMessage request
        ///
        /// @since 3.16.0
        /// </summary>
        public Supports<ShowMessageRequestClientCapabilities> ShowMessage { get; set; }

        /// <summary>
        /// Client capabilities for the show document request.
        ///
        /// @since 3.16.0
        /// </summary>
        public Supports<ShowDocumentClientCapabilities> ShowDocument { get; set; }
    }
}
