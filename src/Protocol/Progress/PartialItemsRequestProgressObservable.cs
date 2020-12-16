﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Progress
{
    internal class PartialItemsRequestProgressObservable<TItem, TResult> : IRequestProgressObservable<IEnumerable<TItem>, TResult>, IObserver<JToken>, IDisposable
        where TResult : IEnumerable<TItem>?
    {
        private readonly ISerializer _serializer;
        private readonly ReplaySubject<IEnumerable<TItem>> _dataSubject;
        private readonly CompositeDisposable _disposable;
        private readonly Task<TResult> _task;

        public PartialItemsRequestProgressObservable(
            ISerializer serializer,
            ProgressToken token,
            IObservable<TResult> requestResult,
            Func<IEnumerable<TItem>, TResult> factory,
            CancellationToken cancellationToken,
            Action disposal
        )
        {
            _serializer = serializer;
            _dataSubject = new ReplaySubject<IEnumerable<TItem>>(int.MaxValue);
            var request = requestResult
                         .Do(
                              // this should be fine as long as the other side is spec compliant (requests cannot return new results) so this should be null or empty
                              result => _dataSubject.OnNext(result ?? Enumerable.Empty<TItem>()),
                              OnError,
                              OnCompleted
                          )
                         .Replay(1);
            _disposable = new CompositeDisposable {
                request.Connect(),
                Disposable.Create(disposal)
            };

            _task = _dataSubject
                   .Scan(
                        new List<TItem>(),
                        (acc, data) => {
                            acc.AddRange(data);
                            return acc;
                        }
                    )
                   .StartWith(new List<TItem>())
                   .Select(factory)
                   .ForkJoin(request, (items, result) => items?.Count() > result?.Count() ? items : result)
                   .ToTask(cancellationToken);

#pragma warning disable VSTHRD105
#pragma warning disable VSTHRD110
            _task.ContinueWith(_ => Dispose());
#pragma warning restore VSTHRD110
#pragma warning restore VSTHRD105

            ProgressToken = token;
            if (_dataSubject is IDisposable disposable)
            {
                _disposable.Add(disposable);
            }
        }

        public ProgressToken ProgressToken { get; }
        public Type ParamsType { get; } = typeof(TItem);

        void IObserver<JToken>.OnCompleted() => OnCompleted();
        void IObserver<JToken>.OnError(Exception error) => OnError(error);

        private void OnCompleted()
        {
            if (_dataSubject.IsDisposed) return;
            _dataSubject.OnCompleted();
        }

        private void OnError(Exception error)
        {
            if (_dataSubject.IsDisposed) return;
            _dataSubject.OnError(error);
        }

        public void OnNext(JToken value)
        {
            if (_dataSubject.IsDisposed) return;
            _dataSubject.OnNext(value.ToObject<TItem[]>(_serializer.JsonSerializer));
        }

        public void Dispose()
        {
            if (_disposable.IsDisposed) return;
            _disposable.Dispose();
        }

        public IDisposable Subscribe(IObserver<IEnumerable<TItem>> observer) => _disposable.IsDisposed ? Disposable.Empty : _dataSubject.Subscribe(observer);

#pragma warning disable VSTHRD003
        public Task<TResult> AsTask() => _task;
#pragma warning restore VSTHRD003
        public TaskAwaiter<TResult> GetAwaiter() => _task.GetAwaiter();
    }

    internal class PartialItemsRequestProgressObservable<TItem> : PartialItemsRequestProgressObservable<TItem, Container<TItem>?>, IRequestProgressObservable<TItem>
    {
        public PartialItemsRequestProgressObservable(
            ISerializer serializer,
            ProgressToken token,
            IObservable<Container<TItem>?> requestResult,
            Func<IEnumerable<TItem>, Container<TItem>?> factory,
            CancellationToken cancellationToken,
            Action disposal
        ) : base(
            serializer,
            token,
            requestResult,
            factory,
            cancellationToken,
            disposal
        )
        {
        }
    }
}
