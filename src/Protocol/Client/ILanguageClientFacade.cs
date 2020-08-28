using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public interface ILanguageClientFacade : ILanguageClientProxy, IJsonRpcHandlerInstance<ILanguageClientRegistry>
    {
        ITextDocumentLanguageClient TextDocument { get; }
        IClientLanguageClient Client { get; }
        IGeneralLanguageClient General { get; }
        IWindowLanguageClient Window { get; }
        IWorkspaceLanguageClient Workspace { get; }
    }
}
