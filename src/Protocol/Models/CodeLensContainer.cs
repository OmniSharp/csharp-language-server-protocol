using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class CodeLensContainer : Container<CodeLens>
    {
        public CodeLensContainer() : this(Enumerable.Empty<CodeLens>())
        {
        }

        public CodeLensContainer(IEnumerable<CodeLens> items) : base(items)
        {
        }

        public CodeLensContainer(params CodeLens[] items) : base(items)
        {
        }

        public static implicit operator CodeLensContainer(CodeLens[] items) => new CodeLensContainer(items);

        public static implicit operator CodeLensContainer(Collection<CodeLens> items) => new CodeLensContainer(items);

        public static implicit operator CodeLensContainer(List<CodeLens> items) => new CodeLensContainer(items);
    }

    /// <remarks>
    /// Typed code lens used for the typed handlers
    /// </remarks>
    public class CodeLensContainer<T> : Container<CodeLens<T>> where T : HandlerIdentity, new()
    {
        public CodeLensContainer() : this(Enumerable.Empty<CodeLens<T>>())
        {
        }

        public CodeLensContainer(IEnumerable<CodeLens<T>> items) : base(items)
        {
        }

        public CodeLensContainer(params CodeLens<T>[] items) : base(items)
        {
        }

        public static implicit operator CodeLensContainer<T>(CodeLens<T>[] items) => new CodeLensContainer<T>(items);

        public static implicit operator CodeLensContainer<T>(Collection<CodeLens<T>> items) => new CodeLensContainer<T>(items);

        public static implicit operator CodeLensContainer<T>(List<CodeLens<T>> items) => new CodeLensContainer<T>(items);

        public static implicit operator CodeLensContainer(CodeLensContainer<T> container) => new CodeLensContainer(container.Select(z => (CodeLens) z));
    }
}
