using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class WorkspaceSymbolsExtensions
    {
        public static Task<Container<SymbolInformation>> WorkspaceSymbol(this ILanguageClientWorkspace router, WorkspaceSymbolParams @params, CancellationToken cancellationToken = default)
        {
            return router.SendRequest<WorkspaceSymbolParams, Container<SymbolInformation>>(WorkspaceNames.WorkspaceSymbol, @params, cancellationToken);
        }
    }
}
