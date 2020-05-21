using System;
using System.Reactive;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc.Testing
{
    public interface ISettler
    {
        Task SettleNext();
        IObservable<Unit> Settle();
    }
}
