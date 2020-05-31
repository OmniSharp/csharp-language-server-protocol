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


        public T OnJsonRequest(string method, Func<JToken, CancellationToken, Task<JToken>> handler)
        {
            return AddHandler(method, _ => new DelegatingJsonRequestHandler(handler));
        }

        public T OnJsonRequest(string method, Func<JToken, Task<JToken>> handler)
        {
            return OnJsonRequest(method, (request, ct) => handler(request));
        }

        public T OnRequest<TParams, TResponse>(string method, Func<TParams, Task<TResponse>> handler)
        {
            return OnRequest<TParams, TResponse>(method, (value, cancellationToken) => handler(value));
        }

        public T OnRequest<TResponse>(string method, Func<Task<TResponse>> handler)
        {
            return OnRequest<Unit, TResponse>(method, (value, cancellationToken) => handler());
        }

        public T OnRequest<TParams, TResponse>(string method, Func<TParams, CancellationToken, Task<TResponse>> handler)
        {
            return AddHandler(method, _ => new DelegatingRequestHandler<TParams, TResponse>(_.GetRequiredService<ISerializer>(), handler));
        }

        public T OnRequest<TResponse>(
            string method,
            Func<CancellationToken, Task<TResponse>> handler)
        {
            return OnRequest<Unit, TResponse>(method, (value, cancellationToken) => handler(cancellationToken));
        }

        public T OnRequest<TParams>(string method, Func<TParams, Task> handler)
        {
            return OnRequest<TParams>(method, (value, cancellationToken) => handler(value));
        }

        public T OnRequest<TParams>(string method, Func<TParams, CancellationToken, Task> handler)
        {
            return AddHandler(method, _ => new DelegatingRequestHandler<TParams>(_.GetRequiredService<ISerializer>(), handler));
        }

        public T OnRequest<TParams>(string method, Func<CancellationToken, Task> handler)
        {
            return OnRequest<TParams>(method, (value, cancellationToken) => handler(cancellationToken));
        }

        public T OnJsonNotification(string method, Action<JToken, CancellationToken> handler)
        {
            return OnJsonNotification(method, (value, cancellationToken) => {
                handler(value, cancellationToken);
                return Task.CompletedTask;
            });
        }

        public T OnJsonNotification(string method, Action<JToken> handler)
        {
            return OnJsonNotification(method, (value, cancellationToken) => {
                handler(value);
                return Task.CompletedTask;
            });
        }

        public T OnJsonNotification(string method, Func<JToken, CancellationToken, Task> handler)
        {
            return AddHandler(method, _ => new DelegatingJsonNotificationHandler(handler));
        }

        public T OnJsonNotification(string method, Func<JToken, Task> handler)
        {
            return OnJsonNotification(method, (value, cancellationToken) => handler(value));
        }

        public T OnNotification<TParams>(string method, Action<TParams, CancellationToken> handler)
        {
            return OnNotification<TParams>(method, (value, cancellationToken) => {
                handler(value, cancellationToken);
                return Task.CompletedTask;
            });
        }

        public T OnNotification<TParams>(string method, Action<TParams> handler)
        {
            return OnNotification<TParams>(method, (value, cancellationToken) => {
                handler(value);
                return Task.CompletedTask;
            });
        }

        public T OnNotification<TParams>(string method, Func<TParams, CancellationToken, Task> handler)
        {
            return AddHandler(method, _ => new DelegatingNotificationHandler<TParams>(_.GetRequiredService<ISerializer>(), handler));
        }

        public T OnNotification<TParams>(string method, Func<TParams, Task> handler)
        {
            return OnNotification<TParams>(method, (value, cancellationToken) => handler(value));
        }

        public T OnNotification(string method, Action handler)
        {
            return OnNotification<Unit>(method, (value, cancellationToken) => {
                handler();
                return Task.CompletedTask;
            });
        }

        public T OnNotification(string method, Func<CancellationToken, Task> handler)
        {
            return AddHandler(method, _ => new DelegatingNotificationHandler<Unit>(_.GetRequiredService<ISerializer>(), (unit, token) => handler(token)));
        }

        public T OnNotification(string method, Func<Task> handler)
        {
            return OnNotification(method, (cancellationToken) => handler());
        }

        #endregion

        #region AddHandler

        public abstract T AddHandler(string method, IJsonRpcHandler handler);
        public abstract T AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc);
        public abstract T AddHandlers(params IJsonRpcHandler[] handlers);
        public abstract T AddHandler<THandler>(Func<IServiceProvider, THandler> handlerFunc) where THandler : IJsonRpcHandler;
        public abstract T AddHandler<THandler>(THandler handler) where THandler : IJsonRpcHandler;
        public abstract T AddHandler<THandler>() where THandler : IJsonRpcHandler;
        public abstract T AddHandler<THandler>(string method) where THandler : IJsonRpcHandler;
        public abstract T AddHandler(Type type);
        public abstract T AddHandler(string method, Type type);

        #endregion
    }
}
