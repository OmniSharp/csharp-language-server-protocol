using System.Reflection;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class DocumentSymbolConverter : ConstrainedConverter<DocumentSymbol>
    {
        private readonly SymbolKind[] _symbolKinds;
        private readonly SymbolTag[] _symbolTags;

        public DocumentSymbolConverter(SymbolKind[] symbolKinds, SymbolTag[] symbolTags)
        {
            _symbolKinds = symbolKinds;
            _symbolTags = symbolTags;
        }

        protected override void ProcessProperty(string name, PropertyInfo propertyInfo, ref object value)
        {
            if (propertyInfo.PropertyType == typeof(SymbolKind))
            {
                value = ValidValue(_symbolKinds, value);
            }

            if (propertyInfo.PropertyType == typeof(Container<SymbolTag>))
            {
                value = new Container<SymbolTag>(ValidValues(_symbolTags, value));
            }
        }
    }
}