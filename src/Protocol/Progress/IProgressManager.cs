using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Progress
{
    public interface IProgressManager : IProgressHandler
    {
        IProgressObservable<T> Monitor<T>(ProgressToken token);
        IProgressObservable<T> Monitor<T>(ProgressToken token, Func<JToken, T> factory);

        IRequestProgressObservable<TItem, TResult> MonitorUntil<TItem, TResult>(
            IPartialItemRequest<TResult, TItem> request, Func<TItem, TResult> factory, CancellationToken cancellationToken
        );

        IRequestProgressObservable<TItem, TResult> MonitorUntil<TItem, TResult>(
            IPartialItemRequest<TResult, TItem> request, Func<TItem, TResult, TResult> factory, CancellationToken cancellationToken
        );

        IRequestProgressObservable<IEnumerable<TItem>, TResponse> MonitorUntil<TResponse, TItem>(
            IPartialItemsRequest<TResponse, TItem> request, Func<IEnumerable<TItem>, TResponse> factory, CancellationToken cancellationToken
        )
            where TResponse : IEnumerable<TItem>;

        IRequestProgressObservable<TItem> MonitorUntil<TItem>(IPartialItemsRequest<Container<TItem>, TItem> request, CancellationToken cancellationToken);

        IProgressObserver<T> For<T>(ProgressToken token, CancellationToken cancellationToken);
        IProgressObserver<TItem> For<TResponse, TItem>(IPartialItemRequest<TResponse, TItem> request, CancellationToken cancellationToken);

        IProgressObserver<IEnumerable<TItem>?> For<TResponse, TItem>(IPartialItemsRequest<TResponse, TItem> request, CancellationToken cancellationToken)
            where TResponse : IEnumerable<TItem>?;
    }
}
