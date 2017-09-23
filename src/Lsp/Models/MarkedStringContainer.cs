using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServerProtocol.Converters;

namespace OmniSharp.Extensions.LanguageServerProtocol.Models
{
    [JsonConverter(typeof(MarkedStringCollectionConverter))]
    public class MarkedStringContainer : ContainerBase<MarkedString>
    {
        public MarkedStringContainer() : this(Enumerable.Empty<MarkedString>())
        {
        }

        public MarkedStringContainer(IEnumerable<MarkedString> items) : base(items)
        {
        }

        public MarkedStringContainer(params MarkedString[] items) : base(items)
        {
        }

        public static implicit operator MarkedStringContainer(MarkedString[] items)
        {
            return new MarkedStringContainer(items);
        }

        public static implicit operator MarkedStringContainer(Collection<MarkedString> items)
        {
            return new MarkedStringContainer(items);
        }

        public static implicit operator MarkedStringContainer(List<MarkedString> items)
        {
            return new MarkedStringContainer(items);
        }

        public static implicit operator MarkedStringContainer(MarkedString marked)
        {
            return new MarkedStringContainer(marked);
        }

        public static implicit operator MarkedStringContainer(string marked)
        {
            return new MarkedStringContainer(marked);
        }
    }
}