using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ConfigurationItem
    {
        [Optional]
        public DocumentUri ScopeUri { get; set; }
        [Optional]
        public string Section { get; set; }
    }
}
