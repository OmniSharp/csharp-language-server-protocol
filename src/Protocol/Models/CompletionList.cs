using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    interface ICompletionList
    {
        /// <summary>
        /// This list it not complete. Further typing should result in recomputing
        /// this list.
        /// </summary>
        public bool IsIncomplete { get; }

        /// <summary>
        /// The completion items.
        /// </summary>
        public IEnumerable<object> Items { get; }
    }

    /// <summary>
    /// Represents a collection of [completion items](#CompletionItem) to be presented
    /// in the editor.
    /// </summary>
    public class CompletionList<TData> : Container<CompletionItem<TData>>, ICompletionList, IAggregateResults
        where TData : CanBeResolvedData
    {
        public CompletionList() : base(Enumerable.Empty<CompletionItem<TData>>())
        {
        }

        public CompletionList(bool isIncomplete) : base(Enumerable.Empty<CompletionItem<TData>>())
        {
            IsIncomplete = isIncomplete;
        }

        public CompletionList(IEnumerable<CompletionItem<TData>> items) : base(items)
        {
        }

        public CompletionList(IEnumerable<CompletionItem<TData>> items, bool isIncomplete) : base(items)
        {
            IsIncomplete = isIncomplete;
        }

        public CompletionList(params CompletionItem<TData>[] items) : base(items)
        {
        }

        public CompletionList(bool isIncomplete, params CompletionItem<TData>[] items) : base(items)
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
        public IEnumerable<CompletionItem<TData>> Items => this;

        IEnumerable<object> ICompletionList.Items => this;

        public static implicit operator CompletionList<TData>(CompletionItem<TData>[] items)
        {
            return new CompletionList<TData>(items);
        }

        public static implicit operator CompletionList<TData>(Collection<CompletionItem<TData>> items)
        {
            return new CompletionList<TData>(items);
        }

        public static implicit operator CompletionList<TData>(List<CompletionItem<TData>> items)
        {
            return new CompletionList<TData>(items);
        }

        public static implicit operator CompletionItem<TData>[](CompletionList<TData> list)
        {
            return list.ToArray();
        }

        object IAggregateResults.AggregateResults(IEnumerable<object> values)
        {
            return new CompletionList<CanBeResolvedData>(
                values
                    .Cast<IEnumerable<object>>()
                    .SelectMany(z => z.OfType<ICompletionItem<CanBeResolvedData>>())
                    .Concat(this)
                    .Select(CompletionItem<CanBeResolvedData>.From)
            );
        }
    }

    /// <summary>
    /// Represents a collection of [completion items](#CompletionItem) to be presented
    /// in the editor.
    /// </summary>
    public class CompletionList : CompletionList<ResolvedData>
    {
        public CompletionList() : base(Enumerable.Empty<CompletionItem<ResolvedData>>())
        {
        }

        public CompletionList(bool isIncomplete) : base(isIncomplete)
        {
        }

        public CompletionList(IEnumerable<CompletionItem<ResolvedData>> items) : base(items)
        {
        }

        public CompletionList(IEnumerable<CompletionItem<ResolvedData>> items, bool isIncomplete) : base(items, isIncomplete)
        {
        }

        public CompletionList(params CompletionItem<ResolvedData>[] items) : base(items)
        {
        }

        public CompletionList(bool isIncomplete, params CompletionItem<ResolvedData>[] items) : base(isIncomplete, items)
        {
        }

        public static implicit operator CompletionList(CompletionItem<ResolvedData>[] items)
        {
            return new CompletionList(items);
        }

        public static implicit operator CompletionList(Collection<CompletionItem<ResolvedData>> items)
        {
            return new CompletionList(items);
        }

        public static implicit operator CompletionList(List<CompletionItem<ResolvedData>> items)
        {
            return new CompletionList(items);
        }

        public static implicit operator CompletionItem<ResolvedData>[](CompletionList list)
        {
            return list.ToArray();
        }
    }
}
