using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Method("workspace/symbol")]
    public interface IWorkspaceSymbolsHandler : IRequestHandler<WorkspaceSymbolParams, SymbolInformationContainer> { }
}