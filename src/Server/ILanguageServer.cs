using System;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public interface ILanguageServer : OmniSharp.Extensions.LanguageServer.Protocol.Server.ILanguageServer
    {
        InitializeParams ClientSettings { get; }
        InitializeResult ServerSettings { get; }
        IServiceProvider Services { get; }
        ILanguageServerConfiguration Configuration { get; }

        IObservable<InitializeResult> Start { get; }
        IObservable<bool> Shutdown { get; }
        IObservable<int> Exit { get; }
        Task<InitializeResult> WasStarted { get; }
        Task WasShutDown { get; }
        Task WaitForExit { get; }
    }
}
