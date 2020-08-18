using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc.Testing
{
    public abstract class JsonRpcTestBase : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        public JsonRpcTestBase(JsonRpcTestOptions testOptions)
        {
            TestOptions = testOptions;
            Disposable = new CompositeDisposable { testOptions.ClientLoggerFactory, testOptions.ServerLoggerFactory };

            _cancellationTokenSource = new CancellationTokenSource();
            if (!Debugger.IsAttached)
            {
                _cancellationTokenSource.CancelAfter(testOptions.CancellationTimeout);
            }

            ClientEvents = new Settler(TestOptions, CancellationToken);
            ServerEvents = new Settler(TestOptions, CancellationToken);
            Events = new AggregateSettler(ClientEvents, ServerEvents);
        }

        protected CompositeDisposable Disposable { get; }
        public ISettler ClientEvents { get; }
        public  ISettler ServerEvents { get; }
        public  ISettler Events { get; }
        public  JsonRpcTestOptions TestOptions { get; }
        public  CancellationToken CancellationToken => _cancellationTokenSource.Token;
        public  Task SettleNext() => Events.SettleNext();
        public  IObservable<Unit> Settle() => Events.Settle();

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
            Disposable?.Dispose();
        }
    }
}
