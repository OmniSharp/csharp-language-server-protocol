using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class TypeDefinitionOptions : StaticWorkDoneTextDocumentRegistrationOptions, ITypeDefinitionOptions
    {
        public static TypeDefinitionOptions Of(ITypeDefinitionOptions options, IEnumerable<IHandlerDescriptor> descriptors)
        {
            return new TypeDefinitionOptions()
            {
                WorkDoneProgress = options.WorkDoneProgress
            };
        }
    }
}
