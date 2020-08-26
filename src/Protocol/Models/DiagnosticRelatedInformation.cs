namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Represents a related message and source code location for a diagnostic. This should be
    /// used to point to code locations that cause or related to a diagnostics, e.g when duplicating
    /// a symbol in a scope.
    /// </summary>
    public class DiagnosticRelatedInformation
    {
        /// <summary>
        /// The location of this related diagnostic information.
        /// </summary>
        public Location Location { get; set; } = null!;

        /// <summary>
        /// The message of this related diagnostic information.
        /// </summary>
        public string Message { get; set; } = null!;
    }
}
