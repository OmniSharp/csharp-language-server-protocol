using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class DefinitionOptions : StaticWorkDoneTextDocumentRegistrationOptions, IDefinitionOptions
    {
        public static DefinitionOptions Of(IDefinitionOptions options, IEnumerable<IHandlerDescriptor> descriptors) =>
            new DefinitionOptions {
                WorkDoneProgress = options.WorkDoneProgress,
            };
    }
}
