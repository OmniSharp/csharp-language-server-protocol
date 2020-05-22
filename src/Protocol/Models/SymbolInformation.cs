using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class SymbolInformation
    {
        /// <summary>
        /// The name of this symbol.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The kind of this symbol.
        /// </summary>
        public SymbolKind Kind { get; set; }

        /// <summary>
        /// Tags for this completion item.
        ///
        /// @since 3.16.0 - Proposed state
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [Optional]
        public Container<SymbolTag> Tags { get; set; }

        /// <summary>
        /// Indicates if this item is deprecated.
        /// </summary>
        [Optional]
        public bool Deprecated { get; set; }

        /// <summary>
        /// The location of this symbol.
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// The name of the symbol containing this symbol.
        /// </summary>
        [Optional]
        public string ContainerName { get; set; }
    }
}
