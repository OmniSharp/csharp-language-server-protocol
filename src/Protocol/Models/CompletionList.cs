using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Represents a collection of [completion items](#CompletionItem) to be presented
    /// in the editor.
    /// </summary>
    public class CompletionList : Container<CompletionItem>
    {
        public CompletionList() : base(Enumerable.Empty<CompletionItem>()) { }
        public CompletionList(bool isIncomplete) : base(Enumerable.Empty<CompletionItem>())
        {
            IsIncomplete = isIncomplete;
        }

        public CompletionList(IEnumerable<CompletionItem> items) : base(items) { }
        public CompletionList(IEnumerable<CompletionItem> items, bool isIncomplete) : base(items)
        {
            IsIncomplete = isIncomplete;
        }

        public CompletionList(params CompletionItem[] items) : base(items) { }
        public CompletionList(bool isIncomplete, params CompletionItem[] items) : base(items)
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

        public static implicit operator CompletionList(CompletionItem[] items)
        {
            return new CompletionList(items);
        }

        public static implicit operator CompletionList(Collection<CompletionItem> items)
        {
            return new CompletionList(items);
        }

        public static implicit operator CompletionList(List<CompletionItem> items)
        {
            return new CompletionList(items);
        }

        public static implicit operator CompletionItem[] (CompletionList list)
        {
            return list.ToArray();
        }
    }
}
