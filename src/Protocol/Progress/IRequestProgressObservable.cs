using System.Runtime.CompilerServices;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Progress
{
    public interface IRequestProgressObservable<out TItem, TResult> : IProgressObservable, IObservable<TItem>
    {
        Task<TResult> AsTask();
        TaskAwaiter<TResult> GetAwaiter();
    }

    public interface IRequestProgressObservable<TItem> : IRequestProgressObservable<IEnumerable<TItem>?, Container<TItem>?>
    {
    }
}
