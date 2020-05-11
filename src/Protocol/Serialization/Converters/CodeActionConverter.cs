using System.Reflection;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class CodeActionConverter : ConstrainedConverter<CodeAction>
    {
        private readonly CodeActionKind[] _codeActionKinds;

        public CodeActionConverter(CodeActionKind[] codeActionKinds)
        {
            _codeActionKinds = codeActionKinds;
        }

        protected override void ProcessProperty(string name, PropertyInfo propertyInfo, ref object value)
        {
            if (propertyInfo.PropertyType == typeof(CodeActionKind))
            {
                value = ValidValue(_codeActionKinds, value);
            }
        }
    }
}