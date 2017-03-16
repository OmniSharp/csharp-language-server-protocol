using System.Collections.Generic;
using System.Linq;

namespace Lsp.Models
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
    }
}