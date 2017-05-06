using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace JsonRpc
{
    public class ProcessScheduler : IScheduler
    {
        private readonly BlockingCollection<(RequestProcessType type, Func<Task> request)> _queue;
        private readonly CancellationTokenSource _cancel;
        private Thread _queueThread;

        public ProcessScheduler() {
            _queue = new BlockingCollection<(RequestProcessType type, Func<Task> request)>();
            _cancel = new CancellationTokenSource();
            _queueThread = new Thread(ProcessRequestQueue) { IsBackground = true };
        }

        public void Start() {
            _queueThread.Start();
        }

        public void Add(RequestProcessType type, Func<Task> request) {
            _queue.Add((type, request));
        }

        private Task Start(Func<Task> request)
        {
            var t = request();
            if (!t.IsCompleted)
                t.Start();
            return t;
        }

        private void ProcessRequestQueue()
        {
            // see https://github.com/OmniSharp/csharp-language-server-protocol/issues/4
            // no need to be async, because this thing already allocated a thread on it's own.
            var token = _cancel.Token;
            var waitables = new List<Task>();
            while(true)
            {
                if (_queueThread == null) return;
                if (_queue.TryTake(out var item, Timeout.Infinite, token))
                {
                    var (type, request) = item;
                    if (type == RequestProcessType.Serial)
                    {
                        Task.WaitAll(waitables.ToArray(), token);
                        waitables.Clear();
                        Start(request).Wait(token);
                    }
                    else if (type == RequestProcessType.Parallel)
                    {
                        waitables.Add(Start(request));
                    }
                    else
                        throw new NotImplementedException("Only Serial and Parallel execution types can be handled currently");
                }
            }
        }

        public void Dispose()
        {
            _queueThread = null;
            _cancel.Cancel();
        }
    }
}