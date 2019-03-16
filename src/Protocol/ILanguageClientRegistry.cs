using System;
using OmniSharp.Extensions.JsonRpc;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public interface ILanguageClientRegistry
    {
        IDisposable AddHandler(string method, IJsonRpcHandler handler);
        IDisposable AddHandlers(params IJsonRpcHandler[] handlers);
        IDisposable AddHandler<T>() where T : IJsonRpcHandler;
    }
}
