using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public record FileSystemWatcher
    {
        /// <summary>
        /// The glob pattern to watch. See <see cref="Models.GlobPattern"/>
        /// for more detail.
        ///
        /// @since 3.17.0 support for relative patterns.
        /// </summary>
        public GlobPattern GlobPattern { get; init; }

        /// <summary>
        /// The kind of events of interest. If omitted it defaults
        /// to WatchKind.Create | WatchKind.Change | WatchKind.Delete
        /// which is 7.
        /// </summary>
        public WatchKind Kind { get; init; }

        private string DebuggerDisplay => $"[{Kind}] {GlobPattern}";

        /// <inheritdoc />
        public override string ToString()
        {
            return DebuggerDisplay;
        }
    }
}
