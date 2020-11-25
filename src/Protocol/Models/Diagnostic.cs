using System;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [GenerateTypedData]
    public partial class Diagnostic : ICanHaveData
    {
        /// <summary>
        /// The range at which the message applies.
        /// </summary>
        public Range Range { get; set; } = null!;

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
        /// An optional property to describe the error code.
        ///
        /// @since 3.16.0 - proposed state
        /// </summary>
        [Optional]
        public CodeDescription? CodeDescription { get; set; }

        /// <summary>
        /// A human-readable string describing the source of this
        /// diagnostic, e.g. 'typescript' or 'super lint'.
        /// </summary>
        [Optional]
        public string? Source { get; set; }

        /// <summary>
        /// The diagnostic's message.
        /// </summary>
        public string Message { get; set; } = null!;

        /// <summary>
        /// Additional metadata about the diagnostic.
        ///
        /// @since 3.15.0
        /// </summary>
        [Optional]
        public Container<DiagnosticTag>? Tags { get; set; }

        /// <summary>
        /// An array of related diagnostic information, e.g. when symbol-names within
        /// a scope collide all definitions can be marked via this property.
        /// </summary>
        [Optional]
        public Container<DiagnosticRelatedInformation>? RelatedInformation { get; set; }

        /// <summary>
        /// A data entry field that is preserved on a code lens item between
        /// a code lens and a code lens resolve request.
        /// </summary>
        [Optional] public JToken? Data { get; set; }

        private string DebuggerDisplay =>
            $"{( Code.HasValue ? $"[{Code.Value.ToString()}]" : "" )}" +
            $"{Range}" +
            $"{( string.IsNullOrWhiteSpace(Source) ? "" : $" ({Source})" )}" +
            $"{( Tags?.Any() == true ? $" [tags: {string.Join(", ", Tags.Select(z => z.ToString()))}]" : "" )}" +
            $" {( Message?.Length > 20 ? Message.Substring(0, 20) : Message )}";
    }

    [JsonConverter(typeof(DiagnosticCodeConverter))]
    public struct DiagnosticCode
    {
        public DiagnosticCode(long value)
        {
            Long = value;
            String = null;
        }

        public DiagnosticCode(string value)
        {
            Long = 0;
            String = value;
        }

        public bool IsLong => String == null;
        public long Long { get; set; }
        public bool IsString => String != null;
        public string? String { get; set; }

        public static implicit operator DiagnosticCode(long value) => new DiagnosticCode(value);

        public static implicit operator DiagnosticCode(string value) => new DiagnosticCode(value);

        public static implicit operator long(DiagnosticCode value) => value.IsLong ? value.Long : 0;

        public static implicit operator string?(DiagnosticCode value) => value.IsString ? value.String : null;
    }

    [JsonConverter(typeof(NumberEnumConverter))]
    public enum DiagnosticSeverity
    {
        /// <summary>
        /// Reports an error.
        /// </summary>
        Error = 1,

        /// <summary>
        /// Reports a warning.
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Reports an information.
        /// </summary>
        Information = 3,

        /// <summary>
        /// Reports a hint.
        /// </summary>
        Hint = 4,
    }

    /// <summary>
    /// The diagnostic tags.
    ///
    /// @since 3.15.0
    /// </summary>
    [JsonConverter(typeof(NumberEnumConverter))]
    public enum DiagnosticTag
    {
        /// <summary>
        /// Unused or unnecessary code.
        ///
        /// Clients are allowed to render diagnostics with this tag faded out instead of having
        /// an error squiggle.
        /// </summary>
        Unnecessary = 1,

        /// <summary>
        /// Deprecated or obsolete code.
        ///
        /// Clients are allowed to rendered diagnostics with this tag strike through.
        /// </summary>
        Deprecated = 2,
    }

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
