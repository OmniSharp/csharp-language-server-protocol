using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Progress
{
    public interface IProgressObservable<out TItem> : IProgressObservable, IDisposable, IObservable<TItem>
    {
    }

    public interface IProgressObservable : IProgressContext
    {
        Type ParamsType { get; }
    }
}
