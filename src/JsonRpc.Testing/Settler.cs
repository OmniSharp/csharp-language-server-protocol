using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using static System.Reactive.Linq.Observable;

namespace OmniSharp.Extensions.JsonRpc.Testing
{
    public class Settler : ISettler, IRequestSettler, IDisposable
    {
        private readonly JsonRpcTestOptions _options;
        private readonly CancellationToken _cancellationToken;
        private readonly IScheduler _scheduler;
        private readonly IObservable<Unit> _settle;
        private readonly IObserver<int> _requester;
        private readonly IDisposable _connectable;
        private readonly IObservable<Unit> _defaultValue;

        public Settler(JsonRpcTestOptions options, CancellationToken cancellationToken, IScheduler scheduler = null)
        {
            _options = options;
            _cancellationToken = cancellationToken;
            scheduler ??= Scheduler.Immediate;
            _scheduler = scheduler;
            _defaultValue = Return(Unit.Default, _scheduler);
            var subject = new Subject<int>();
            var data = subject;

            var connectable = data
                             .StartWith(0)
                             .Scan(
                                  0, (acc, next) => {
                                      acc += next;
                                      return acc;
                                  }
                              )
                             .DistinctUntilChanged()
                             .Select(
                                  z => {
                                      if (z > 0)
                                      {
                                          return Timer(_options.SettleTimeout, _scheduler)
                                             .Select(z => Unit.Default);
                                      }

                                      return Amb(Timer(_options.SettleTimeout, _scheduler), Timer(_options.SettleTimeout, _scheduler))
                                         .Select(z => Unit.Default);
                                  }
                              )
                             .Replay(1, _scheduler);
            _connectable = connectable.Connect();
            _settle = connectable
               .Switch();
            _requester = subject.AsObserver();
        }

        public Task SettleNext() => _settle.Take(1).IgnoreElements().LastOrDefaultAsync().ToTask(_cancellationToken);

        public IObservable<Unit> Settle() => _settle.Timeout(_options.SettleTimeout, _scheduler).Catch<Unit, Exception>(_ => _defaultValue);

        void IRequestSettler.OnStartRequest() => _requester.OnNext(1);

        void IRequestSettler.OnEndRequest() => _requester.OnNext(-1);

        public void Dispose() => _connectable?.Dispose();
    }
}
