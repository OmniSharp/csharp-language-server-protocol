using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
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
            IPartialItemRequest<TResult, TItem> request,
            Func<TItem, TResult> factory,
            Func<TResult, TItem> reverseFactory,
            CancellationToken cancellationToken
        );

        IRequestProgressObservable<TItem, TResult> MonitorUntil<TItem, TResult>(
            IPartialItemRequest<TResult, TItem> request,
            Func<TResult, TItem, TResult> factory,
            Func<TResult, TItem> reverseFactory,
            CancellationToken cancellationToken
        );

        IRequestProgressObservable<TItem, TResult> MonitorUntil<TItem, TResult>(
            IPartialItemWithInitialValueRequest<TResult, TItem> request,
            Func<TResult, TItem, TResult> factory,
            CancellationToken cancellationToken
        ) where TResult : TItem;

        IRequestProgressObservable<IEnumerable<TItem>, TResponse> MonitorUntil<TResponse, TItem>(
            IPartialItemsRequest<TResponse, TItem> request,
            Func<IEnumerable<TItem>, TResponse> factory,
            CancellationToken cancellationToken
        ) where TResponse : IEnumerable<TItem>?;

        IRequestProgressObservable<TItem> MonitorUntil<TItem>(
            IPartialItemsRequest<Container<TItem>, TItem> request,
            CancellationToken cancellationToken
        );
        
        IRequestProgressObservable<IEnumerable<TItem>, TResponse> MonitorUntil<TResponse, TItem>(
            IPartialItemsWithInitialValueRequest<TResponse, TItem> request,
            Func<TResponse, IEnumerable<TItem>, TResponse> factory,
            CancellationToken cancellationToken
        ) where TResponse : IEnumerable<TItem>?;

        IProgressObserver<T> For<T>(ProgressToken token, CancellationToken cancellationToken);
        IProgressObserver<TItem> For<TResponse, TItem>(IPartialItemRequest<TResponse, TItem> request, CancellationToken cancellationToken);
        IProgressObserverWithInitialValue<TInitial, TItem> For<TInitial, TItem>(IPartialItemWithInitialValueRequest<TInitial, TItem> request, CancellationToken cancellationToken);

        IProgressObserver<IEnumerable<TItem>> For<TResponse, TItem>(IPartialItemsRequest<TResponse, TItem> request, CancellationToken cancellationToken)
            where TResponse : IEnumerable<TItem>?;
        IProgressObserverWithInitialValue<TInitial, IEnumerable<TItem>> For<TInitial, TItem>(IPartialItemsWithInitialValueRequest<TInitial, TItem> request, CancellationToken cancellationToken)
            where TInitial : IEnumerable<TItem>?;
        IScheduler Scheduler { get; }
    }
}
