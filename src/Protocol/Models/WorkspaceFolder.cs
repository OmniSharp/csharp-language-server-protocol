using System;
using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public record WorkspaceFolder(string Name, DocumentUri Uri)
    {
        public WorkspaceFolder() : this(null!, null!)
        {
        }

        private string DebuggerDisplay => $"{Name} ({Uri})";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
