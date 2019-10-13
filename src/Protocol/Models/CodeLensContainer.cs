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

        public static implicit operator CodeLensContainer(CodeLens[] items)
        {
            return new CodeLensContainer(items);
        }

        public static implicit operator CodeLensContainer(Collection<CodeLens> items)
        {
            return new CodeLensContainer(items);
        }

        public static implicit operator CodeLensContainer(List<CodeLens> items)
        {
            return new CodeLensContainer(items);
        }
    }
}
