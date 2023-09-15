using System.Diagnostics;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [GenerateContainer]
    public record Location
    {
        /// <summary>
        /// The uri of the document
        /// </summary>
        public DocumentUri Uri { get; init; } = null!;

        /// <summary>
        /// The range inside the document given by the uri
        /// </summary>
        public Range Range { get; init; } = null!;

        private string DebuggerDisplay => $"{{{Range} {Uri}}}";

        /// <inheritdoc />
        public override string ToString()
        {
            return DebuggerDisplay;
        }
    }
}
