using System;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.Threading;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Progress
{
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
