using System.Diagnostics;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// General paramters to to regsiter for a capability.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Registration
    {
        /// <summary>
        /// The id used to register the request. The id can be used to deregister
        /// the request again.
        /// </summary>
        public string Id { get; set; } = null!;

        /// <summary>
        /// The method / capability to register for.
        /// </summary>
        public string Method { get; set; } = null!;

        /// <summary>
        /// Options necessary for the registration.
        /// </summary>
        [Optional]
        public object? RegisterOptions { get; set; }

        private string DebuggerDisplay => $"[{Id}] {( RegisterOptions is ITextDocumentRegistrationOptions td ? $"{td.DocumentSelector}" : string.Empty )} {Method}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
