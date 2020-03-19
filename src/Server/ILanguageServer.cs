using System;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public interface ILanguageServer : OmniSharp.Extensions.LanguageServer.Protocol.Server.ILanguageServer, OmniSharp.Extensions.LanguageServer.Protocol.Server.ILanguageServerRegistry, IDisposable
    {
        InitializeParams ClientSettings { get; }
        InitializeResult ServerSettings { get; }
        IServiceProvider Services { get; }

        IObservable<InitializeResult> Start { get; }
        IObservable<bool> Shutdown { get; }
        IObservable<int> Exit { get; }
        Task<InitializeResult> WasStarted { get; }
        Task WasShutDown { get; }
        Task WaitForExit { get; }
    }
}
