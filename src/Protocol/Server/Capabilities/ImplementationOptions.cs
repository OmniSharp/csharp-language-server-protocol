using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class ImplementationOptions : StaticTextDocumentRegistrationOptions, IImplementationOptions
    {
        public static ImplementationOptions Of(IImplementationOptions options)
        {
            return new ImplementationOptions();
        }
    }
}
