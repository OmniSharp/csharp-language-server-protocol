using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class ColorOptions : StaticTextDocumentRegistrationOptions, IColorOptions
    {
        public static ColorOptions Of(IColorOptions options)
        {
            return new ColorOptions() { };
        }
    }
}
