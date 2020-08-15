using System;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Server.Configuration
{
    class ConfigurationItemData : IEquatable<ConfigurationItemData>
    {
        public ConfigurationItemData(ConfigurationItem configurationItem)
        {
            ConfigurationItem = configurationItem;
        }

        public ConfigurationItem ConfigurationItem { get; }
        public DocumentUri ScopeUri => ConfigurationItem.ScopeUri;
        public string Section => ConfigurationItem.Section;

        public bool Equals(ConfigurationItemData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ConfigurationItem.Equals(other.ConfigurationItem);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ConfigurationItemData) obj);
        }

        public override int GetHashCode() => ConfigurationItem.GetHashCode();

        public static bool operator ==(ConfigurationItemData left, ConfigurationItemData right) => Equals(left, right);

        public static bool operator !=(ConfigurationItemData left, ConfigurationItemData right) => !Equals(left, right);
    }
}