using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class Diagnostic
    {
        /// <summary>
        /// The range at which the message applies.
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// The diagnostic's severity. Can be omitted. If omitted it is up to the
        /// client to interpret diagnostics as error, warning, info or hint.
        /// </summary>
        [Optional]
        public DiagnosticSeverity? Severity { get; set; }

        /// <summary>
        /// The diagnostic's code. Can be omitted.
        /// </summary>
        [Optional]
        public DiagnosticCode? Code { get; set; }

        /// <summary>
        /// A human-readable string describing the source of this
        /// diagnostic, e.g. 'typescript' or 'super lint'.
        /// </summary>
        [Optional]
        public string Source { get; set; }

        /// <summary>
        /// The diagnostic's message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Additional metadata about the diagnostic.
        ///
        /// @since 3.15.0
        /// </summary>
        [Optional]
        public Container<DiagnosticTag> Tags { get; set; }

        /// <summary>
        /// An array of related diagnostic information, e.g. when symbol-names within
        /// a scope collide all definitions can be marked via this property.
        /// </summary>
        [Optional]
        public Container<DiagnosticRelatedInformation> RelatedInformation { get; set; }
    }
}
