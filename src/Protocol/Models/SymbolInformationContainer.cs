using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Models
{
    public class SymbolInformationContainer : Container<SymbolInformation>
    {
        public SymbolInformationContainer() : this(Enumerable.Empty<SymbolInformation>())
        {
        }

        public SymbolInformationContainer(IEnumerable<SymbolInformation> items) : base(items)
        {
        }

        public SymbolInformationContainer(params SymbolInformation[] items) : base(items)
        {
        }

        public static implicit operator SymbolInformationContainer(SymbolInformation[] items)
        {
            return new SymbolInformationContainer(items);
        }

        public static implicit operator SymbolInformationContainer(Collection<SymbolInformation> items)
        {
            return new SymbolInformationContainer(items);
        }

        public static implicit operator SymbolInformationContainer(List<SymbolInformation> items)
        {
            return new SymbolInformationContainer(items);
        }
    }
}