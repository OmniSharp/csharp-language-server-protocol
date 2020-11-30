using System.Diagnostics;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Information about the server.
    ///
    /// @since 3.15.0
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public record ServerInfo
    {
        /// <summary>
        /// The name of the server as defined by the server.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// The servers's version as defined by the server.
        /// </summary>
        [Optional]
        public string? Version { get; init; }

        private string DebuggerDisplay => string.IsNullOrWhiteSpace(Version) ? Name : $"{Name} ({Version})";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
