using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class DocumentHighlightOptions : WorkDoneProgressOptions, IDocumentHighlightOptions
    {
        public static DocumentHighlightOptions Of(IDocumentHighlightOptions options)
        {
            return new DocumentHighlightOptions()
            {
                WorkDoneProgress = options.WorkDoneProgress
            };
        }
    }
}