using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class ColorOptions : IColorOptions
    {
        public static StaticColorOptions Of(IColorOptions options)
        {
            return new StaticColorOptions() { };
        }
    }
}
