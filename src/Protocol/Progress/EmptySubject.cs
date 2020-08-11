using System;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Progress
{
    internal class EmptySubject<T> : SubjectBase<T>
    {
        public override void Dispose()
        {
        }

        public override void OnCompleted()
        {
        }

        public override void OnError(Exception error)
        {
        }

        public override void OnNext(T value)
        {
        }

        public override IDisposable Subscribe(IObserver<T> observer)
        {
            observer.OnCompleted();
            return Disposable.Empty;
            ;
        }

        public override bool HasObservers => false;
        public override bool IsDisposed => false;
    }
}
