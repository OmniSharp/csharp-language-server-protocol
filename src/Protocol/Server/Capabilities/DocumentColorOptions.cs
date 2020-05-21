using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class DocumentColorOptions : StaticWorkDoneTextDocumentRegistrationOptions, IDocumentColorOptions
    {
        public static DocumentColorOptions Of(IDocumentColorOptions options, IEnumerable<IHandlerDescriptor> descriptors)
        {
            return new DocumentColorOptions() {
                WorkDoneProgress = options.WorkDoneProgress,
            };
        }
    }
}
