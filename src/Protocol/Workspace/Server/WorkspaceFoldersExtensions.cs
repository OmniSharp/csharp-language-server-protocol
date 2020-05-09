using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading.Tasks;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public static class WorkspaceFoldersExtensions
    {
        public static Task<Container<WorkspaceFolder>> WorkspaceFolders(this ILanguageServerWorkspace mediator, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(new WorkspaceFolderParams(), cancellationToken);
        }
    }
}

