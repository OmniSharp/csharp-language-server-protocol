using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class TypeDefinitionOptions : StaticWorkDoneTextDocumentRegistrationOptions, ITypeDefinitionOptions
    {
        public static TypeDefinitionOptions Of(ITypeDefinitionOptions options)
        {
            return new TypeDefinitionOptions()
            {
                WorkDoneProgress = options.WorkDoneProgress
            };
        }
    }
}
