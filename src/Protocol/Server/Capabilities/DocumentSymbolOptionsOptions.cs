using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class DocumentSymbolOptionsOptions : WorkDoneProgressOptions, IDocumentSymbolOptionsOptions
    {
        public static DocumentSymbolOptionsOptions Of(IDocumentSymbolOptionsOptions options)
        {
            return new DocumentSymbolOptionsOptions()
            {
                WorkDoneProgress = options.WorkDoneProgress
            };
        }
    }
}