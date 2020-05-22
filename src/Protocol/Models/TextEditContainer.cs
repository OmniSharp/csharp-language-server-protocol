using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
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

        public static implicit operator TextEditContainer(Collection<TextEdit> items)
        {
            return new TextEditContainer(items);
        }

        public static implicit operator TextEditContainer(List<TextEdit> items)
        {
            return new TextEditContainer(items);
        }
    }
}
