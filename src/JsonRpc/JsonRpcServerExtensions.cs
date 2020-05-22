using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
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
            return registry.OnRequest<T, TResponse>(method, (value, cancellationToken) => handler(value));
        }

        public static IDisposable OnRequest<TResponse>(
            this IJsonRpcHandlerRegistry registry,
            string method,
            Func<Task<TResponse>> handler)
        {
            return registry.OnRequest<Unit, TResponse>(method, (value, cancellationToken) => handler());
        }

        public static IDisposable OnRequest<T, TResponse>(
            this IJsonRpcHandlerRegistry registry,
            string method,
            Func<T, CancellationToken, Task<TResponse>> handler)
        {
            return registry.AddHandler(method, _ => new DelegatingRequestHandler<T, TResponse>(_.GetRequiredService<ISerializer>(), handler));
        }

        public static IDisposable OnRequest<TResponse>(
            this IJsonRpcHandlerRegistry registry,
            string method,
            Func<CancellationToken, Task<TResponse>> handler)
        {
            return registry.OnRequest<Unit, TResponse>(method, (value, cancellationToken) => handler(cancellationToken));
        }

        public static IDisposable OnRequest<T>(
            this IJsonRpcHandlerRegistry registry,
            string method,
            Func<T, Task> handler)
        {
            return registry.OnRequest<T>(method, (value, cancellationToken) => handler(value));
        }

        public static IDisposable OnRequest<T>(
            this IJsonRpcHandlerRegistry registry,
            string method,
            Func<T, CancellationToken, Task> handler)
        {
            return registry.AddHandler(method, _ => new DelegatingRequestHandler<T>(_.GetRequiredService<ISerializer>(), handler));
        }

        public static IDisposable OnRequest<T>(
            this IJsonRpcHandlerRegistry registry,
            string method,
            Func<CancellationToken, Task> handler)
        {
            return registry.OnRequest<T>(method, (value, cancellationToken) => handler(cancellationToken));
        }

        public static IDisposable OnNotification<T>(
            this IJsonRpcHandlerRegistry registry,
            string method,
            Action<T, CancellationToken> handler)
        {
            return registry.OnNotification<T>(method, (value, cancellationToken) => {
                handler(value, cancellationToken);
                return Task.CompletedTask;
            });
        }

        public static IDisposable OnNotification(
            this IJsonRpcHandlerRegistry registry,
            string method,
            Action handler)
        {
            return registry.OnNotification<Unit>(method, (value, cancellationToken) => {
                handler();
                return Task.CompletedTask;
            });
        }

        public static IDisposable OnNotification<T>(
            this IJsonRpcHandlerRegistry registry,
            string method,
            Action<T> handler)
        {
            return registry.OnNotification<T>(method, (value, cancellationToken) => {
                handler(value);
                return Task.CompletedTask;
            });
        }

        public static IDisposable OnNotification<T>(
            this IJsonRpcHandlerRegistry registry,
            string method,
            Func<T, CancellationToken, Task> handler)
        {
            return registry.AddHandler(method, _ => new DelegatingNotificationHandler<T>(_.GetRequiredService<ISerializer>(), handler));
        }

        public static IDisposable OnNotification<T>(
            this IJsonRpcHandlerRegistry registry,
            string method,
            Func<T, Task> handler)
        {
            return registry.OnNotification<T>(method, (value, cancellationToken) => handler(value));
        }

        public static IDisposable OnNotification(
            this IJsonRpcHandlerRegistry registry,
            string method,
            Func<CancellationToken, Task> handler)
        {
            return registry.AddHandler(method, _ => new DelegatingNotificationHandler<Unit>(_.GetRequiredService<ISerializer>(), (unit, token) => handler(token)));
        }

        public static IDisposable OnNotification(
            this IJsonRpcHandlerRegistry registry,
            string method,
            Func<Task> handler)
        {
            return registry.OnNotification(method, (cancellationToken) => handler());
        }

        /// <summary>
        /// Set maximum number of allowed parallel actions
        /// </summary>
        /// <param name="options"></param>
        /// <param name="concurrency"></param>
        /// <returns></returns>
        public static JsonRpcServerOptions WithConcurrency(this JsonRpcServerOptions options, int? concurrency)
        {
            options.Concurrency = concurrency;
            return options;
        }
    }
}
