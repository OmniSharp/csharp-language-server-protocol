namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Enum of known range kinds
    /// </summary>
    public enum FoldingRangeKind
    {
        /// <summary>
        /// Folding range for a comment
        /// </summary>
        Comment,
        /// <summary>
        /// Folding range for a imports or includes
        /// </summary>
        Imports,
        /// <summary>
        /// Folding range for a region (e.g. `#region`)
        /// </summary>
        Region
    }
}
