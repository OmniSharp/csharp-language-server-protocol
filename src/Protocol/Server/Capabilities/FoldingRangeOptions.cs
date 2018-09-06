using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class FoldingRangeOptions : StaticTextDocumentRegistrationOptions, IFoldingRangeOptions
    {
        public static FoldingRangeOptions Of(IFoldingRangeOptions options)
        {
            return new FoldingRangeOptions() { };
        }
    }
}
