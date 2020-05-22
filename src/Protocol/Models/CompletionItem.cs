using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.CompletionResolve, Direction.ClientToServer)]
    public class CompletionItem : ICanBeResolved, IRequest<CompletionItem>
    {
        /// <summary>
        /// The label of this completion item. By default
        /// also the text that is inserted when selecting
        /// this completion.
        /// </summary>
        public string Label { get; set; }

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
        public Container<CompletionItemTag> Tags { get; set; }

        /// <summary>
        /// A human-readable string with additional information
        /// about this item, like type or symbol information.
        /// </summary>
        [Optional]
        public string Detail { get; set; }

        /// <summary>
        /// A human-readable string that represents a doc-comment.
        /// </summary>
        [Optional]
        public StringOrMarkupContent Documentation { get; set; }

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
        public string SortText { get; set; }

        /// <summary>
        /// A string that should be used when filtering a set of
        /// completion items. When `falsy` the label is used.
        /// </summary>

        [Optional]
        public string FilterText { get; set; }

        /// <summary>
        /// A string that should be inserted a document when selecting
        /// this completion. When `falsy` the label is used.
        /// </summary>

        [Optional]
        public string InsertText { get; set; }

        /// <summary>
        /// The format of the insert text. The format applies to both the `insertText` property
        /// and the `newText` property of a provided `textEdit`.
        /// </summary>
        [Optional]
        public InsertTextFormat InsertTextFormat { get; set; }

        /// <summary>
        /// An edit which is applied to a document when selecting this completion. When an edit is provided the value of
        /// `insertText` is ignored.
        ///
        /// *Note:* The range of the edit must be a single line range and it must contain the position at which completion
        /// has been requested.
        /// </summary>
        [Optional]
        public TextEdit TextEdit { get; set; }

        /// <summary>
        /// An optional array of additional text edits that are applied when
        /// selecting this completion. Edits must not overlap with the main edit
        /// nor with themselves.
        /// </summary>
        [Optional]
        public TextEditContainer AdditionalTextEdits { get; set; }

        /// <summary>
        /// An optional set of characters that when pressed while this completion is active will accept it first and
        /// then type that character. *Note* that all commit characters should have `length=1` and that superfluous
        /// characters will be ignored.
        /// </summary>
        [Optional]
        public Container<string> CommitCharacters { get; set; }

        /// <summary>
        /// An optional command that is executed/// after* inserting this completion./// Note* that
        /// additional modifications to the current document should be described with the
        /// additionalTextEdits-property.
        /// </summary>
        [Optional]
        public Command Command { get; set; }

        /// <summary>
        /// An data entry field that is preserved on a completion item between
        /// a completion and a completion resolve request.
        /// </summary>
        [Optional]
        public JToken Data { get; set; }
    }
}
