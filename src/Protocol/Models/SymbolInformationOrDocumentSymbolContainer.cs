using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class SymbolInformationOrDocumentSymbolContainer : SymbolInformationOrDocumentSymbolContainer
    {
        public SymbolInformationOrDocumentSymbolContainer() : this(Enumerable.Empty<SymbolInformationOrDocumentSymbol>())
        {
        }

        public SymbolInformationOrDocumentSymbolContainer(IEnumerable<SymbolInformationOrDocumentSymbol> items) : base(items)
        {
        }

        public SymbolInformationOrDocumentSymbolContainer(params SymbolInformationOrDocumentSymbol[] items) : base(items)
        {
        }

        public static implicit operator SymbolInformationOrDocumentSymbolContainer(SymbolInformationOrDocumentSymbol[] items)
        {
            return new SymbolInformationOrDocumentSymbolContainer(items);
        }

        public static implicit operator SymbolInformationOrDocumentSymbolContainer(Collection<SymbolInformationOrDocumentSymbol> items)
        {
            return new SymbolInformationOrDocumentSymbolContainer(items);
        }

        public static implicit operator SymbolInformationOrDocumentSymbolContainer(List<SymbolInformationOrDocumentSymbol> items)
        {
            return new SymbolInformationOrDocumentSymbolContainer(items);
        }
    }
}
