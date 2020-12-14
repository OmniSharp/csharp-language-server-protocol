using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Pipelines;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.JsonRpc
{
    public class OutputHandler : IOutputHandler
    {
        private readonly PipeWriter _pipeWriter;
        private readonly ISerializer _serializer;
        private readonly IEnumerable<IOutputFilter> _outputFilters;
        private readonly ILogger<OutputHandler> _logger;
        private readonly Subject<object> _queue;
        private readonly ReplaySubject<object> _delayedQueue;
        private readonly TaskCompletionSource<object?> _outputIsFinished;
        private readonly CompositeDisposable _disposable;
        private bool _delayComplete;

        public OutputHandler(
            PipeWriter pipeWriter,
            ISerializer serializer,
            IEnumerable<IOutputFilter> outputFilters,
            IScheduler scheduler,
            ILogger<OutputHandler> logger
        )
        {
            _pipeWriter = pipeWriter;
            _serializer = serializer;
            _outputFilters = outputFilters.ToArray();
            _logger = logger;
            _queue = new Subject<object>();
            _delayedQueue = new ReplaySubject<object>();
            _outputIsFinished = new TaskCompletionSource<object?>();

            _disposable = new CompositeDisposable {
                _queue
                   .ObserveOn(scheduler)
                   .Select(value => Observable.FromAsync(ct => ProcessOutputStream(value, ct)))
                   .Concat()
                   .Subscribe(),
                _delayedQueue
                   .ToArray()
                   .SelectMany(z => z)
                   .Subscribe(_queue.OnNext),
                _queue,
                _delayedQueue
            };
        }

        private bool ShouldSend(object value)
        {
            return _outputFilters.Any(z => z.ShouldOutput(value));
        }

        public void Send(object? value)
        {
            try
            {
                if (_queue.IsDisposed || _disposable.IsDisposed || value == null) return;
                if (!ShouldSend(value))
                {
                    if (_delayComplete || _delayedQueue.IsDisposed || !_delayedQueue.HasObservers) return;
                    _delayedQueue.OnNext(value);
                }
                else
                {
                    _queue.OnNext(value);
                }
            }
            catch (ObjectDisposedException) { }
        }

        public void Initialized()
        {
            if (_delayComplete || _delayedQueue.IsDisposed || !_delayedQueue.HasObservers) return;
            _delayedQueue.OnCompleted();
            _delayComplete = true;
            _delayedQueue.Dispose();
        }

        public async Task StopAsync()
        {
            await _pipeWriter.CompleteAsync().ConfigureAwait(false);
            _disposable.Dispose();
        }

        /// <summary>
        /// For unit test use only
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal async Task WriteAndFlush()
        {
            await _pipeWriter.FlushAsync().ConfigureAwait(false);
            await _pipeWriter.CompleteAsync().ConfigureAwait(false);
        }

        private async Task ProcessOutputStream(object value, CancellationToken cancellationToken)
        {
            try
            {
//                _logger.LogTrace("Writing out {@Value}", value);
                // TODO: this will be part of the serialization refactor to make streaming first class
                var content = _serializer.SerializeObject(value);
                var contentBytes = Encoding.UTF8.GetBytes(content).AsMemory();
                await _pipeWriter.WriteAsync(Encoding.UTF8.GetBytes($"Content-Length: {contentBytes.Length}\r\n\r\n"), cancellationToken).ConfigureAwait(false);
                await _pipeWriter.WriteAsync(contentBytes, cancellationToken).ConfigureAwait(false);
                await _pipeWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken != cancellationToken)
            {
                _logger.LogTrace(ex, "Cancellation happened");
                Error(ex);
            }
            catch (Exception e)
            {
                _logger.LogTrace(e, "Could not write to output handler, perhaps serialization failed?");
                Error(e);
            }
        }

        public Task WaitForShutdown() => _outputIsFinished.Task;

        private void Error(Exception ex)
        {
            _outputIsFinished.TrySetResult(ex);
            _disposable.Dispose();
        }

        public void Dispose()
        {
            _outputIsFinished.TrySetResult(null);
            _disposable.Dispose();
        }
    }
}
