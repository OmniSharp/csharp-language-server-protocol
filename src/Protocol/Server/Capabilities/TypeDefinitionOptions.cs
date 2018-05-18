using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class TypeDefinitionOptions : StaticTextDocumentRegistrationOptions, ITypeDefinitionOptions
    {
        public static TypeDefinitionOptions Of(ITypeDefinitionOptions options)
        {
            return new TypeDefinitionOptions();
        }
    }
}
