using System.Reactive;

namespace OmniSharp.Extensions.JsonRpc.Testing
{
    public interface ISettler
    {
        Task SettleNext();
        IObservable<Unit> Settle();
    }
}
