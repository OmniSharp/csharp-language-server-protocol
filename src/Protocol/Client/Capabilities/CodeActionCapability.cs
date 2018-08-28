using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class CodeActionCapability : DynamicCapability, ConnectedCapability<ICodeActionHandler>
    {
        /// <summary>
        /// The client support code action literals as a valid
        /// response of the `textDocument/codeAction` request.
        ///
        /// Since 3.8.0
        /// </summary>
        [Optional]
        public CodeActionLiteralSupportCapability codeActionLiteralSupport { get; set; }
    }

    public class CodeActionLiteralSupportCapability
    {

        /// <summary>
        /// The code action kind is support with the following value
        /// set.
        /// </summary>

        public CodeActionKindCapability CodeActionKind { get; set; }
    }

    public class CodeActionKindCapability
    {
        /// <summary>
        /// The code action kind values the client supports. When this
        /// property exists the client also guarantees that it will
        /// handle values outside its set gracefully and falls back
        /// to a default value when unknown.
        /// </summary>
        public Container<CodeActionKind> ValueSet { get; set; }
    }

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
            kind = kind;
        }

        public string Kind { get; }

    }
}
