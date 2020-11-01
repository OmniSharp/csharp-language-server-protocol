using System.Diagnostics;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [Method(TextDocumentNames.CodeActionResolve, Direction.ClientToServer)]
    public class CodeAction : ICanBeResolved, IRequest<CodeAction>
    {
        /// <summary>
        /// A short, human-readable, title for this code action.
        /// </summary>
        public string Title { get; set; } = null!;

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
        public Container<Diagnostic>? Diagnostics { get; set; }

        /// <summary>
        /// The workspace edit this code action performs.
        /// </summary>
        [Optional]
        public WorkspaceEdit? Edit { get; set; }

        /// <summary>
        /// A command this code action executes. If a code action
        /// provides an edit and a command, first the edit is
        /// executed and then the command.
        /// </summary>
        [Optional]
        public Command? Command { get; set; }

        /// <summary>
        /// Marks that the code action cannot currently be applied.
        ///
        /// Clients should follow the following guidelines regarding disabled code actions:
        ///
        ///   - Disabled code actions are not shown in automatic [lightbulb](https://code.visualstudio.com/docs/editor/editingevolved#_code-action)
        ///     code action menu.
        ///
        ///   - Disabled actions are shown as faded out in the code action menu when the user request a more specific type
        ///     of code action, such as refactorings.
        ///
        ///   - If the user has a [keybinding](https://code.visualstudio.com/docs/editor/refactoring#_keybindings-for-code-actions)
        ///     that auto applies a code action and only a disabled code actions are returned, the client should show the user an
        ///     error message with `reason` in the editor.
        ///
        /// @since 3.16.0
        /// </summary>
        [Optional]
        public CodeActionDisabled? Disabled { get; set; }

        /// <summary>
        /// A data entry field that is preserved on a document link between a
        /// DocumentLinkRequest and a DocumentLinkResolveRequest.
        /// </summary>
        [Optional]
        public JToken? Data { get; set; }

        private string DebuggerDisplay => $"[{Kind}] {Title}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }

    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class CodeAction<T> : ICanBeResolved
        where T : HandlerIdentity?, new()
    {
        /// <summary>
        /// A short, human-readable, title for this code action.
        /// </summary>
        public string Title { get; set; } = null!;

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
        public Container<Diagnostic>? Diagnostics { get; set; }

        /// <summary>
        /// The workspace edit this code action performs.
        /// </summary>
        [Optional]
        public WorkspaceEdit? Edit { get; set; }

        /// <summary>
        /// A command this code action executes. If a code action
        /// provides an edit and a command, first the edit is
        /// executed and then the command.
        /// </summary>
        [Optional]
        public Command? Command { get; set; }

        /// <summary>
        /// Marks that the code action cannot currently be applied.
        ///
        /// Clients should follow the following guidelines regarding disabled code actions:
        ///
        ///   - Disabled code actions are not shown in automatic [lightbulb](https://code.visualstudio.com/docs/editor/editingevolved#_code-action)
        ///     code action menu.
        ///
        ///   - Disabled actions are shown as faded out in the code action menu when the user request a more specific type
        ///     of code action, such as refactorings.
        ///
        ///   - If the user has a [keybinding](https://code.visualstudio.com/docs/editor/refactoring#_keybindings-for-code-actions)
        ///     that auto applies a code action and only a disabled code actions are returned, the client should show the user an
        ///     error message with `reason` in the editor.
        ///
        /// @since 3.16.0
        /// </summary>
        [Optional]
        public CodeActionDisabled? Disabled { get; set; }

        /// <summary>
        /// A data entry field that is preserved on a code lens item between
        /// a code lens and a code lens resolve request.
        /// </summary>
        [Optional]
        public T Data
        {
            get => ( (ICanBeResolved) this ).Data?.ToObject<T>()!;
            set => ( (ICanBeResolved) this ).Data = JToken.FromObject(value);
        }

        JToken? ICanBeResolved.Data { get; set; }

        public static implicit operator CodeAction(CodeAction<T> value) => new CodeAction {
            Data = ( (ICanBeResolved) value ).Data,
            Command = value.Command,
            Diagnostics = value.Diagnostics,
            Disabled = value.Disabled,
            Edit = value.Edit,
            Kind = value.Kind,
            Title = value.Title,
            IsPreferred = value.IsPreferred,
        };

        public static implicit operator CodeAction<T>(CodeAction value)
        {
            var item = new CodeAction<T> {
                Command = value.Command,
                Diagnostics = value.Diagnostics,
                Disabled = value.Disabled,
                Edit = value.Edit,
                Kind = value.Kind,
                Title = value.Title,
                IsPreferred = value.IsPreferred,
            };
            ( (ICanBeResolved) item ).Data = value.Data;
            return item;
        }

        private string DebuggerDisplay => $"[{Kind}] {Title}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
