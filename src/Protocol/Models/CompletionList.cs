using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Represents a collection of [completion items](#CompletionItem) to be presented
    /// in the editor.
    /// </summary>
    [JsonConverter(typeof(CompletionListConverter))]
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

    /// <summary>
    /// Represents a collection of [completion items](#CompletionItem) to be presented
    /// in the editor.
    /// </summary>
    public class CompletionList<T> : Container<CompletionItem<T>> where T : HandlerIdentity, new()
    {
        public CompletionList() : base(Enumerable.Empty<CompletionItem<T>>()) { }
        public CompletionList(bool isIncomplete) : base(Enumerable.Empty<CompletionItem<T>>())
        {
            IsIncomplete = isIncomplete;
        }

        public CompletionList(IEnumerable<CompletionItem<T>> items) : base(items) { }
        public CompletionList(IEnumerable<CompletionItem<T>> items, bool isIncomplete) : base(items)
        {
            IsIncomplete = isIncomplete;
        }

        public CompletionList(params CompletionItem<T>[] items) : base(items) { }
        public CompletionList(bool isIncomplete, params CompletionItem<T>[] items) : base(items)
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

        public static implicit operator CompletionList<T>(CompletionItem<T>[] items)
        {
            return new CompletionList<T>(items);
        }

        public static implicit operator CompletionList<T>(Collection<CompletionItem<T>> items)
        {
            return new CompletionList<T>(items);
        }

        public static implicit operator CompletionList<T>(List<CompletionItem<T>> items)
        {
            return new CompletionList<T>(items);
        }

        public static implicit operator CompletionItem<T>[] (CompletionList<T> list)
        {
            return list.ToArray();
        }

        public static implicit operator CompletionList(CompletionList<T> container) => new CompletionList(container.Select(z => (CompletionItem)z));
    }
}
