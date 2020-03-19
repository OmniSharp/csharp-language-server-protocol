using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class ImplementationOptions : StaticWorkDoneTextDocumentRegistrationOptions, IImplementationOptions
    {
        public static ImplementationOptions Of(IImplementationOptions options)
        {
            return new ImplementationOptions()
            {
                WorkDoneProgress = options.WorkDoneProgress,

            };
        }
    }
}
