using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class CodeLensContainer<TData> : Container<CodeLens<TData>>, IAggregateResults
        where TData : CanBeResolvedData
    {
        public CodeLensContainer() : this(Enumerable.Empty<CodeLens<TData>>())
        {
        }

        public CodeLensContainer(IEnumerable<CodeLens<TData>> items) : base(items)
        {
        }

        public CodeLensContainer(params CodeLens<TData>[] items) : base(items)
        {
        }

        public static implicit operator CodeLensContainer<TData>(CodeLens<TData>[] items)
        {
            return new CodeLensContainer<TData>(items);
        }

        public static implicit operator CodeLensContainer<TData>(Collection<CodeLens<TData>> items)
        {
            return new CodeLensContainer<TData>(items);
        }

        public static implicit operator CodeLensContainer<TData>(List<CodeLens<TData>> items)
        {
            return new CodeLensContainer<TData>(items);
        }

        object IAggregateResults.AggregateResults(IEnumerable<object> values)
        {
            return new CodeLensContainer<CanBeResolvedData>(
                values.Cast<IEnumerable<object>>()
                    .SelectMany(z => z.OfType<ICodeLens<CanBeResolvedData>>())
                    .Concat(this)
                    .Select(CodeLens<CanBeResolvedData>.From)
            );
        }
    }
}
