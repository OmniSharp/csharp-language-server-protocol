using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class DocumentRangeFormattingOptions : WorkDoneProgressOptions, IDocumentRangeFormattingOptions
    {
        public static DocumentRangeFormattingOptions Of(IDocumentRangeFormattingOptions options)
        {
            return new DocumentRangeFormattingOptions()
            {
                WorkDoneProgress = options.WorkDoneProgress,
                
            };
        }
    }
}
