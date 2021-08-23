using System;
using System.Threading;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public class RequestInvocationHandle : IDisposable
    {
        private bool _disposed;

        public event Action<Request>? OnComplete;

        public RequestInvocationHandle(Request request)
        {
            Request = request;
            CancellationTokenSource = new CancellationTokenSource();
        }

        public Request Request { get; }

        public CancellationTokenSource CancellationTokenSource { get; }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            OnComplete?.Invoke(Request);
        }
    }
}
