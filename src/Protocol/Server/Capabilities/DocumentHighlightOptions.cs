using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class DocumentHighlightOptions : WorkDoneProgressOptions, IDocumentHighlightOptions
    {
        public static DocumentHighlightOptions Of(IDocumentHighlightOptions options, IEnumerable<IHandlerDescriptor> descriptors)
        {
            return new DocumentHighlightOptions()
            {
                WorkDoneProgress = options.WorkDoneProgress
            };
        }
    }
}
