using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// Capabilities specific to the showDocument request
    ///
    /// @since 3.16.0 - proposed state
    /// </summary>
    [Obsolete(Constants.Proposal)]
    [CapabilityKey(nameof(ClientCapabilities.Window), nameof(WindowClientCapabilities.ShowDocument))]
    public class ShowDocumentClientCapabilities
    {
        /// <summary>
        /// Capabilities specific to the `MessageActionItem` type.
        /// </summary>
        [Optional]
        public bool Support { get; set; }
    }
}
