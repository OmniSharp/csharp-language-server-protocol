using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public interface ILanguageClient : ILanguageClientProxy, IJsonRpcHandlerInstance<ILanguageClientRegistry>, IDisposable
    {
        ITextDocumentLanguageClient TextDocument { get; }
        IClientLanguageClient Client { get; }
        IGeneralLanguageClient General { get; }
        IWindowLanguageClient Window { get; }
        IWorkspaceLanguageClient Workspace { get; }
        IServiceProvider Services { get; }
        IClientWorkDoneManager WorkDoneManager { get; }
        IRegistrationManager RegistrationManager { get; }
        ILanguageClientWorkspaceFoldersManager WorkspaceFoldersManager { get; }
        InitializeParams ClientSettings { get; }
        InitializeResult ServerSettings { get; }
        Task Initialize(CancellationToken token);
        Task Shutdown();
    }
}
