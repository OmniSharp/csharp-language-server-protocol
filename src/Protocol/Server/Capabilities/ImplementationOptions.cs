using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class ImplementationOptions : StaticWorkDoneTextDocumentRegistrationOptions, IImplementationOptions
    {
        public static ImplementationOptions Of(IImplementationOptions options, IEnumerable<IHandlerDescriptor> descriptors)
        {
            return new ImplementationOptions()
            {
                WorkDoneProgress = options.WorkDoneProgress,


            };
        }
    }
}
