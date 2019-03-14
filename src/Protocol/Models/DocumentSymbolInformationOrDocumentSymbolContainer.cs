using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentSymbolInformationOrDocumentSymbolContainer : Container<DocumentSymbolInformationOrDocumentSymbol>
    {
        public DocumentSymbolInformationOrDocumentSymbolContainer() : this(Enumerable.Empty<DocumentSymbolInformationOrDocumentSymbol>())
        {
        }

        public DocumentSymbolInformationOrDocumentSymbolContainer(IEnumerable<DocumentSymbolInformationOrDocumentSymbol> items) : base(items)
        {
        }

        public DocumentSymbolInformationOrDocumentSymbolContainer(params DocumentSymbolInformationOrDocumentSymbol[] items) : base(items)
        {
        }

        public static implicit operator DocumentSymbolInformationOrDocumentSymbolContainer(DocumentSymbolInformationOrDocumentSymbol[] items)
        {
            return new DocumentSymbolInformationOrDocumentSymbolContainer(items);
        }

        public static implicit operator DocumentSymbolInformationOrDocumentSymbolContainer(Collection<DocumentSymbolInformationOrDocumentSymbol> items)
        {
            return new DocumentSymbolInformationOrDocumentSymbolContainer(items);
        }

        public static implicit operator DocumentSymbolInformationOrDocumentSymbolContainer(List<DocumentSymbolInformationOrDocumentSymbol> items)
        {
            return new DocumentSymbolInformationOrDocumentSymbolContainer(items);
        }
    }
}
