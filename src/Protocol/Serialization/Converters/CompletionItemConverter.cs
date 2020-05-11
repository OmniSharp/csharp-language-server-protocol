using System.Reflection;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class CompletionItemConverter : ConstrainedConverter<CompletionItem>
    {
        private readonly CompletionItemKind[] _completionItemKinds;
        private readonly CompletionItemTag[] _completionItemTags;

        public CompletionItemConverter(
            CompletionItemKind[] completionItemKinds,
            CompletionItemTag[] completionItemTags)
        {
            _completionItemKinds = completionItemKinds;
            _completionItemTags = completionItemTags;
        }

        protected override void ProcessProperty(string name, PropertyInfo propertyInfo, ref object value)
        {
            if (propertyInfo.PropertyType == typeof(CompletionItemKind))
            {
                value = ValidValue(_completionItemKinds, value);
            }

            if (propertyInfo.PropertyType == typeof(Container<CompletionItemTag>))
            {
                value = new Container<CompletionItemTag>(ValidValues(_completionItemTags, value));
            }
        }
    }
}