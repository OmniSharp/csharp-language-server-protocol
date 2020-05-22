using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [Parallel, Method(GeneralNames.Progress, Direction.Bidirectional)]
    public interface IProgressHandler : IJsonRpcNotificationHandler<ProgressParams>
    {
    }

    public abstract class ProgressHandler : IProgressHandler
    {
        public abstract Task<Unit> Handle(ProgressParams request, CancellationToken cancellationToken);
    }

    public static class ProgressExtensions
    {
        public static IDisposable OnProgress(
            this ILanguageServerRegistry registry,
            Action<ProgressParams, CancellationToken> handler)
        {
            return registry.AddHandler(GeneralNames.Progress, NotificationHandler.For(handler));
        }

        public static IDisposable OnProgress(
            this ILanguageServerRegistry registry,
            Func<ProgressParams, CancellationToken, Task> handler)
        {
            return registry.AddHandler(GeneralNames.Progress, NotificationHandler.For(handler));
        }

        public static IDisposable OnProgress(
            this ILanguageServerRegistry registry,
            Action<ProgressParams> handler)
        {
            return registry.AddHandler(GeneralNames.Progress, NotificationHandler.For(handler));
        }

        public static IDisposable OnProgress(
            this ILanguageServerRegistry registry,
            Func<ProgressParams, Task> handler)
        {
            return registry.AddHandler(GeneralNames.Progress, NotificationHandler.For(handler));
        }

        public static IDisposable OnProgress(
            this ILanguageClientRegistry registry,
            Action<ProgressParams, CancellationToken> handler)
        {
            return registry.AddHandler(GeneralNames.Progress, NotificationHandler.For(handler));
        }

        public static IDisposable OnProgress(
            this ILanguageClientRegistry registry,
            Func<ProgressParams, CancellationToken, Task> handler)
        {
            return registry.AddHandler(GeneralNames.Progress, NotificationHandler.For(handler));
        }

        public static IDisposable OnProgress(
            this ILanguageClientRegistry registry,
            Action<ProgressParams> handler)
        {
            return registry.AddHandler(GeneralNames.Progress, NotificationHandler.For(handler));
        }

        public static IDisposable OnProgress(
            this ILanguageClientRegistry registry,
            Func<ProgressParams, Task> handler)
        {
            return registry.AddHandler(GeneralNames.Progress, NotificationHandler.For(handler));
        }

        public static void SendProgress(this IGeneralLanguageClient registry, ProgressParams @params)
        {
            registry.SendNotification(@params);
        }

        public static void SendProgress(this IGeneralLanguageServer registry, ProgressParams @params)
        {
            registry.SendNotification(@params);
        }

        public static IRequestProgressObservable<TItem, TResponse> RequestProgress<TResponse, TItem>(this IClientProxy requestRouter, IPartialItemRequest<TResponse, TItem> @params, Func<TItem, TResponse> factory, CancellationToken cancellationToken = default)
        {
            var resultToken = new ProgressToken(Guid.NewGuid().ToString());
            @params.PartialResultToken = resultToken;

            return requestRouter.ProgressManager.MonitorUntil(@params, factory, cancellationToken);
        }

        public static IRequestProgressObservable<IEnumerable<TItem>, TResponse> RequestProgress<TResponse, TItem>(this IClientProxy requestRouter, IPartialItemsRequest<TResponse, TItem> @params, Func<IEnumerable<TItem>, TResponse> factory, CancellationToken cancellationToken = default)
            where TResponse : IEnumerable<TItem>
        {
            var resultToken = new ProgressToken(Guid.NewGuid().ToString());
            @params.PartialResultToken = resultToken;

            return requestRouter.ProgressManager.MonitorUntil(@params, factory, cancellationToken);
        }

        public static IRequestProgressObservable<TItem, TResponse> RequestProgress<TResponse, TItem>(this IServerProxy requestRouter, IPartialItemRequest<TResponse, TItem> @params, Func<TItem, TResponse> factory, CancellationToken cancellationToken = default)
        {
            var resultToken = new ProgressToken(Guid.NewGuid().ToString());
            @params.PartialResultToken = resultToken;

            return requestRouter.ProgressManager.MonitorUntil(@params, factory, cancellationToken);
        }

        public static IRequestProgressObservable<IEnumerable<TItem>, TResponse> RequestProgress<TResponse, TItem>(this IServerProxy requestRouter, IPartialItemsRequest<TResponse, TItem> @params, Func<IEnumerable<TItem>, TResponse> factory, CancellationToken cancellationToken = default)
            where TResponse : IEnumerable<TItem>
        {
            var resultToken = new ProgressToken(Guid.NewGuid().ToString());
            @params.PartialResultToken = resultToken;

            return requestRouter.ProgressManager.MonitorUntil(@params, factory, cancellationToken);
        }
    }
}
