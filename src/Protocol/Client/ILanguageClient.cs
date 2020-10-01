using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public interface ILanguageClient : ILanguageClientFacade, IDisposable
    {
        IServiceProvider Services { get; }
        IClientWorkDoneManager WorkDoneManager { get; }
        IRegistrationManager RegistrationManager { get; }
        ILanguageClientWorkspaceFoldersManager WorkspaceFoldersManager { get; }
        Task Initialize(CancellationToken token);
        Task Shutdown();
    }
}
