using System;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ConfigurationItem
    {
        [Optional]
        [JsonConverter(typeof(AbsoluteUriConverter))]
        public Uri ScopeUri { get; set; }
        [Optional]
        public string Section { get; set; }
    }
}
