using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static WorkspaceNames;
    public static partial class WorkspaceNames
    {
        public const string Symbol = "workspace/symbol";
    }

    [Parallel, Method(Symbol)]
    public interface IWorkspaceSymbolsHandler : IRequestHandler<WorkspaceSymbolParams, SymbolInformationContainer>, ICapability<WorkspaceSymbolCapability> { }
}
