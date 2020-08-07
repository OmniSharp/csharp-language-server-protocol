using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    class LanguageProtocolSettingsBag : ILanguageProtocolSettings
    {
        public InitializeParams ClientSettings { get; internal set; }

        public InitializeResult ServerSettings { get; internal set; }
    }
}
