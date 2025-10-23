using System.ComponentModel;
using System.IO.Pipelines;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.JsonRpc
{
    public class OutputHandler : IOutputHandler
    {
        private readonly PipeWriter _pipeWriter;
        private readonly ISerializer _serializer;
        private readonly IEnumerable<IOutputFilter> _outputFilters;
        private readonly ILogger<OutputHandler> _logger;
        private readonly IActivityTracingStrategy? _activityTracingStrategy;


        private readonly ChannelReader<object> _queue;
        private readonly Queue<object> _delayedQueue;
        private readonly TaskCompletionSource<object?> _outputIsFinished;
        private readonly CompositeDisposable _disposable;
        private bool _delayComplete;
        private readonly Channel<object> _channel;
        private readonly ChannelWriter<object> _writer;

        public OutputHandler(
            PipeWriter pipeWriter,
            ISerializer serializer,
            IEnumerable<IOutputFilter> outputFilters,
            IScheduler scheduler,
            ILogger<OutputHandler> logger,
            IActivityTracingStrategy? activityTracingStrategy = null
        )
        {
            _pipeWriter = pipeWriter;
            _serializer = serializer;
            _outputFilters = outputFilters.ToArray();
            _logger = logger;
            _activityTracingStrategy = activityTracingStrategy;
            _delayedQueue = new Queue<object>();
            _outputIsFinished = new TaskCompletionSource<object?>();

            _channel = Channel.CreateUnbounded<object>(
                new UnboundedChannelOptions
                {
                    AllowSynchronousContinuations = true,
                    SingleReader = true,
                    SingleWriter = false
                }
            );
            _queue = _channel.Reader;
            _writer = _channel.Writer;

            var stopProcessing = new CancellationTokenSource();
            _disposable = new CompositeDisposable
            {
                Disposable.Create(() => stopProcessing.Cancel()),
                stopProcessing,
                Observable.FromAsync(() => ProcessOutputStream(stopProcessing.Token))
                          .Do(_ => { }, e => _logger.LogCritical(e, "unhandled exception"))
                          .Subscribe()
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
                if (_disposable.IsDisposed || value == null) return;
                if (!ShouldSend(value) && !_delayComplete)
                {
                    _delayedQueue.Enqueue(value);
                }
                else
                {
                    _writer.TryWrite(value);
                }
            }
            catch (ObjectDisposedException)
            {
            }
        }

        public void Initialized()
        {
            if (_delayComplete) return;
            while (_delayedQueue.Count > 0)
            {
                var item = _delayedQueue.Dequeue();
                _writer.TryWrite(item);
            }

            _delayComplete = true;
            _delayedQueue.Clear();
        }

        public async Task StopAsync()
        {
            _channel.Writer.TryComplete();
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

        private async Task ProcessOutputStream(CancellationToken cancellationToken)
        {
            try
            {
                do
                {
                    var value = await _queue.ReadAsync(cancellationToken).ConfigureAwait(false);
                    if (value is ITraceData traceData)
                    {
                        _activityTracingStrategy?.ApplyOutgoing(traceData);
                    }

                    // TODO: this will be part of the serialization refactor to make streaming first class
                    var content = _serializer.SerializeObject(value);
                    var contentBytes = Encoding.UTF8.GetBytes(content).AsMemory();
                    await _pipeWriter.WriteAsync(Encoding.UTF8.GetBytes($"Content-Length: {contentBytes.Length}\r\n\r\n"), cancellationToken)
                                     .ConfigureAwait(false);
                    await _pipeWriter.WriteAsync(contentBytes, cancellationToken).ConfigureAwait(false);
                    await _pipeWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
                } while (true);
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken != cancellationToken)
            {
                _logger.LogTrace(ex, "Cancellation happened");
                Error(ex);
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == cancellationToken)
            {
                _logger.LogTrace(ex, "Cancellation happened");
                Cancel();
            }
            catch (Exception e)
            {
                _logger.LogTrace(e, "Could not write to output handler, perhaps serialization failed?");
                Error(e);
            }
        }

        public Task WaitForShutdown()
        {
            return _outputIsFinished.Task;
        }

        private void Error(Exception ex)
        {
            _outputIsFinished.TrySetResult(ex);
            _writer.TryComplete();
            _disposable.Dispose();
        }

        private void Cancel()
        {
            _outputIsFinished.TrySetCanceled();
            _writer.TryComplete();
            _disposable.Dispose();
        }

        public void Dispose()
        {
            _outputIsFinished.TrySetResult(null);
            _writer.TryComplete();
            _disposable.Dispose();
        }
    }
}
