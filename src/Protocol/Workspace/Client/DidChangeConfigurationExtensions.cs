using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class DidChangeConfigurationExtensions
    {
        public static void DidChangeConfiguration(this ILanguageClientWorkspace router, DidChangeConfigurationParams @params)
        {
            router.SendNotification(WorkspaceNames.DidChangeConfiguration, @params);
        }
    }
}
