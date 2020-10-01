using System;
using System.ComponentModel;
using System.IO.Pipelines;
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
        private readonly ILogger<OutputHandler> _logger;
        private readonly Subject<object> _queue;
        private readonly TaskCompletionSource<object> _outputIsFinished;
        private readonly CompositeDisposable _disposable;

        public OutputHandler(
            PipeWriter pipeWriter,
            ISerializer serializer,
            IReceiver receiver,
            IScheduler scheduler,
            ILogger<OutputHandler> logger
        )
        {
            _pipeWriter = pipeWriter;
            _serializer = serializer;
            _logger = logger;
            _queue = new Subject<object>();
            _outputIsFinished = new TaskCompletionSource<object>();

            _disposable = new CompositeDisposable {
                _queue
                   .ObserveOn(scheduler)
                   .Where(receiver.ShouldFilterOutput)
                   .Select(value => Observable.FromAsync(ct => ProcessOutputStream(value, ct)))
                   .Concat()
                   .Subscribe(),
                _queue
            };
        }

        public OutputHandler(
            PipeWriter pipeWriter,
            ISerializer serializer,
            IReceiver receiver,
            ILogger<OutputHandler> logger
        ) : this(
            pipeWriter,
            serializer,
            receiver,
            TaskPoolScheduler.Default,
            //new EventLoopScheduler(_ => new Thread(_) { IsBackground = true, Name = "OutputHandler" }),
            logger
        )
        {
        }

        public void Send(object value)
        {
            if (_queue.IsDisposed) return;
            _queue.OnNext(value);
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
