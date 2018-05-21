using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class DidChangeWorkspaceFoldersExtensions
    {
        public static void DidChangeWorkspaceFolders(this ILanguageClientWorkspace router, DidChangeWorkspaceFoldersParams @params)
        {
            router.SendNotification(WorkspaceNames.DidChangeWorkspaceFolders, @params);
        }
    }
}
