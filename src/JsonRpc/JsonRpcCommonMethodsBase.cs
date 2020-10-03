using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc
{
    public abstract class JsonRpcCommonMethodsBase<T> : IJsonRpcHandlerRegistry<T> where T : IJsonRpcHandlerRegistry<T>
    {
        #region OnRequest / OnNotification

        public T OnJsonRequest(string method, Func<JToken, CancellationToken, Task<JToken>> handler, JsonRpcHandlerOptions? options = null) =>
            AddHandler(method, _ => new DelegatingJsonRequestHandler(handler), options);

        public T OnJsonRequest(string method, Func<JToken, Task<JToken>> handler, JsonRpcHandlerOptions? options = null) =>
            OnJsonRequest(method, (request, ct) => handler(request), options);

        public T OnRequest<TParams, TResponse>(string method, Func<TParams, Task<TResponse>> handler, JsonRpcHandlerOptions? options = null) =>
            OnRequest<TParams, TResponse>(method, (value, cancellationToken) => handler(value), options);

        public T OnRequest<TResponse>(string method, Func<Task<TResponse>> handler, JsonRpcHandlerOptions? options = null) =>
            OnRequest<Unit, TResponse>(method, (value, cancellationToken) => handler(), options);

        public T OnRequest<TParams, TResponse>(string method, Func<TParams, CancellationToken, Task<TResponse>> handler, JsonRpcHandlerOptions? options = null) => AddHandler(
            method, _ => new DelegatingRequestHandler<TParams, TResponse>(_.GetRequiredService<ISerializer>(), handler), options
        );

        public T OnRequest<TResponse>(string method, Func<CancellationToken, Task<TResponse>> handler, JsonRpcHandlerOptions? options = null) =>
            OnRequest<Unit, TResponse>(method, (value, cancellationToken) => handler(cancellationToken), options);

        public T OnRequest<TParams>(string method, Func<TParams, Task> handler, JsonRpcHandlerOptions? options = null) =>
            OnRequest<TParams>(method, (value, cancellationToken) => handler(value), options);

        public T OnRequest<TParams>(string method, Func<TParams, CancellationToken, Task> handler, JsonRpcHandlerOptions? options = null) => AddHandler(
            method, _ => new DelegatingRequestHandler<TParams>(_.GetRequiredService<ISerializer>(), handler), options
        );

        public T OnRequest<TParams>(string method, Func<CancellationToken, Task> handler, JsonRpcHandlerOptions? options = null) =>
            OnRequest<TParams>(method, (value, cancellationToken) => handler(cancellationToken), options);

        public T OnJsonNotification(string method, Action<JToken, CancellationToken> handler, JsonRpcHandlerOptions? options = null) =>
            OnJsonNotification(
                method, (value, cancellationToken) => {
                    handler(value, cancellationToken);
                    return Task.CompletedTask;
                }, options
            );

        public T OnJsonNotification(string method, Action<JToken> handler, JsonRpcHandlerOptions? options = null) =>
            OnJsonNotification(
                method, (value, cancellationToken) => {
                    handler(value);
                    return Task.CompletedTask;
                }, options
            );

        public T OnJsonNotification(string method, Func<JToken, CancellationToken, Task> handler, JsonRpcHandlerOptions? options = null) =>
            AddHandler(method, _ => new DelegatingJsonNotificationHandler(handler), options);

        public T OnJsonNotification(string method, Func<JToken, Task> handler, JsonRpcHandlerOptions? options = null) =>
            OnJsonNotification(method, (value, cancellationToken) => handler(value), options);

        public T OnNotification<TParams>(string method, Action<TParams, CancellationToken> handler, JsonRpcHandlerOptions? options = null) =>
            OnNotification<TParams>(
                method, (value, cancellationToken) => {
                    handler(value, cancellationToken);
                    return Task.CompletedTask;
                }, options
            );

        public T OnNotification<TParams>(string method, Action<TParams> handler, JsonRpcHandlerOptions? options = null) =>
            OnNotification<TParams>(
                method, (value, cancellationToken) => {
                    handler(value);
                    return Task.CompletedTask;
                }, options
            );

        public T OnNotification<TParams>(string method, Func<TParams, CancellationToken, Task> handler, JsonRpcHandlerOptions? options = null) => AddHandler(
            method, _ => new DelegatingNotificationHandler<TParams>(_.GetRequiredService<ISerializer>(), handler), options
        );

        public T OnNotification<TParams>(string method, Func<TParams, Task> handler, JsonRpcHandlerOptions? options = null) =>
            OnNotification<TParams>(method, (value, cancellationToken) => handler(value), options);

        public T OnNotification(string method, Action handler, JsonRpcHandlerOptions? options = null) =>
            OnNotification<Unit>(
                method, (value, cancellationToken) => {
                    handler();
                    return Task.CompletedTask;
                }, options
            );

        public T OnNotification(string method, Func<CancellationToken, Task> handler, JsonRpcHandlerOptions? options = null) => AddHandler(
            method, _ => new DelegatingNotificationHandler<Unit>(_.GetRequiredService<ISerializer>(), (unit, token) => handler(token)), options
        );

        public T OnNotification(string method, Func<Task> handler, JsonRpcHandlerOptions? options = null) => OnNotification(method, cancellationToken => handler(), options);

        #endregion

        #region AddHandler

        public abstract T AddHandler(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions? options = null);
        public abstract T AddHandler(string method, JsonRpcHandlerFactory handlerFunc, JsonRpcHandlerOptions? options = null);
        public abstract T AddHandler(IJsonRpcHandler handler, JsonRpcHandlerOptions? options = null);
        public abstract T AddHandler(JsonRpcHandlerFactory handlerFunc, JsonRpcHandlerOptions? options = null);
        public abstract T AddHandlers(params IJsonRpcHandler[] handlers);
        public abstract T AddHandler<THandler>(JsonRpcHandlerOptions? options = null) where THandler : IJsonRpcHandler;
        public abstract T AddHandler<THandler>(string method, JsonRpcHandlerOptions? options = null) where THandler : IJsonRpcHandler;
        public abstract T AddHandler(Type type, JsonRpcHandlerOptions? options = null);
        public abstract T AddHandler(string method, Type type, JsonRpcHandlerOptions? options = null);
        public abstract T AddHandlerLink(string fromMethod, string toMethod);

        #endregion
    }
}
