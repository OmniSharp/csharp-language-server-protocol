using System.Collections.Generic;
using System.Linq;

namespace Lsp.Models
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
    }
}