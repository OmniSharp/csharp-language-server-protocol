using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class HoverOptions : WorkDoneProgressOptions, IHoverOptions
    {
        public static HoverOptions Of(IHoverOptions options)
        {
            return new HoverOptions()
            {
                WorkDoneProgress = options.WorkDoneProgress,
                
            };
        }
    }
}
