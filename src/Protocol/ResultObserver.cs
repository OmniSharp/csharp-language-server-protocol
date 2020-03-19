using System;
using System.Reactive;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public class ResultObserver<T> : IDisposable, IObserver<T>
    {
        public static ResultObserver<T> Noop { get; } = new ResultObserver<T>(Observer.Create<T>(x => { }));
        public ResultObserver(IObserver<T> observer)
        {
            Result = observer;
        }

        public IObserver<T> Result { get; }

        public void Dispose()
        {
            Result.OnCompleted();
        }

        public void OnCompleted()
        {
            Result.OnCompleted();
        }

        public void OnError(Exception error)
        {
            Result.OnError(error);
        }

        public void OnNext(T value)
        {
            Result.OnNext(value);
        }
    }
}
