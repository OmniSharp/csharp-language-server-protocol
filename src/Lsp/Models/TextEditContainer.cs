using System.Collections.Generic;
using System.Linq;

namespace Lsp.Models
{
    public class TextEditContainer : Container<TextEdit>
    {
        public TextEditContainer() : this(Enumerable.Empty<TextEdit>())
        {
        }

        public TextEditContainer(IEnumerable<TextEdit> items) : base(items)
        {
        }

        public TextEditContainer(params TextEdit[] items) : base(items)
        {
        }

        public static implicit operator TextEditContainer(TextEdit[] items)
        {
            return new TextEditContainer(items);
        }
    }
}