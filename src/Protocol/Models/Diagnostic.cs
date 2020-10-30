using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Diagnostic : ICanHaveData
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

    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Diagnostic<T> : ICanHaveData
        where T : class?, new()
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
        public T Data
        {
            get => ( (ICanHaveData) this ).Data?.ToObject<T>()!;
            set => ( (ICanHaveData) this ).Data = JToken.FromObject(value);
        }

        JToken? ICanHaveData.Data { get; set; }

        public static implicit operator Diagnostic(Diagnostic<T> value) => new Diagnostic {
            Data = ( (ICanHaveData) value ).Data,
            Code = value.Code,
            Message = value.Message,
            Range = value.Range,
            Severity = value.Severity,
            Source = value.Source,
            Tags = value.Tags,
            CodeDescription = value.CodeDescription,
            RelatedInformation = value.RelatedInformation
        };

        public static implicit operator Diagnostic<T>(Diagnostic value)
        {
            var item = new Diagnostic<T> {
                Code = value.Code,
                Message = value.Message,
                Range = value.Range,
                Severity = value.Severity,
                Source = value.Source,
                Tags = value.Tags,
                CodeDescription = value.CodeDescription,
                RelatedInformation = value.RelatedInformation
            };
            ( (ICanHaveData) item ).Data = value.Data;
            return item;
        }

        private string DebuggerDisplay =>
            $"{( Code.HasValue ? $"[{Code.Value.ToString()}]" : "" )}" +
            $"{Range}" +
            $"{( string.IsNullOrWhiteSpace(Source) ? "" : $" ({Source})" )}" +
            $"{( Tags?.Any() == true ? $" [tags: {string.Join(", ", Tags.Select(z => z.ToString()))}]" : "" )}" +
            $" {( Message?.Length > 20 ? Message.Substring(0, 20) : Message )}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
