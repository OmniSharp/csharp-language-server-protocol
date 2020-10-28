using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Structure to capture a description for an error code.
    ///
    /// @since 3.16.0 - proposed state
    /// </summary>
    public class CodeDescription
    {
        /// <summary>
        /// An URI to open with more information about the diagnostic error.
        /// </summary>
        public Uri Href { get; set; } = null!;
    }
}
