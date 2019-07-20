using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class JsonRpcServerExtensions
    {
        public static IDisposable OnRequest<T, TResponse>(
            this IJsonRpcHandlerRegistry registry,
            string method,
            Func<T, Task<TResponse>> handler)
        {
            return registry.AddHandler(method, _ => new DelegatingRequestHandler<T, TResponse>(_.GetRequiredService<ISerializer>(), (x, ct) => handler(x)));
        }

        public static IDisposable OnRequest<T, TResponse>(
            this IJsonRpcHandlerRegistry registry,
            string method,
            Func<T, CancellationToken, Task<TResponse>> handler)
        {
            return registry.AddHandler(method, _ => new DelegatingRequestHandler<T, TResponse>(_.GetRequiredService<ISerializer>(), handler));
        }

        public static IDisposable OnRequest<T>(
            this IJsonRpcHandlerRegistry registry,
            string method,
            Func<T, Task> handler)
        {
            return registry.AddHandler(method, _ => new DelegatingRequestHandler<T>(_.GetRequiredService<ISerializer>(), (x, ct) => handler(x)));
        }

        public static IDisposable OnRequest<T>(
            this IJsonRpcHandlerRegistry registry,
            string method,
            Func<T, CancellationToken, Task> handler)
        {
            return registry.AddHandler(method, _ => new DelegatingRequestHandler<T>(_.GetRequiredService<ISerializer>(), handler));
        }

        public static IDisposable OnNotification<T>(
            this IJsonRpcHandlerRegistry registry,
            string method,
            Action<T> handler)
        {
            return registry.AddHandler(method, _ => new DelegatingNotificationHandler<T>(_.GetRequiredService<ISerializer>(), handler));
        }

        public static IDisposable OnNotification(
            this IJsonRpcHandlerRegistry registry,
            string method,
            Action handler)
        {
            return registry.AddHandler(method, _ => new DelegatingNotificationHandler(_.GetRequiredService<ISerializer>(), handler));
        }
    }
}
