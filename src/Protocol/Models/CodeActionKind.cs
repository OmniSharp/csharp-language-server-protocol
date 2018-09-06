using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// A set of predefined code action kinds
    /// </summary>
    [JsonConverter(typeof(CodeActionKindConverter))]
    public class CodeActionKind
    {
        /// <summary>
        /// Base kind for quickfix actions: 'quickfix'
        /// </summary>
        public static CodeActionKind QuickFix = new CodeActionKind("quickfix");

        /// <summary>
        /// Base kind for refactoring actions: 'refactor'
        /// </summary>
        public static CodeActionKind Refactor = new CodeActionKind("refactor");

        /// <summary>
        /// Base kind for refactoring extraction actions: 'refactor.extract'
        ///
        /// Example extract actions:
        ///
        /// - Extract method
        /// - Extract function
        /// - Extract variable
        /// - Extract interface from class
        /// - ...
        /// </summary>
        public static CodeActionKind RefactorExtract = new CodeActionKind("refactor.extract");

        /// <summary>
        /// Base kind for refactoring inline actions: 'refactor.inline'
        ///
        /// Example inline actions:
        ///
        /// - Inline function
        /// - Inline variable
        /// - Inline constant
        /// - ...
        /// </summary>
        public static CodeActionKind RefactorInline = new CodeActionKind("refactor.inline");

        /// <summary>
        /// Base kind for refactoring rewrite actions: 'refactor.rewrite'
        ///
        /// Example rewrite actions:
        ///
        /// - Convert JavaScript function to class
        /// - Add or remove parameter
        /// - Encapsulate field
        /// - Make method static
        /// - Move method to base class
        /// - ...
        /// </summary>
        public static CodeActionKind RefactorRewrite = new CodeActionKind("refactor.rewrite");

        /// <summary>
        /// Base kind for source actions: `source`
        ///
        /// Source code actions apply to the entire file.
        /// </summary>
        public static CodeActionKind Source = new CodeActionKind("source");

        /// <summary>
        /// Base kind for an organize imports source action: `source.organizeImports`
        /// </summary>
        public static CodeActionKind SourceOrganizeImports = new CodeActionKind("source.organizeImports");

        public CodeActionKind(string kind)
        {
            Kind = kind;
        }

        public string Kind { get; }

    }
}
