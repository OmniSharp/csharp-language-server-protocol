using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class DocumentFormattingOptions : WorkDoneProgressOptions, IDocumentFormattingOptions
    {
        public static DocumentFormattingOptions Of(IDocumentFormattingOptions options, IEnumerable<IHandlerDescriptor> descriptors)
        {
            return new DocumentFormattingOptions()
            {
                WorkDoneProgress = options.WorkDoneProgress
            };
        }
    }
}
