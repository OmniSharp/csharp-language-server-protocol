using System;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Progress
{
    internal class ProgressObserver<T> : IProgressObserver<T>
    {
        private readonly IResponseRouter? _responseRouter;
        private readonly ISerializer? _serializer;
        private readonly Action _disposal;
        private readonly TaskCompletionSource<Unit> _completionSource;
        private bool _isComplete;

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
            _responseRouter = responseRouter;
            _serializer = serializer;
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
            if (_isComplete) return;
            _completionSource.TrySetResult(Unit.Default);
            _isComplete = true;
        }

        void IObserver<T>.OnError(Exception error)
        {
            if (_isComplete) return;
            _completionSource.TrySetException(error);
            _isComplete = true;
        }

        public void OnNext(T value)
        {
            if (_isComplete || _responseRouter == null) return;
            _responseRouter.SendNotification(
                new ProgressParams {
                    Token = ProgressToken,
                    Value = JToken.FromObject(value, _serializer?.JsonSerializer)
                }
            );
        }

        public void Dispose() => _disposal();
    }
}
