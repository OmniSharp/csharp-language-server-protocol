using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class SelectionRangeOptions : StaticWorkDoneTextDocumentRegistrationOptions, ISelectionRangeOptions
    {
        public static SelectionRangeOptions Of(ISelectionRangeOptions options)
        {
            return new SelectionRangeOptions()
            {
                WorkDoneProgress = options.WorkDoneProgress,
            };
        }
    }
}