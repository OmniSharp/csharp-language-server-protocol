using System;
using System.Threading;
using System.Threading.Tasks;

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
