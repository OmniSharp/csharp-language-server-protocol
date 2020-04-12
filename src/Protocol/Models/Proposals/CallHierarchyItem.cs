using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// Represents programming constructs like functions or constructors in the context
    /// of call hierarchy.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public class CallHierarchyItem
    {
        /// <summary>
        /// The name of this item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The kind of this item.
        /// </summary>
        public SymbolKind Kind { get; set; }

        /// <summary>
        /// Tags for this item.
        /// </summary>
        [Optional]
        public Container<SymbolTag> Tags { get; set; }

        /// <summary>
        /// More detail for this item, e.g. the signature of a function.
        /// </summary>
        [Optional]
        public string Detail { get; set; }

        /// <summary>
        /// The resource identifier of this item.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// The range enclosing this symbol not including leading/trailing whitespace but everything else, e.g. comments and code.
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// The range that should be selected and revealed when this symbol is being picked, e.g. the name of a function.
        /// Must be contained by the [`range`](#CallHierarchyItem.range).
        /// </summary>
        public Range SelectionRange { get; set; }
    }
}
