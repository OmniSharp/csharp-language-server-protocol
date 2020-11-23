using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(WorkspaceNames.WorkspaceFolders, Direction.ServerToClient)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace", Name = "WorkspaceFolders"), GenerateHandlerMethods, GenerateRequestMethods(typeof(IWorkspaceLanguageServer), typeof(ILanguageServer))]
        public partial class WorkspaceFolderParams : IRequest<Container<WorkspaceFolder>?>
        {
            public static WorkspaceFolderParams Instance = new WorkspaceFolderParams();
        }

        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public partial class WorkspaceFolder : IEquatable<WorkspaceFolder>
        {
            /// <summary>
            /// The associated URI for this workspace folder.
            /// </summary>
            public DocumentUri Uri { get; set; } = null!;

            /// <summary>
            /// The name of the workspace folder. Defaults to the uri's basename.
            /// </summary>
            public string Name { get; set; } = null!;

            public bool Equals(WorkspaceFolder? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(Uri, other.Uri) && Name == other.Name;
            }

            public override bool Equals(object? obj)
            {
                if (obj is null) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((WorkspaceFolder) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ( ( Uri is not null ? Uri.GetHashCode() : 0 ) * 397 ) ^ ( Name != null ? Name.GetHashCode() : 0 );
                }
            }

            public static bool operator ==(WorkspaceFolder left, WorkspaceFolder right) => Equals(left, right);

            public static bool operator !=(WorkspaceFolder left, WorkspaceFolder right) => !Equals(left, right);

            private string DebuggerDisplay => $"{Name} ({Uri})";

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;
        }
    }
}
