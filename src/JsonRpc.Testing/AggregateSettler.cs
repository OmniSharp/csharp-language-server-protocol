using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc.Testing
{
    public class AggregateSettler : ISettler
    {
        private readonly ISettler[] _settlers;

        public AggregateSettler(params ISettler[] settlers)
        {
            _settlers = settlers;
        }

        public Task SettleNext()
        {
            return _settlers
                .Select(z => z.Settle().Take(1))
                .ForkJoin()
                .LastOrDefaultAsync()
                .ToTask();
        }

        public IObservable<Unit> Settle() =>
            _settlers
                .Select(z => z.Settle())
                .ForkJoin()
                .Select(z => Unit.Default)
                .LastOrDefaultAsync();
    }
}
