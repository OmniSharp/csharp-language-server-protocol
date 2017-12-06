using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentSymbolInformationContainer : Container<DocumentSymbolInformation>
    {
        public DocumentSymbolInformationContainer() : this(Enumerable.Empty<DocumentSymbolInformation>())
        {
        }

        public DocumentSymbolInformationContainer(IEnumerable<DocumentSymbolInformation> items) : base(items)
        {
        }

        public DocumentSymbolInformationContainer(params DocumentSymbolInformation[] items) : base(items)
        {
        }

        public static implicit operator DocumentSymbolInformationContainer(DocumentSymbolInformation[] items)
        {
            return new DocumentSymbolInformationContainer(items);
        }

        public static implicit operator DocumentSymbolInformationContainer(Collection<DocumentSymbolInformation> items)
        {
            return new DocumentSymbolInformationContainer(items);
        }

        public static implicit operator DocumentSymbolInformationContainer(List<DocumentSymbolInformation> items)
        {
            return new DocumentSymbolInformationContainer(items);
        }
    }
}
