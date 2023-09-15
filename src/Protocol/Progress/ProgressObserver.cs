using System;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

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
                    Value = JToken.FromObject(initial, serializer?.JsonSerializer)
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
        private readonly TaskCompletionSource<Unit> _completionSource;
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
            _completionSource = new TaskCompletionSource<Unit>();
        }

        public TaskAwaiter<Unit> GetAwaiter() => _completionSource.Task.GetAwaiter();
        public ProgressToken ProgressToken { get; }
        public CancellationToken CancellationToken { get; }
        public Type ParamsType { get; } = typeof(T);

        public void OnCompleted()
        {
            if (isComplete) return;
            _completionSource.TrySetResult(Unit.Default);
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
                    Value = JToken.FromObject(value, serializer?.JsonSerializer)
                }
            );
        }

        public void Dispose() => _disposal();
    }
}
