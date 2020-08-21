using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ConfigurationItem : IEquatable<ConfigurationItem>
    {
        [Optional] public DocumentUri ScopeUri { get; set; }
        [Optional] public string Section { get; set; }

        public bool Equals(ConfigurationItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(ScopeUri, other.ScopeUri) && Section == other.Section;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ConfigurationItem) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ( ( ScopeUri != null ? ScopeUri.GetHashCode() : 0 ) * 397 ) ^ ( Section != null ? Section.GetHashCode() : 0 );
            }
        }

        public static bool operator ==(ConfigurationItem left, ConfigurationItem right) => Equals(left, right);

        public static bool operator !=(ConfigurationItem left, ConfigurationItem right) => !Equals(left, right);
    }
}
