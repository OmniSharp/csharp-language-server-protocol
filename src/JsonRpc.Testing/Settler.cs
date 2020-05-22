using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc.Testing
{
    class Settler : ISettler, IRequestSettler
    {
        private readonly CancellationToken _cancellationToken;
        private readonly IObservable<Unit> _settle;
        private readonly IObserver<int> _requester;

        public Settler(TimeSpan waitTime, TimeSpan timeout, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            var subject = new Subject<int>();
            var data = subject;
            _settle = data
                .StartWith(0)
                .Scan(0, (acc, next) => {
                    acc += next;
                    return acc;
                })
                .Select(z => z <= 0 ? Observable.Timer(waitTime).Select(_ => Unit.Default).Timeout(timeout, Observable.Return(Unit.Default)) : Observable.Never<Unit>())
                .Switch()
                .Replay(1)
                .RefCount();
            _requester = subject;
        }

        public Task SettleNext()
        {
            return _settle
                .Take(1)
                .ToTask(_cancellationToken);
        }

        public IObservable<Unit> Settle()
        {
            return _settle;
        }

        void IRequestSettler.OnStartRequest()
        {
            _requester.OnNext(1);
        }

        void IRequestSettler.OnEndRequest()
        {
            _requester.OnNext(-1);
        }
    }
}
