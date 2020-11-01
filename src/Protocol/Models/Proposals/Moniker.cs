using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// Moniker definition to match LSIF 0.5 moniker definition.
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public class Moniker
    {
        /// <summary>
        /// The scheme of the moniker. For example tsc or .Net
        /// </summary>
        public string Scheme { get; set; } = null!;

        /// <summary>
        /// The identifier of the moniker. The value is opaque in LSIF however
        /// schema owners are allowed to define the structure if they want.
        /// </summary>
        public string Identifier { get; set; } = null!;

        /// <summary>
        /// The scope in which the moniker is unique
        /// </summary>
        public UniquenessLevel Unique { get; set; }

        /// <summary>
        /// The moniker kind if known.
        /// </summary>
        [Optional]
        public MonikerKind Kind { get; set; }
    }
}
