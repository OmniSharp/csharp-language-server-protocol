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
        /// Marks this as a preferred action. Preferred actions are used by the `auto fix` command and can be targeted
        /// by keybindings.
        ///
        /// A quick fix should be marked preferred if it properly addresses the underlying error.
        /// A refactoring should be marked preferred if it is the most reasonable choice of actions to take.
        ///
        /// @since 3.15.0
        /// </summary>
        [Optional]
        public bool IsPreferred { get; set; }

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
