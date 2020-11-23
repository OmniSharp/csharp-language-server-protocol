using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        /// <summary>
        /// Params for the CodeActionRequest
        /// </summary>
        [Parallel]
        [Method(TextDocumentNames.CodeAction, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(CodeActionRegistrationOptions)), Capability(typeof(CodeActionCapability)), Resolver(typeof(CodeAction))]
        public partial class CodeActionParams : ITextDocumentIdentifierParams, IPartialItemsRequest<CommandOrCodeActionContainer, CommandOrCodeAction>, IWorkDoneProgressParams
        {
            /// <summary>
            /// The document in which the command was invoked.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; set; } = null!;

            /// <summary>
            /// The range for which the command was invoked.
            /// </summary>
            public Range Range { get; set; } = null!;

            /// <summary>
            /// Context carrying additional information.
            /// </summary>
            public CodeActionContext Context { get; set; } = null!;
        }

        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        [Parallel]
        [Method(TextDocumentNames.CodeActionResolve, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document", Name = "CodeActionResolve"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient)),
            GenerateTypedData,
            GenerateContainer
        ]
        [RegistrationOptions(typeof(CodeActionRegistrationOptions)), Capability(typeof(CodeActionCapability))]
        public partial class CodeAction : ICanBeResolved, IRequest<CodeAction>
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

        [JsonConverter(typeof(CommandOrCodeActionConverter))]
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        [GenerateContainer]
        public class CommandOrCodeAction : ICanBeResolved // This to ensure that code actions get updated as expected
        {
            private CodeAction? _codeAction;
            private Command? _command;

            public CommandOrCodeAction(CodeAction value)
            {
                _codeAction = value;
                _command = default;
            }

            public CommandOrCodeAction(Command value)
            {
                _codeAction = default;
                _command = value;
            }

            public bool IsCommand => _command != null;

            public Command? Command
            {
                get => _command;
                set {
                    _command = value;
                    _codeAction = null;
                }
            }

            public bool IsCodeAction => _codeAction != null;

            public CodeAction? CodeAction
            {
                get => _codeAction;
                set {
                    _command = default;
                    _codeAction = value;
                }
            }

            public object? RawValue
            {
                get {
                    if (IsCommand) return Command!;
                    if (IsCodeAction) return CodeAction!;
                    return default;
                }
            }

            public static implicit operator CommandOrCodeAction(Command value) => new CommandOrCodeAction(value);

            public static implicit operator CommandOrCodeAction(CodeAction value) => new CommandOrCodeAction(value);

            private string DebuggerDisplay => $"{( IsCommand ? $"command: {Command}" : IsCodeAction ? $"code action: {CodeAction}" : "..." )}";

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;

            JToken? ICanBeResolved.Data
            {
                get => _codeAction?.Data;
                set {
                    if (_codeAction == null) return;
                    _codeAction.Data = value;
                }
            }
        }

        public partial class CodeActionContainer<T>
        {
            public static implicit operator CommandOrCodeActionContainer(CodeActionContainer<T> container) => new CodeActionContainer(container.Select(CodeAction.From));
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.CodeActionProvider))]
        [RegistrationOptionsConverter(typeof(CodeActionRegistrationOptionsConverter))]
        public partial class CodeActionRegistrationOptions : IWorkDoneProgressOptions, ITextDocumentRegistrationOptions
        {
            /// <summary>
            /// CodeActionKinds that this server may return.
            ///
            /// The list of kinds may be generic, such as `CodeActionKind.Refactor`, or the server
            /// may list out every specific kind they provide.
            /// </summary>
            [Optional]
            public Container<CodeActionKind>? CodeActionKinds { get; set; } = new Container<CodeActionKind>();

            /// <summary>
            /// The server provides support to resolve additional
            /// information for a code action.
            ///
            /// @since 3.16.0
            /// </summary>
            public bool ResolveProvider { get; set; }

            class CodeActionRegistrationOptionsConverter : RegistrationOptionsConverterBase<CodeActionRegistrationOptions, StaticOptions>
            {
                private readonly IHandlersManager _handlersManager;

                public CodeActionRegistrationOptionsConverter(IHandlersManager handlersManager) : base(nameof(ServerCapabilities.CodeActionProvider))
                {
                    _handlersManager = handlersManager;
                }

                public override StaticOptions Convert(CodeActionRegistrationOptions source)
                {
                    return new() {
                        CodeActionKinds = source.CodeActionKinds,
                        ResolveProvider = source.ResolveProvider || _handlersManager.Descriptors.Any(z => z.HandlerType == typeof(ICodeActionResolveHandler)),
                        WorkDoneProgress = source.WorkDoneProgress,
                    };
                }
            }
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.CodeAction))]
        public partial class CodeActionCapability : DynamicCapability, ConnectedCapability<ICodeActionHandler>
        {
            /// <summary>
            /// The client support code action literals as a valid
            /// response of the `textDocument/codeAction` request.
            ///
            /// Since 3.8.0
            /// </summary>
            [Optional]
            public CodeActionLiteralSupportOptions? CodeActionLiteralSupport { get; set; }

            /// <summary>
            /// Whether code action supports the `isPreferred` property.
            /// @since 3.15.0
            /// </summary>
            [Optional]
            public bool IsPreferredSupport { get; set; }

            /// <summary>
            ///  Whether code action supports the `disabled` property.
            ///
            /// @since 3.16.0 - proposed state
            /// </summary>
            [Optional]
            public bool DisabledSupport { get; set; }

            /// <summary>
            /// Whether code action supports the `data` property which is
            /// preserved between a `textDocument/codeAction` and a `codeAction/resolve` request.
            ///
            /// @since 3.16.0 - proposed state
            /// </summary>
            [Optional]
            public bool DataSupport { get; set; }

            /// <summary>
            /// Whether the client supports resolving additional code action
            /// properties via a separate `codeAction/resolve` request.
            ///
            /// @since 3.16.0 - proposed state
            /// </summary>
            [Optional]
            public CodeActionCapabilityResolveSupportOptions? ResolveSupport { get; set; }
        }
    }

    namespace Document
    {
    }
}
