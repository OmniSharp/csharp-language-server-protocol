using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// General parameters to unregister a request or notification.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Unregistration
    {
        /// <summary>
        /// The id used to unregister the request or notification. Usually an id
        /// provided during the register request.
        /// </summary>
        public string Id { get; set; } = null!;

        /// <summary>
        /// The method to unregister for.
        /// </summary>
        public string Method { get; set; } = null!;

        public static implicit operator Unregistration(Registration registration) =>
            new Unregistration {
                Id = registration.Id,
                Method = registration.Method
            };

        private string DebuggerDisplay => $"[{Id}] {Method}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
