using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading.Tasks;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static WorkspaceNames;
    public static partial class WorkspaceNames
    {
        public const string WorkspaceFolders = "workspace/workspaceFolders";
    }

    public static class WorkspaceFoldersExtensions
    {
        public static Task<Container<WorkspaceFolder>> WorkspaceFolders(this IResponseRouter mediator)
        {
            return mediator.SendRequest<Container<WorkspaceFolder>>(WorkspaceNames.WorkspaceFolders);
        }
    }
}

