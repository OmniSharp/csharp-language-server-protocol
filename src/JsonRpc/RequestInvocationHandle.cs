using System;
using System.Threading;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public class RequestInvocationHandle : IDisposable
    {
        private readonly object _completionGate = new object();
        private bool _disposed;
        private Action<Request>? _onComplete;

        /// <summary>
        /// Notifies subscribers when the request completes. Subscribers added after completion
        /// are invoked synchronously so that an already completed request can still be released.
        /// </summary>
        public event Action<Request>? OnComplete
        {
            add
            {
                bool completed;
                lock (_completionGate)
                {
                    completed = _disposed;
                    if (!completed)
                    {
                        _onComplete += value;
                    }
                }

                if (completed)
                {
                    value?.Invoke(Request);
                }
            }
            remove
            {
                lock (_completionGate)
                {
                    _onComplete -= value;
                }
            }
        }

        public RequestInvocationHandle(Request request)
        {
            Request = request;
            CancellationTokenSource = new CancellationTokenSource();
        }

        public Request Request { get; }

        public CancellationTokenSource CancellationTokenSource { get; }

        public void Dispose()
        {
            Action<Request>? completed;
            lock (_completionGate)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                completed = _onComplete;
                _onComplete = null;
            }

            completed?.Invoke(Request);
        }
    }
}
