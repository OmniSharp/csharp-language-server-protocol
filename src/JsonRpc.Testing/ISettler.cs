using System;
using System.Threading.Tasks;
using ReactiveUnit = System.Reactive.Unit;

namespace OmniSharp.Extensions.JsonRpc.Testing
{
    public interface ISettler
    {
        Task SettleNext();
        IObservable<ReactiveUnit> Settle();
    }
}
