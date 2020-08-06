using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public interface ILanguageServer : ILanguageServerProxy, IJsonRpcHandlerInstance<ILanguageServerRegistry>, IDisposable
    {
        ITextDocumentLanguageServer TextDocument { get; }
        IClientLanguageServer Client { get; }
        IGeneralLanguageServer General { get; }
        IWindowLanguageServer Window { get; }
        IWorkspaceLanguageServer Workspace { get; }
        IServiceProvider Services { get; }
        IServerWorkDoneManager WorkDoneManager { get; }
        ILanguageServerConfiguration Configuration { get; }

        IObservable<InitializeResult> Start { get; }
        IObservable<bool> Shutdown { get; }
        IObservable<int> Exit { get; }
        Task<InitializeResult> WasStarted { get; }
        Task WasShutDown { get; }
        Task WaitForExit { get; }
        Task Initialize(CancellationToken token);
    }
}
