using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class SymbolInformationOrDocumentSymbolContainer : Container<SymbolInformationOrDocumentSymbol>
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

        public static implicit operator SymbolInformationOrDocumentSymbolContainer(SymbolInformationOrDocumentSymbol[] items) =>
            new SymbolInformationOrDocumentSymbolContainer(items);

        public static implicit operator SymbolInformationOrDocumentSymbolContainer(Collection<SymbolInformationOrDocumentSymbol> items) =>
            new SymbolInformationOrDocumentSymbolContainer(items);

        public static implicit operator SymbolInformationOrDocumentSymbolContainer(List<SymbolInformationOrDocumentSymbol> items) =>
            new SymbolInformationOrDocumentSymbolContainer(items);

        public static implicit operator SymbolInformationOrDocumentSymbolContainer(SymbolInformation[] items) =>
            new SymbolInformationOrDocumentSymbolContainer(items.Select(SymbolInformationOrDocumentSymbol.Create));

        public static implicit operator SymbolInformationOrDocumentSymbolContainer(Collection<SymbolInformation> items) =>
            new SymbolInformationOrDocumentSymbolContainer(items.Select(SymbolInformationOrDocumentSymbol.Create));

        public static implicit operator SymbolInformationOrDocumentSymbolContainer(List<SymbolInformation> items) =>
            new SymbolInformationOrDocumentSymbolContainer(items.Select(SymbolInformationOrDocumentSymbol.Create));

        public static implicit operator SymbolInformationOrDocumentSymbolContainer(DocumentSymbol[] items) =>
            new SymbolInformationOrDocumentSymbolContainer(items.Select(SymbolInformationOrDocumentSymbol.Create));

        public static implicit operator SymbolInformationOrDocumentSymbolContainer(Collection<DocumentSymbol> items) =>
            new SymbolInformationOrDocumentSymbolContainer(items.Select(SymbolInformationOrDocumentSymbol.Create));

        public static implicit operator SymbolInformationOrDocumentSymbolContainer(List<DocumentSymbol> items) =>
            new SymbolInformationOrDocumentSymbolContainer(items.Select(SymbolInformationOrDocumentSymbol.Create));
    }
}
