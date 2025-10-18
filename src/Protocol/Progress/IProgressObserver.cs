using System.Reactive;
using System.Runtime.CompilerServices;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Progress
{
    public interface IProgressObserverWithInitialValue<TInitial, in TItem> : IProgressObserver, IObserver<TItem>
    {
        void OnNext(TInitial initial);
        TaskAwaiter<Unit> GetAwaiter();
    }
    public interface IProgressObserver<in TItem> : IProgressObserver, IObserver<TItem>
    {
        TaskAwaiter<Unit> GetAwaiter();
    }

    public interface IProgressObserver : IProgressContext, IDisposable
    {
        CancellationToken CancellationToken { get; }
        Type ParamsType { get; }
    }
}
