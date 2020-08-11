using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class MessageActionItem
    {
        /// <summary>
        /// A short title like 'Retry', 'Open Log' etc.
        /// </summary>
        public string Title { get; set; }

        private string DebuggerDisplay => Title;

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
