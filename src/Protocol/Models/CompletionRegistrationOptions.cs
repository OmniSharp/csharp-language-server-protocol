using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class CompletionRegistrationOptions : WorkDoneTextDocumentRegistrationOptions, ICompletionOptions
    {
        /// <summary>
        /// The server provides support to resolve additional
        /// information for a completion item.
        /// </summary>
        [Optional]
        public bool ResolveProvider { get; set; }

        /// <summary>
        /// Most tools trigger completion request automatically without explicitly requesting
        /// it using a keyboard shortcut (e.g. Ctrl+Space). Typically they do so when the user
        /// starts to type an identifier. For example if the user types `c` in a JavaScript file
        /// code complete will automatically pop up present `console` besides others as a
        /// completion item. Characters that make up identifiers don't need to be listed here.
        ///
        /// If code complete should automatically be trigger on characters not being valid inside
        /// an identifier (for example `.` in JavaScript) list them in `triggerCharacters`.
        /// </summary>
        [Optional]
        public Container<string> TriggerCharacters { get; set; }

        /// <summary>
        /// The list of all possible characters that commit a completion. This field can be used
        /// if clients don't support individual commmit characters per completion item. See
        /// `Capability.textDocument.completion.completionItem.commitCharactersSupport`
        ///
        /// @since 3.2.0
        /// </summary>
        [Optional]
        public Container<string> AllCommitCharacters { get; set; }
    }
}
