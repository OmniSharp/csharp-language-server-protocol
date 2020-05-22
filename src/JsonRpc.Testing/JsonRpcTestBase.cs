using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc.Testing
{
    public abstract class JsonRpcTestBase
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        public JsonRpcTestBase(JsonRpcTestOptions testOptions)
        {
            TestOptions = testOptions;
            Disposable = new CompositeDisposable();
            Disposable.Add(testOptions.ClientLoggerFactory);
            Disposable.Add(testOptions.ServerLoggerFactory);

            _cancellationTokenSource = new CancellationTokenSource();
            if (!Debugger.IsAttached)
            {
                _cancellationTokenSource.CancelAfter(testOptions.TestTimeout);
            }

            ClientEvents = new Settler(TestOptions.SettleTimeSpan, TestOptions.SettleTimeout, CancellationToken);
            ServerEvents = new Settler(TestOptions.SettleTimeSpan, TestOptions.SettleTimeout, CancellationToken);
            Events = new AggregateSettler(ClientEvents, ServerEvents);
        }

        protected CompositeDisposable Disposable { get; }
        protected ISettler ClientEvents { get; }
        protected ISettler ServerEvents { get; }
        protected ISettler Events { get; }
        protected JsonRpcTestOptions TestOptions { get; }
        protected CancellationToken CancellationToken => _cancellationTokenSource.Token;
        protected Task SettleNext() => Events.SettleNext();
        protected IObservable<Unit> Settle() => Events.Settle();
    }
}
