using System.Diagnostics;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// The glob pattern. Either a string pattern or a relative pattern.
    ///
    /// @since 3.17.0
    /// </summary>
    [JsonConverter(typeof(GlobPatternConverter))]
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public record GlobPattern
    {
        public GlobPattern(string value) => Pattern = value;

        public GlobPattern(RelativePattern value) => RelativePattern = value;

        /// <summary>
        /// The glob pattern to watch relative to the base path. Glob patterns can have
        /// the following syntax:
        /// - `*` to match one or more characters in a path segment
        /// - `?` to match on one character in a path segment
        /// - `**` to match any number of path segments, including none
        /// - `{}` to group conditions (e.g. `**​/*.{ts,js}` matches all TypeScript
        ///   and JavaScript files)
        /// - `[]` to declare a range of characters to match in a path segment
        ///   (e.g., `example.[0-9]` to match on `example.0`, `example.1`, …)
        /// - `[!...]` to negate a range of characters to match in a path segment
        ///   (e.g., `example.[!0-9]` to match on `example.a`, `example.b`,
        ///   but not `example.0`)
        ///
        /// @since 3.17.0
        /// </summary>
        public string? Pattern { get; }
        public bool HasPattern => RelativePattern is null;

        /// <summary>
        /// A relative pattern is a helper to construct glob patterns that are matched
        /// relatively to a base URI. The common value for a `baseUri` is a workspace
        /// folder root, but it can be another absolute URI as well.
        ///
        /// @since 3.17.0
        /// </summary>
        public RelativePattern? RelativePattern { get; }
        public bool HasRelativePattern => RelativePattern is { };

        public static implicit operator GlobPattern?(string? value) =>
            value is null ? null : new GlobPattern(value);

        public static implicit operator GlobPattern?(RelativePattern value) =>
            value is null ? null : new GlobPattern(value);

        private string DebuggerDisplay =>
            $"{( HasPattern ? Pattern : HasRelativePattern ? RelativePattern : string.Empty )}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
