using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public interface ILanguageClient : IClientProxy, ILanguageClientRegistry, IDisposable
    {
        ITextDocumentLanguageClient TextDocument { get; }
        IClientLanguageClient Client { get; }
        IGeneralLanguageClient General { get; }
        IWindowLanguageClient Window { get; }
        IWorkspaceLanguageClient Workspace { get; }
        IServiceProvider Services { get; }
        Task Initialize(CancellationToken token);
        Task Shutdown();
    }
}
