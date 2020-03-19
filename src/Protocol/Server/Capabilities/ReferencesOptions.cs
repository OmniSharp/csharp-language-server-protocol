using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class ReferencesOptions : WorkDoneProgressOptions, IReferencesOptions
    {
        public static ReferencesOptions Of(IReferencesOptions options)
        {
            return new ReferencesOptions()
            {
                WorkDoneProgress = options.WorkDoneProgress,
                
            };
        }
    }
}
