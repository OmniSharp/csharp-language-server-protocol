using JsonRpc;
using Lsp.Capabilities.Client;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("workspace/symbol")]
    public interface IWorkspaceSymbolsHandler : IRequestHandler<WorkspaceSymbolParams, SymbolInformationContainer>, ICapability<WorkspaceSymbolCapability> { }
}