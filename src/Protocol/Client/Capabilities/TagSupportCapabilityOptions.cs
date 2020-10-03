using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// The client supports tags on `SymbolInformation`.Tags are supported on
    /// `DocumentSymbol` if `hierarchicalDocumentSymbolSupport` is set tot true.
    /// Clients supporting tags have to handle unknown tags gracefully.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public class TagSupportCapabilityOptions
    {
        /// <summary>
        /// The tags supported by the client.
        /// </summary>
        [Obsolete(Constants.Proposal)]
        public Container<SymbolTag> ValueSet { get; set; } = null!;
    }
}
