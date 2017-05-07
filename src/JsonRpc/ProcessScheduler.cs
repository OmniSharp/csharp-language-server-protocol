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
        private readonly Thread _thread;

        public ProcessScheduler()
        {
            _queue = new BlockingCollection<(RequestProcessType type, Func<Task> request)>();
            _cancel = new CancellationTokenSource();
            _thread = new Thread(ProcessRequestQueue) { IsBackground = true, Name = "ProcessRequestQueue" };
        }

        public void Start()
        {
            _thread.Start();
        }

        public void Add(RequestProcessType type, Func<Task> request)
        {
            _queue.Add((type, request));
        }

        private Task Start(Func<Task> request)
        {
            var t = request();
            if (t.Status == TaskStatus.Created) // || t.Status = TaskStatus.WaitingForActivation ?
                t.Start();
            return t;
        }

        private List<Task> RemoveCompleteTasks(List<Task> list)
        {
            if (list.Count == 0) return list;

            var result = new List<Task>();
            foreach(var t in list)
            {
                if (t.IsFaulted)
                {
                    // TODO: Handle Fault
                }
                else if (!t.IsCompleted)
                {
                    result.Add(t);
                }
            }
            return result;
        }

        public long _TestOnly_NonCompleteTaskCount = 0;
        private void ProcessRequestQueue()
        {
            // see https://github.com/OmniSharp/csharp-language-server-protocol/issues/4
            // no need to be async, because this thing already allocated a thread on it's own.
            var token = _cancel.Token;
            var waitables = new List<Task>();
            try
            {
                while (true)
                {
                    if (_queue.TryTake(out var item, Timeout.Infinite, token))
                    {
                        var (type, request) = item;
                        if (type == RequestProcessType.Serial)
                        {
                            Task.WaitAll(waitables.ToArray(), token);
                            Start(request).Wait(token);
                        }
                        else if (type == RequestProcessType.Parallel)
                        {
                            waitables.Add(Start(request));
                        }
                        else
                            throw new NotImplementedException("Only Serial and Parallel execution types can be handled currently");
                        waitables = RemoveCompleteTasks(waitables);
                        Interlocked.Exchange(ref _TestOnly_NonCompleteTaskCount, waitables.Count);
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                if (ex.CancellationToken != token)
                    throw;
                // OperationCanceledException - The CancellationToken has been canceled.
                Task.WaitAll(waitables.ToArray(), TimeSpan.FromMilliseconds(1000));
                var keeponrunning = RemoveCompleteTasks(waitables);
                Interlocked.Exchange(ref _TestOnly_NonCompleteTaskCount, keeponrunning.Count);
                keeponrunning.ForEach((t) =>
                {
                    // TODO: There is no way to abort a Task. As we don't construct the tasks, we can do nothing here
                    // Option is: change the task factory "Func<Task> request" to a "Func<CancellationToken, Task> request"
                });
            }
        }

        private bool _disposed = false;
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _cancel.Cancel();
            _thread.Join();
            _cancel.Dispose();
        }
    }
}
