using System.Reflection;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class DiagnosticConverter : ConstrainedConverter<Diagnostic>
    {
        private readonly DiagnosticTag[] _diagnosticTags;

        public DiagnosticConverter(DiagnosticTag[] diagnosticTags)
        {
            _diagnosticTags = diagnosticTags;
        }

        protected override void ProcessProperty(string name, PropertyInfo propertyInfo, ref object value)
        {
            if (propertyInfo.PropertyType == typeof(Container<DiagnosticTag>))
            {
                value = new Container<DiagnosticTag>(ValidValues(_diagnosticTags, value));
            }
        }
    }
}