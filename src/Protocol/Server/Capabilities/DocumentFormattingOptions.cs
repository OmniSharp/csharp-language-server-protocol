using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class DocumentFormattingOptions : WorkDoneProgressOptions, IDocumentFormattingOptions
    {
        public static DocumentFormattingOptions Of(IDocumentFormattingOptions options)
        {
            return new DocumentFormattingOptions()
            {
                WorkDoneProgress = options.WorkDoneProgress
            };
        }
    }
}
