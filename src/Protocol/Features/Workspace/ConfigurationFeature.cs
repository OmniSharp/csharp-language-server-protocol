using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(WorkspaceNames.Configuration, Direction.ServerToClient)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace"), GenerateHandlerMethods, GenerateRequestMethods(typeof(IWorkspaceLanguageServer), typeof(ILanguageServer))]
        public partial class ConfigurationParams : IRequest<Container<JToken>>
        {
            public Container<ConfigurationItem> Items { get; set; } = null!;
        }

        public class ConfigurationItem : IEquatable<ConfigurationItem?>
        {
            [Optional] public DocumentUri? ScopeUri { get; set; }
            [Optional] public string? Section { get; set; }

            public bool Equals(ConfigurationItem? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(ScopeUri, other.ScopeUri) && Section == other.Section;
            }

            public override bool Equals(object? obj)
            {
                if (obj is null) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((ConfigurationItem)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ( ( ScopeUri is not null ? ScopeUri.GetHashCode() : 0 ) * 397 ) ^ ( Section is not null ? Section.GetHashCode() : 0 );
                }
            }

            public static bool operator ==(ConfigurationItem left, ConfigurationItem right) => Equals(left, right);

            public static bool operator !=(ConfigurationItem left, ConfigurationItem right) => !Equals(left, right);
        }
    }
}
