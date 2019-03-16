using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentSymbolInformationContainer : Container<SymbolInformationOrDocumentSymbol>
    {
        public DocumentSymbolInformationContainer() : this(Enumerable.Empty<SymbolInformationOrDocumentSymbol>())
        {
        }

        public DocumentSymbolInformationContainer(IEnumerable<SymbolInformationOrDocumentSymbol> items) : base(items)
        {
        }

        public DocumentSymbolInformationContainer(params SymbolInformationOrDocumentSymbol[] items) : base(items)
        {
        }

        public static implicit operator DocumentSymbolInformationContainer(SymbolInformationOrDocumentSymbol[] items)
        {
            return new DocumentSymbolInformationContainer(items);
        }

        public static implicit operator DocumentSymbolInformationContainer(Collection<SymbolInformationOrDocumentSymbol> items)
        {
            return new DocumentSymbolInformationContainer(items);
        }

        public static implicit operator DocumentSymbolInformationContainer(List<SymbolInformationOrDocumentSymbol> items)
        {
            return new DocumentSymbolInformationContainer(items);
        }
    }
}
