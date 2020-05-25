using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace OmniSharp.Extensions.JsonRpc
{

    public interface IJsonRpcHandlerRegistry {}
    public interface IJsonRpcHandlerRegistry<out T> : IJsonRpcHandlerRegistry where T : IJsonRpcHandlerRegistry<T>
    {
        T AddHandler(string method, IJsonRpcHandler handler);
        T AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc);
        T AddHandlers(params IJsonRpcHandler[] handlers);
        T AddHandler<THandler>(Func<IServiceProvider, THandler> handlerFunc) where THandler : IJsonRpcHandler;
        T AddHandler<THandler>(THandler handler) where THandler : IJsonRpcHandler;
        T AddHandler<TTHandler>() where TTHandler : IJsonRpcHandler;
        T AddHandler<TTHandler>(string method) where TTHandler : IJsonRpcHandler;
        T AddHandler(Type type);
        T AddHandler(string method, Type type);

        T OnRequest<TParams, TResponse>(string method, Func<TParams, Task<TResponse>> handler);
        T OnRequest<TResponse>(string method, Func<Task<TResponse>> handler);
        T OnRequest<TParams, TResponse>(string method, Func<TParams, CancellationToken, Task<TResponse>> handler);
        T OnRequest<TResponse>(string method, Func<CancellationToken, Task<TResponse>> handler);
        T OnRequest<TParams>(string method, Func<TParams, Task> handler);
        T OnRequest<TParams>(string method, Func<TParams, CancellationToken, Task> handler);
        T OnRequest<TParams>(string method, Func<CancellationToken, Task> handler);
        T OnNotification<TParams>(string method, Action<TParams, CancellationToken> handler);
        T OnNotification(string method, Action handler);
        T OnNotification<TParams>(string method, Action<TParams> handler);
        T OnNotification<TParams>(string method, Func<TParams, CancellationToken, Task> handler);
        T OnNotification<TParams>(string method, Func<TParams, Task> handler);
        T OnNotification(string method, Func<CancellationToken, Task> handler);
        T OnNotification(string method, Func<Task> handler);
    }
}
