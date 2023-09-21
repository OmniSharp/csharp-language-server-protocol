using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public interface ILanguageServerFacade : ILanguageServerProxy, IJsonRpcHandlerInstance<ILanguageServerRegistry>
    {
        ITextDocumentLanguageServer TextDocument { get; }
        INotebookDocumentLanguageServer NotebookDocument { get; }
        IClientLanguageServer Client { get; }
        IGeneralLanguageServer General { get; }
        IWindowLanguageServer Window { get; }
        IWorkspaceLanguageServer Workspace { get; }
    }
}
