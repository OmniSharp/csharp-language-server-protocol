using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class DocumentSymbolOptionsOptions : WorkDoneProgressOptions, IDocumentSymbolOptionsOptions
    {
        public static DocumentSymbolOptionsOptions Of(IDocumentSymbolOptionsOptions options, IEnumerable<IHandlerDescriptor> descriptors) =>
            new DocumentSymbolOptionsOptions {
                WorkDoneProgress = options.WorkDoneProgress
            };
    }
}
