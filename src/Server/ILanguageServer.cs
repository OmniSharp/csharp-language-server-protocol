using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server.Handlers;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public interface ILanguageServer : IResponseRouter, IDisposable
    {
        IDisposable AddHandler(string method, IJsonRpcHandler handler);
        IDisposable AddHandler(IJsonRpcHandler handler);
        IDisposable AddHandlers(IEnumerable<IJsonRpcHandler> handlers);
        IDisposable AddHandlers(params IJsonRpcHandler[] handlers);

        InitializeParams Client { get; }
        InitializeResult Server { get; }

        Task Initialize();

        event ShutdownEventHandler Shutdown;
        event ExitEventHandler Exit;
        Task WasShutDown { get; }
        Task WaitForExit { get; }
    }
}
