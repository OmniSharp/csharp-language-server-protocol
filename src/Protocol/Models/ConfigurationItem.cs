using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ConfigurationItem
    {
        [Optional]
        public Uri ScopeUri { get; set; }
        [Optional]
        public string Section { get; set; }
    }
}
