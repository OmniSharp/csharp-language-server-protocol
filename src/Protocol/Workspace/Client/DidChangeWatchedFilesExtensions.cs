using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class DidChangeWatchedFilesExtensions
    {
        public static void DidChangeWatchedFiles(this ILanguageClientWorkspace router, DidChangeWatchedFilesParams @params)
        {
            router.SendNotification(WorkspaceNames.DidChangeWatchedFiles, @params);
        }
    }
}
