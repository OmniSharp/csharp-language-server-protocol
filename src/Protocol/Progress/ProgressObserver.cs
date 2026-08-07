using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ReactiveUnit = System.Reactive.Unit;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Progress
{
    internal class ProgressObserver<TInitial, T> : ProgressObserver<T>, IProgressObserverWithInitialValue<TInitial, T>
    {
        private bool _isInitialized;
        
        public static ProgressObserver<TInitial, T> Noop { get; } =
            new ProgressObserver<TInitial, T>(new ProgressToken(nameof(Noop)), null, null, CancellationToken.None, () => { });

        public ProgressObserver(
            ProgressToken token, IResponseRouter? responseRouter, ISerializer? serializer, CancellationToken cancellationToken, Action disposal
        ) : base(token, responseRouter, serializer, cancellationToken, disposal)
        {
        }

        public void OnNext(TInitial initial)
        {
            if (_isInitialized || isComplete || responseRouter == null) return;
            responseRouter.SendNotification(
                new ProgressParams
                {
                    Token = ProgressToken,
                    Value = serializer is null ? JToken.FromObject(initial) : JToken.Parse(serializer.SerializeObject(initial))
                }
            );
            _isInitialized = true;
        }
    }

    internal class ProgressObserver<T> : IProgressObserver<T>
    {
        protected readonly IResponseRouter? responseRouter;
        protected readonly ISerializer? serializer;
        private readonly Action _disposal;
        private readonly TaskCompletionSource<ReactiveUnit> _completionSource;
        protected bool isComplete;

        public static ProgressObserver<T> Noop { get; } =
            new ProgressObserver<T>(new ProgressToken(nameof(Noop)), null, null, CancellationToken.None, () => { });

        public ProgressObserver(
            ProgressToken token,
            IResponseRouter? responseRouter,
            ISerializer? serializer,
            CancellationToken cancellationToken,
            Action disposal
        )
        {
            this.responseRouter = responseRouter;
            this.serializer = serializer;
            _disposal = disposal;
            ProgressToken = token;
            CancellationToken = cancellationToken;
            _completionSource = new TaskCompletionSource<ReactiveUnit>();
        }

        public TaskAwaiter<ReactiveUnit> GetAwaiter() => _completionSource.Task.GetAwaiter();
        public ProgressToken ProgressToken { get; }
        public CancellationToken CancellationToken { get; }
        public Type ParamsType { get; } = typeof(T);

        public void OnCompleted()
        {
            if (isComplete) return;
            _completionSource.TrySetResult(ReactiveUnit.Default);
            isComplete = true;
        }

        void IObserver<T>.OnError(Exception error)
        {
            if (isComplete) return;
            _completionSource.TrySetException(error);
            isComplete = true;
        }

        public void OnNext(T value)
        {
            if (isComplete || responseRouter == null) return;
            responseRouter.SendNotification(
                new ProgressParams
                {
                    Token = ProgressToken,
                    Value = serializer is null ? JToken.FromObject(value) : JToken.Parse(serializer.SerializeObject(value))
                }
            );
        }

        public void Dispose() => _disposal();
    }
}
