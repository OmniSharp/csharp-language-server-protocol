using OmniSharp.Extensions.LanguageServer.Protocol.Client.WorkDone;

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
