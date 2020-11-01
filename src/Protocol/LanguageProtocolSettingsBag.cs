using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    internal class LanguageProtocolSettingsBag : ILanguageProtocolSettings
    {
        public InitializeParams ClientSettings { get; internal set; } = null!;

        public InitializeResult ServerSettings { get; internal set; } = null!;
    }
}
