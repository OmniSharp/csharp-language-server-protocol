using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// A collection of document filters used to identify valid documents
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class DocumentSelector : ContainerBase<DocumentFilter>
    {
        public DocumentSelector() : this(Enumerable.Empty<DocumentFilter>())
        {
        }

        public DocumentSelector(IEnumerable<DocumentFilter> items) : base(items)
        {
        }

        public DocumentSelector(params DocumentFilter[] items) : base(items)
        {
        }

        public static implicit operator DocumentSelector(DocumentFilter[] items) => new DocumentSelector(items);

        public static implicit operator DocumentSelector(Collection<DocumentFilter> items) => new DocumentSelector(items);

        public static implicit operator DocumentSelector(List<DocumentFilter> items) => new DocumentSelector(items);

        public static implicit operator string(DocumentSelector? documentSelector) =>
            documentSelector is not null ? string.Join(", ", documentSelector.Select(x => (string) x)) : string.Empty;

        public bool IsMatch(TextDocumentAttributes attributes) => this.Any(z => z.IsMatch(attributes));

        public override string ToString() => this;

        public static DocumentSelector ForPattern(params string[] wildcards) => new DocumentSelector(wildcards.Select(DocumentFilter.ForPattern));

        public static DocumentSelector ForLanguage(params string[] languages) => new DocumentSelector(languages.Select(DocumentFilter.ForLanguage));

        public static DocumentSelector ForScheme(params string[] schemes) => new DocumentSelector(schemes.Select(DocumentFilter.ForScheme));

        private string DebuggerDisplay => this;
    }
}
