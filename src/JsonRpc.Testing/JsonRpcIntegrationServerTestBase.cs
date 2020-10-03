using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc.Testing
{
    public abstract class JsonRpcIntegrationServerTestBase : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        public JsonRpcIntegrationServerTestBase(JsonRpcTestOptions testOptions)
        {
            TestOptions = testOptions;
            Disposable = new CompositeDisposable { testOptions.ClientLoggerFactory, testOptions.ServerLoggerFactory };

            _cancellationTokenSource = new CancellationTokenSource();
            if (!Debugger.IsAttached)
            {
                _cancellationTokenSource.CancelAfter(testOptions.CancellationTimeout);
            }

            Events = ClientEvents = new Settler(TestOptions, CancellationToken);
        }

        protected CompositeDisposable Disposable { get; }
        protected ISettler ClientEvents { get; }
        protected ISettler Events { get; }
        protected JsonRpcTestOptions TestOptions { get; }
        protected internal CancellationToken CancellationToken => _cancellationTokenSource.Token;
        protected Task SettleNext() => Events.SettleNext();
        protected IObservable<Unit> Settle() => Events.Settle();

        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
            Disposable.Dispose();
        }
    }
}
