using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc.Testing
{
    class AggregateSettler : ISettler
    {
        private readonly ISettler[] _settlers;

        public AggregateSettler(params ISettler[] settlers)
        {
            _settlers = settlers;
        }

        public Task SettleNext()
        {
            return _settlers.ToObservable()
                .Select(z => z.Settle())
                .Merge()
                .Take(1)
                //.Amb(Observable.Timer(_waitTime + _waitTime).Select(z => Unit.Value))
                .ToTask();
        }

        public IObservable<Unit> Settle() =>
            _settlers.ToObservable()
                .Select(z => z.Settle())
                .Switch();
    }
}
