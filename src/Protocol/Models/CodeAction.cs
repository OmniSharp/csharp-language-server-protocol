using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class CodeAction
    {
        /// <summary>
        /// A short, human-readable, title for this code action.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The kind of the code action.
        ///
        /// Used to filter code actions.
        /// </summary>
        [Optional]
        public CodeActionKind Kind { get; set; }

        /// <summary>
        /// The diagnostics that this code action resolves.
        /// </summary>
        [Optional]
        public Container<Diagnostic> Diagnostics { get; set; }

        /// <summary>
        /// The workspace edit this code action performs.
        /// </summary>
        [Optional]
        public WorkspaceEdit Edit { get; set; }

        /// <summary>
        /// A command this code action executes. If a code action
        /// provides an edit and a command, first the edit is
        /// executed and then the command.
        /// </summary>
        [Optional]
        public Command Command { get; set; }
    }
}
