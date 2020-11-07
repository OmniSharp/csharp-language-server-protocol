using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public record Location(DocumentUri Uri, Range Range)
    {
        public Location(): this(null!, ((0, 0), (0, 0))) { }

        private string DebuggerDisplay => $"{{{Range} {Uri}}}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
