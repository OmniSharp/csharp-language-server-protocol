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
        [Parallel]
        [Method(TextDocumentNames.Completion, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(CompletionRegistrationOptions)), Capability(typeof(CompletionCapability)), Resolver(typeof(CompletionItem))]
        public partial class CompletionParams : TextDocumentPositionParams, IWorkDoneProgressParams, IPartialItemsRequest<CompletionList, CompletionItem>
        {
            /// <summary>
            /// The completion context. This is only available it the client specifies to send
            /// this using `Capability.textDocument.completion.contextSupport === true`
            /// </summary>
            [Optional]
            public CompletionContext? Context { get; set; }
        }

        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        [Parallel]
        [Method(TextDocumentNames.CompletionResolve, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document", Name = "CompletionResolve"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient)),
            GenerateContainer("CompletionList"),
            GenerateTypedData
        ]
        [RegistrationOptions(typeof(CompletionRegistrationOptions)), Capability(typeof(CompletionCapability))]
        public partial class CompletionItem : ICanBeResolved, IRequest<CompletionItem>
        {
            /// <summary>
            /// The label of this completion item. By default
            /// also the text that is inserted when selecting
            /// this completion.
            /// </summary>
            public string Label { get; set; } = null!;

            /// <summary>
            /// The kind of this completion item. Based of the kind
            /// an icon is chosen by the editor.
            /// </summary>
            [Optional]
            public CompletionItemKind Kind { get; set; }

            /// <summary>
            /// Tags for this completion item.
            ///
            /// @since 3.15.0
            /// </summary>
            [Optional]
            public Container<CompletionItemTag>? Tags { get; set; }

            /// <summary>
            /// A human-readable string with additional information
            /// about this item, like type or symbol information.
            /// </summary>
            [Optional]
            public string? Detail { get; set; }

            /// <summary>
            /// A human-readable string that represents a doc-comment.
            /// </summary>
            [Optional]
            public StringOrMarkupContent? Documentation { get; set; }

            /// <summary>
            /// Indicates if this item is deprecated.
            /// </summary>
            [Optional]
            public bool Deprecated { get; set; }

            /// <summary>
            /// Select this item when showing.
            ///
            /// *Note* that only one completion item can be selected and that the
            /// tool / client decides which item that is. The rule is that the *first*
            /// item of those that match best is selected.
            /// </summary>
            [Optional]
            public bool Preselect { get; set; }

            /// <summary>
            /// A string that shoud be used when comparing this item
            /// with other items. When `falsy` the label is used.
            /// </summary>
            [Optional]
            public string? SortText { get; set; }

            /// <summary>
            /// A string that should be used when filtering a set of
            /// completion items. When `falsy` the label is used.
            /// </summary>

            [Optional]
            public string? FilterText { get; set; }

            /// <summary>
            /// A string that should be inserted a document when selecting
            /// this completion. When `falsy` the label is used.
            /// </summary>

            [Optional]
            public string? InsertText { get; set; }

            /// <summary>
            /// The format of the insert text. The format applies to both the `insertText` property
            /// and the `newText` property of a provided `textEdit`.
            /// </summary>
            [Optional]
            public InsertTextFormat InsertTextFormat { get; set; }

            /// <summary>
            /// How whitespace and indentation is handled during completion
            /// item insertion.
            ///
            /// @since 3.16.0 - proposed state
            /// </summary>
            [Optional]
            public InsertTextMode InsertTextMode { get; set; }

            /// <summary>
            /// An edit which is applied to a document when selecting this completion. When an edit is provided the value of
            /// `insertText` is ignored.
            ///
            /// *Note:* The range of the edit must be a single line range and it must contain the position at which completion
            /// has been requested.
            ///
            /// Most editors support two different operation when accepting a completion
            /// item. One is to insert a completion text and the other is to replace an
            /// existing text with a competion text. Since this can usually not
            /// predetermend by a server it can report both ranges. Clients need to
            /// signal support for `InsertReplaceEdits` via the
            /// `textDocument.completion.insertReplaceSupport` client capability
            /// property.
            ///
            /// *Note 1:* The text edit's range as well as both ranges from a insert
            /// replace edit must be a [single line] and they must contain the position
            /// at which completion has been requested.
            /// *Note 2:* If an `InsertReplaceEdit` is returned the edit's insert range
            /// must be a prefix of the edit's replace range, that means it must be
            /// contained and starting at the same position.
            ///
            /// @since 3.16.0 additional type `InsertReplaceEdit` - proposed state
            /// </summary>
            /// <remarks>
            /// TODO: Update this to union <see cref="TextEdit"/> <see cref="InsertReplaceEdit"/>
            /// </remarks>
            [Optional]
            public TextEdit? TextEdit { get; set; }

            /// <summary>
            /// An optional array of additional text edits that are applied when
            /// selecting this completion. Edits must not overlap with the main edit
            /// nor with themselves.
            /// </summary>
            [Optional]
            public TextEditContainer? AdditionalTextEdits { get; set; }

            /// <summary>
            /// An optional set of characters that when pressed while this completion is active will accept it first and
            /// then type that character. *Note* that all commit characters should have `length=1` and that superfluous
            /// characters will be ignored.
            /// </summary>
            [Optional]
            public Container<string>? CommitCharacters { get; set; }

            /// <summary>
            /// An optional command that is executed/// after* inserting this completion./// Note* that
            /// additional modifications to the current document should be described with the
            /// additionalTextEdits-property.
            /// </summary>
            [Optional]
            public Command? Command { get; set; }

            /// <summary>
            /// An data entry field that is preserved on a completion item between
            /// a completion and a completion resolve request.
            /// </summary>
            [Optional]
            public JToken? Data { get; set; }

            private string DebuggerDisplay => $"[{Kind}] {Label}{( Tags?.Any() == true ? $" tags: {string.Join(", ", Tags.Select(z => z.ToString()))}" : "" )}";

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.CompletionProvider))]
        [RegistrationOptionsConverter(typeof(CompletionRegistrationOptionsConverter))]
        public partial class CompletionRegistrationOptions : IWorkDoneProgressOptions, ITextDocumentRegistrationOptions
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
            public Container<string>? TriggerCharacters { get; set; }

            /// <summary>
            /// The list of all possible characters that commit a completion. This field can be used
            /// if clients don't support individual commmit characters per completion item. See
            /// `Capability.textDocument.completion.completionItem.commitCharactersSupport`
            ///
            /// @since 3.2.0
            /// </summary>
            [Optional]
            public Container<string>? AllCommitCharacters { get; set; }

            class CompletionRegistrationOptionsConverter : RegistrationOptionsConverterBase<CompletionRegistrationOptions, StaticOptions>
            {
                private readonly IHandlersManager _handlersManager;

                public CompletionRegistrationOptionsConverter(IHandlersManager handlersManager) : base(nameof(ServerCapabilities.CompletionProvider))
                {
                    _handlersManager = handlersManager;
                }

                public override StaticOptions Convert(CompletionRegistrationOptions source)
                {
                    return new() {
                        ResolveProvider = source.ResolveProvider || _handlersManager.Descriptors.Any(z => z.HandlerType == typeof(ICompletionResolveHandler)),
                        AllCommitCharacters = source.AllCommitCharacters,
                        TriggerCharacters = source.TriggerCharacters,
                        WorkDoneProgress = source.WorkDoneProgress,
                    };
                }
            }
        }

        /// <summary>
        /// Represents a collection of [completion items](#CompletionItem) to be presented
        /// in the editor.
        /// </summary>
        [JsonConverter(typeof(CompletionListConverter))]
        public partial class CompletionList
        {
            public CompletionList(bool isIncomplete): this(Enumerable.Empty<CompletionItem>(), isIncomplete)
            {
                IsIncomplete = isIncomplete;
            }

            public CompletionList(IEnumerable<CompletionItem> items, bool isIncomplete): base(items)
            {
                IsIncomplete = isIncomplete;
            }

            /// <summary>
            /// This list it not complete. Further typing should result in recomputing
            /// this list.
            /// </summary>
            public bool IsIncomplete { get; }

            /// <summary>
            /// The completion items.
            /// </summary>
            public IEnumerable<CompletionItem> Items => this;

            public static CompletionList? From<T>(CompletionList<T>? list) where T : HandlerIdentity?, new()
                => list switch {
                    not null => new(list.Items.Select(CompletionItem.From)!, list.IsIncomplete),
                    _        => null
                };
        }

        public partial class CompletionList<T>
        {
            public CompletionList(bool isIncomplete): this(isIncomplete, Enumerable.Empty<CompletionItem<T>>())
            {
                IsIncomplete = isIncomplete;
            }

            public CompletionList(bool isIncomplete, IEnumerable<CompletionItem<T>> items): base(items)
            {
                IsIncomplete = isIncomplete;
            }

            public CompletionList(bool isIncomplete, params CompletionItem<T>[] items): base(items)
            {
                IsIncomplete = isIncomplete;
            }

            /// <summary>
            /// This list it not complete. Further typing should result in recomputing
            /// this list.
            /// </summary>
            public bool IsIncomplete { get; }

            /// <summary>
            /// The completion items.
            /// </summary>
            public IEnumerable<CompletionItem<T>> Items => this;

            public static CompletionList<T>? Create(CompletionList? list)
                => list switch {
                    not null =>
                        new(list.IsIncomplete, list.Items.Select(CompletionItem<T>.From)!),
                    _ => null
                };
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.Completion))]
        public partial class CompletionCapability : DynamicCapability, ConnectedCapability<ICompletionHandler>
        {
            /// <summary>
            /// The client supports the following `CompletionItem` specific
            /// capabilities.
            /// </summary>
            [Optional]
            public CompletionItemCapabilityOptions? CompletionItem { get; set; }

            /// <summary>
            /// Specific capabilities for the `CompletionItemKind` in the `textDocument/completion` request.
            /// </summary>
            [Optional]
            public CompletionItemKindCapabilityOptions? CompletionItemKind { get; set; }

            /// <summary>
            /// The client supports to send additional context information for a `textDocument/completion` request.
            /// </summary>
            [Optional]
            public bool ContextSupport { get; set; }
        }
    public class CompletionItemCapabilityOptions
    {
        /// <summary>
        /// Client supports snippets as insert text.
        ///
        /// A snippet can define tab stops and placeholders with `$1`, `$2`
        /// and `${3:foo}`. `$0` defines the final tab stop, it defaults to
        /// the end of the snippet. Placeholders with equal identifiers are linked,
        /// that is typing in one will update others too.
        /// </summary>
        [Optional]
        public bool SnippetSupport { get; set; }

        /// <summary>
        /// Client supports commit characters on a completion item.
        /// </summary>
        [Optional]
        public bool CommitCharactersSupport { get; set; }

        /// <summary>
        /// Client supports the follow content formats for the documentation
        /// property. The order describes the preferred format of the client.
        /// </summary>
        [Optional]
        public Container<MarkupKind>? DocumentationFormat { get; set; }

        /// <summary>
        /// Client supports the deprecated property on a completion item.
        /// </summary>
        [Optional]
        public bool DeprecatedSupport { get; set; }

        /// <summary>
        /// Client supports the preselect property on a completion item.
        /// </summary>
        [Optional]
        public bool PreselectSupport { get; set; }

        /// <summary>
        /// Client supports the tag property on a completion item. Clients supporting
        /// tags have to handle unknown tags gracefully. Clients especially need to
        /// preserve unknown tags when sending a completion item back to the server in
        /// a resolve call.
        ///
        /// @since 3.15.0
        /// </summary>
        [Optional]
        public Supports<CompletionItemTagSupportCapabilityOptions?> TagSupport { get; set; }

        /// <summary>
        /// Client support insert replace edit to control different behavior if a
        /// completion item is inserted in the text or should replace text.
        ///
        /// @since 3.16.0 - Proposed state
        /// </summary>
        [Optional]
        public bool InsertReplaceSupport { get; set; }

        /// <summary>
        /// Client supports to resolve `additionalTextEdits` in the `completionItem/resolve`
        /// request. So servers can postpone computing them.
        ///
        /// @since 3.16.0 - Proposed state
        /// </summary>
        [Optional]
        public bool ResolveAdditionalTextEditsSupport { get; set; }

        /// <summary>
        /// Indicates which properties a client can resolve lazily on a completion
        /// item. Before version 3.16.0 only the predefined properties `documentation`
        /// and `details` could be resolved lazily.
        ///
        /// @since 3.16.0 - proposed state
        /// </summary>
        [Optional]
        public CompletionItemCapabilityResolveSupportOptions? ResolveSupport { get; set; }

        /// <summary>
        /// The client supports the `insertTextMode` property on
        /// a completion item to override the whitespace handling mode
        /// as defined by the client (see `insertTextMode`).
        ///
        /// @since 3.16.0 - proposed state
        /// </summary>
        [Optional]
        public CompletionItemInsertTextModeSupportCapabilityOptions? InsertTextModeSupport { get; set; }
    }

    public class CompletionItemInsertTextModeSupportCapabilityOptions
    {
        public Container<InsertTextMode> ValueSet { get; set; } = null!;
    }
    /// <summary>
    /// Indicates which properties a client can resolve lazily on a completion
    /// item. Before version 3.16.0 only the predefined properties `documentation`
    /// and `details` could be resolved lazily.
    ///
    /// @since 3.16.0 - proposed state
    /// </summary>
    public class CompletionItemCapabilityResolveSupportOptions
    {
        /// <summary>
        /// The properties that a client can resolve lazily.
        /// </summary>
        public Container<string> Properties { get; set; } = new Container<string>();
    }
    public class CompletionItemKindCapabilityOptions
    {
        /// <summary>
        /// The completion item kind values the client supports. When this
        /// property exists the client also guarantees that it will
        /// handle values outside its set gracefully and falls back
        /// to a default value when unknown.
        ///
        /// If this property is not present the client only supports
        /// the completion items kinds from `Text` to `Reference` as defined in
        /// the initial version of the protocol.
        /// </summary>
        [Optional]
        public Container<CompletionItemKind>? ValueSet { get; set; }
    }
    public class CompletionItemTagSupportCapabilityOptions
    {
        /// <summary>
        /// The tags supported by the client.
        /// </summary>
        public Container<CompletionItemTag> ValueSet { get; set; } = null!;
    }
    }

    namespace Document
    {
    }
}
