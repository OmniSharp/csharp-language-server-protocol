using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server.Handlers;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public interface ILanguageServer : OmniSharp.Extensions.LanguageServer.Protocol.Server.ILanguageServer, IDisposable
    {
        IDisposable AddHandlers(params IJsonRpcHandler[] handlers);
        IDisposable AddHandlers(params Type[] handlerTypes);
        IDisposable AddHandler<T>() where T : IJsonRpcHandler;
        IDisposable AddHandler(string method, IJsonRpcHandler handler);
        IDisposable AddHandler(string method, Type handlerType);

        InitializeParams ClientSettings { get; }
        InitializeResult ServerSettings { get; }
        IServiceProvider Services { get; }

        IObservable<bool> Shutdown { get; }
        IObservable<int> Exit { get; }
        Task WasShutDown { get; }
        Task WaitForExit { get; }
    }
}
