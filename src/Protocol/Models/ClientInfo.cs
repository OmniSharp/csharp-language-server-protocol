using System.Diagnostics;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Information about the client
    ///
    /// @since 3.15.0
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class ClientInfo
    {
        /// <summary>
        /// The name of the client as defined by the client.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// The client's version as defined by the client.
        /// </summary>
        [Optional]
        public string? Version { get; set; }

        private string DebuggerDisplay => string.IsNullOrWhiteSpace(Version) ? Name : $"{Name} ({Version})";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
