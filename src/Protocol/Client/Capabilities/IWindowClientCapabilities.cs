﻿namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public interface IWindowClientCapabilities : ICapabilitiesBase
    {
        /// <summary>
        /// Whether the client supports server initiated progress using the `window/workDoneProgress/create` request.
        /// </summary>
        Supports<bool> WorkDoneProgress { get; set; }

        /// <summary>
        /// Capabilities specific to the showMessage request
        ///
        /// @since 3.16.0
        /// </summary>
        Supports<ShowMessageRequestClientCapabilities> ShowMessage { get; set; }

        /// <summary>
        /// Client capabilities for the show document request.
        ///
        /// @since 3.16.0
        /// </summary>
        Supports<ShowDocumentClientCapabilities> ShowDocument { get; set; }
    }
}
