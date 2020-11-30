using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Reflection;
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
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
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
        public partial record CodeActionParams : ITextDocumentIdentifierParams, IPartialItemsRequest<CommandOrCodeActionContainer, CommandOrCodeAction>, IWorkDoneProgressParams
        {
            /// <summary>
            /// The document in which the command was invoked.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; init; }

            /// <summary>
            /// The range for which the command was invoked.
            /// </summary>
            public Range Range { get; init; }

            /// <summary>
            /// Context carrying additional information.
            /// </summary>
            public CodeActionContext Context { get; init; }
        }

        /// <summary>
        /// Contains additional diagnostic information about the context in which
        /// a code action is run.
        /// </summary>
        public record CodeActionContext
        {
            /// <summary>
            /// An array of diagnostics known on the client side overlapping the range provided to the
            /// `textDocument/codeAction` request. They are provied so that the server knows which
            /// errors are currently presented to the user for the given range. There is no guarantee
            /// that these accurately reflect the error state of the resource. The primary parameter
            /// to compute code actions is the provided range.
            /// </summary>
            public Container<Diagnostic> Diagnostics { get; init; }

            /// <summary>
            /// Requested kind of actions to return.
            ///
            /// Actions not of this kind are filtered out by the client before being shown. So servers
            /// can omit computing them.
            /// </summary>
            [Optional]
            public Container<CodeActionKind>? Only { get; init; }
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
        public partial record CodeAction : ICanBeResolved, IRequest<CodeAction>
        {
            /// <summary>
            /// A short, human-readable, title for this code action.
            /// </summary>
            public string Title { get; init; }

            /// <summary>
            /// The kind of the code action.
            ///
            /// Used to filter code actions.
            /// </summary>
            [Optional]
            public CodeActionKind Kind { get; init; }

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
            public bool IsPreferred { get; init; }

            /// <summary>
            /// The diagnostics that this code action resolves.
            /// </summary>
            [Optional]
            public Container<Diagnostic>? Diagnostics { get; init; }

            /// <summary>
            /// The workspace edit this code action performs.
            /// </summary>
            [Optional]
            public WorkspaceEdit? Edit { get; init; }

            /// <summary>
            /// A command this code action executes. If a code action
            /// provides an edit and a command, first the edit is
            /// executed and then the command.
            /// </summary>
            [Optional]
            public Command? Command { get; init; }

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
            public CodeActionDisabled? Disabled { get; init; }

            /// <summary>
            /// A data entry field that is preserved on a document link between a
            /// DocumentLinkRequest and a DocumentLinkResolveRequest.
            /// </summary>
            [Optional]
            public JToken? Data { get; init; }

            private string DebuggerDisplay => $"[{Kind}] {Title}";

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;
        }

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
        public record CodeActionDisabled
        {
            /// <summary>
            /// Human readable description of why the code action is currently disabled.
            ///
            /// This is displayed in the code actions UI.
            /// </summary>
            public string Reason { get; init; }
        }

        [JsonConverter(typeof(CommandOrCodeActionConverter))]
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        [GenerateContainer]
        public record CommandOrCodeAction : ICanBeResolved // This to ensure that code actions get updated as expected
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

            public static CommandOrCodeAction From(Command value) => new(value);
            public static implicit operator CommandOrCodeAction(Command value) => new(value);

            public static CommandOrCodeAction From(CodeAction value) => new(value);
            public static implicit operator CommandOrCodeAction(CodeAction value) => new(value);
            public static CommandOrCodeAction From<T>(CodeAction<T> value) where T : class?, IHandlerIdentity? => new(value);

            private string DebuggerDisplay => $"{( IsCommand ? $"command: {Command}" : IsCodeAction ? $"code action: {CodeAction}" : "..." )}";

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;

            JToken? ICanBeResolved.Data
            {
                get => _codeAction?.Data;
                init {
                    if (_codeAction == null) return;
                    _codeAction = _codeAction with { Data = value };
                }
            }
        }

        public partial class CodeActionContainer<T>
        {
            public static implicit operator CommandOrCodeActionContainer(CodeActionContainer<T> container) => new(container.Select(CommandOrCodeAction.From));
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
            public Container<CodeActionKind>? CodeActionKinds { get; set; } = new();

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

    /// <summary>
    /// A set of predefined code action kinds
    /// </summary>
    [DebuggerDisplay("{" + nameof(_value) + "}")]
    [JsonConverter(typeof(EnumLikeStringConverter))]
    public readonly struct CodeActionKind : IEquatable<CodeActionKind>, IEnumLikeString
    {
        private static readonly Lazy<IReadOnlyList<CodeActionKind>> _defaults =
            new Lazy<IReadOnlyList<CodeActionKind>>(
                () => {
                    return typeof(CodeActionKind)
                          .GetFields(BindingFlags.Static | BindingFlags.Public)
                          .Select(z => z.GetValue(null))
                          .Cast<CodeActionKind>()
                          .ToArray();
                }
            );

        public static IEnumerable<CodeActionKind> Defaults => _defaults.Value;

        /// <summary>
        /// Base kind for quickfix actions: ''
        /// </summary>
        public static readonly CodeActionKind Empty = new CodeActionKind("");

        /// <summary>
        /// Base kind for quickfix actions: 'quickfix'
        /// </summary>
        public static readonly CodeActionKind QuickFix = new CodeActionKind("quickfix");

        /// <summary>
        /// Base kind for refactoring actions: 'refactor'
        /// </summary>
        public static readonly CodeActionKind Refactor = new CodeActionKind("refactor");

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
        public static readonly CodeActionKind RefactorExtract = new CodeActionKind("refactor.extract");

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
        public static readonly CodeActionKind RefactorInline = new CodeActionKind("refactor.inline");

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
        public static readonly CodeActionKind RefactorRewrite = new CodeActionKind("refactor.rewrite");

        /// <summary>
        /// Base kind for source actions: `source`
        ///
        /// Source code actions apply to the entire file.
        /// </summary>
        public static readonly CodeActionKind Source = new CodeActionKind("source");

        /// <summary>
        /// Base kind for an organize imports source action: `source.organizeImports`
        /// </summary>
        public static readonly CodeActionKind SourceOrganizeImports = new CodeActionKind("source.organizeImports");

        private readonly string? _value;

        public CodeActionKind(string kind) => _value = kind;

        public static implicit operator CodeActionKind(string kind) => new CodeActionKind(kind);

        public static implicit operator string(CodeActionKind kind) => kind._value ?? string.Empty;

        /// <inheritdoc />
        public override string ToString() => _value ?? string.Empty;

        public bool Equals(CodeActionKind other) => _value == other._value;

        public override bool Equals(object obj) => obj is CodeActionKind other && Equals(other);

        public override int GetHashCode() => _value != null ? _value.GetHashCode() : 0;

        public static bool operator ==(CodeActionKind left, CodeActionKind right) => left.Equals(right);

        public static bool operator !=(CodeActionKind left, CodeActionKind right) => !left.Equals(right);
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

        public class CodeActionLiteralSupportOptions
        {
            /// <summary>
            /// The code action kind is support with the following value
            /// set.
            /// </summary>
            public CodeActionKindCapabilityOptions CodeActionKind { get; set; } = null!;
        }

        public class CodeActionKindCapabilityOptions
        {
            /// <summary>
            /// The code action kind values the client supports. When this
            /// property exists the client also guarantees that it will
            /// handle values outside its set gracefully and falls back
            /// to a default value when unknown.
            /// </summary>
            public Container<CodeActionKind> ValueSet { get; set; } = null!;
        }

        /// <summary>
        /// Whether the client supports resolving additional code action
        /// properties via a separate `codeAction/resolve` request.
        ///
        /// @since 3.16.0 - proposed state
        /// </summary>
        public class CodeActionCapabilityResolveSupportOptions
        {
            /// <summary>
            /// The properties that a client can resolve lazily.
            /// </summary>
            public Container<string> Properties { get; set; } = new Container<string>();
        }
    }

    namespace Document
    {
    }
}
