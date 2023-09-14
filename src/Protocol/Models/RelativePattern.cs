using System.Diagnostics;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// A relative pattern is a helper to construct glob patterns that are matched
    /// relatively to a base URI. The common value for a `baseUri` is a workspace
    /// folder root, but it can be another absolute URI as well.
    ///
    /// @since 3.17.0
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [GenerateContainer]
    public record RelativePattern
    {
        /// <summary>
        /// A workspace folder or a base URI to which this pattern will be matched
        /// against relatively.
        /// </summary>
        public WorkspaceFolderOrUri BaseUri { get; init; } = null!;

        /// <summary>
        /// The actual glob pattern;
        /// </summary>
        public string Pattern { get; init; } = null!;

        private string DebuggerDisplay => $"{{{BaseUri} {Pattern}}}";

        /// <inheritdoc />
        public override string ToString()
        {
            return DebuggerDisplay;
        }
    }
}
