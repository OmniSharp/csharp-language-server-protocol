using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public record ConfigurationItem
    {
        public ConfigurationItem() { }

        public ConfigurationItem(string section)
        {
            Section = section;
        }

        public ConfigurationItem(string section, DocumentUri scopeUri): this(section)
        {
            ScopeUri = scopeUri;
        }

        [Optional] public DocumentUri? ScopeUri { get; init; }
        [Optional] public string? Section { get; init; }
    }
}
