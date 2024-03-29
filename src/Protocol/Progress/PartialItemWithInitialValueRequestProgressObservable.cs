﻿using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Progress;

internal class PartialItemWithInitialValueRequestProgressObservable<TItem, TResult> : IRequestProgressObservable<TItem, TResult>, IObserver<JToken>
    where TResult : TItem
{
    private readonly ISerializer _serializer;
    private readonly ReplaySubject<TItem> _dataSubject;
    private readonly CompositeDisposable _disposable;
    private readonly Task<TResult> _task;
    private bool _receivedInitialValue;
    private bool _receivedPartialData;

    public PartialItemWithInitialValueRequestProgressObservable(
        ISerializer serializer,
        ProgressToken token,
        IObservable<TResult> requestResult,
        Func<TResult, TItem, TResult> factory,
        CancellationToken cancellationToken,
        Action onCompleteAction
    )
    {
        _serializer = serializer;
        _dataSubject = new ReplaySubject<TItem>(1, Scheduler.Immediate);
        _disposable = new CompositeDisposable { _dataSubject };
        _task = Observable.Create<TResult>(
                               observer => new CompositeDisposable
                               {
                                   requestResult
                                      .Do(
                                           result =>
                                           {
                                               if (_receivedPartialData) return;
                                               _dataSubject.OnNext(result);
                                           },
                                           _dataSubject.OnError,
                                           _dataSubject.OnCompleted
                                       )
                                      .ForkJoin(_dataSubject, factory)
                                      .Subscribe(observer),
                                   Disposable.Create(onCompleteAction)
                               }
                           )
                          .ToTask(cancellationToken);

        ProgressToken = token;
    }

    public ProgressToken ProgressToken { get; }
    public Type ParamsType { get; } = typeof(TItem);

    void IObserver<JToken>.OnCompleted()
    {
        OnCompleted();
    }

    void IObserver<JToken>.OnError(Exception error)
    {
        OnError(error);
    }

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
        _receivedPartialData = true;
        if (!_receivedInitialValue)
        {
            _receivedInitialValue = true;
            _dataSubject.OnNext(value.ToObject<TResult>(_serializer.JsonSerializer)!);
        }
        else
        {
            _dataSubject.OnNext(value.ToObject<TItem>(_serializer.JsonSerializer)!);
        }
    }

    public void Dispose()
    {
        if (_disposable.IsDisposed) return;
        _disposable.Dispose();
    }

    public IDisposable Subscribe(IObserver<TItem> observer)
    {
        return _dataSubject.Subscribe(observer);
    }

#pragma warning disable VSTHRD003
    public Task<TResult> AsTask()
    {
        return _task;
    }
#pragma warning restore VSTHRD003
    public TaskAwaiter<TResult> GetAwaiter()
    {
        return _task.GetAwaiter();
    }
}
