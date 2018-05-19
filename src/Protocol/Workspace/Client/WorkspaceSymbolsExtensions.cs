using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class WorkspaceSymbolsExtensions
    {
        public static Task<WorkspaceSymbolInformationContainer> WorkspaceSymbol(this ILanguageClientWorkspace router, WorkspaceSymbolParams @params)
        {
            return router.SendRequest<WorkspaceSymbolParams, WorkspaceSymbolInformationContainer>(WorkspaceNames.WorkspaceSymbol, @params);
        }
    }
}
