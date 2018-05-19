using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading.Tasks;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    using static WorkspaceNames;
    public static class WorkspaceFoldersExtensions
    {
        public static Task<Container<WorkspaceFolder>> WorkspaceFolders(this ILanguageServerWorkspace mediator)
        {
            return mediator.SendRequest<Container<WorkspaceFolder>>(WorkspaceNames.WorkspaceFolders);
        }
    }
}

