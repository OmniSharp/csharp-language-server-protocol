using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Converters;

namespace OmniSharp.Extensions.LanguageServer.Models
{
    /// <summary>
    /// Represents a collection of [completion items](#CompletionItem) to be presented
    /// in the editor.
    /// </summary>
    [JsonConverter(typeof(CompletionListConverter))]
    public class CompletionList
    {
        public CompletionList(IEnumerable<CompletionItem> items)
        {
            Items = items;
        }

        public CompletionList(IEnumerable<CompletionItem> items, bool isIncomplete) : this(items)
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
        public IEnumerable<CompletionItem> Items { get; }

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
            return list.Items.ToArray();
        }
    }
}