using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Pipelines;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nerdbank.Streams;

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
        private readonly JsonWriterOptions _writerOptions;
        private static readonly byte[] ContentLengthHeader = Encoding.UTF8.GetBytes("Content-Length: ");
        private static readonly byte[] EndHeaders = Encoding.UTF8.GetBytes("\r\n\r\n");

        public OutputHandler(PipeWriter pipeWriter, ISerializer serializer, ILogger<OutputHandler> logger)
        {
            _pipeWriter = pipeWriter;
            _serializer = serializer;
            _logger = logger;
            _queue = new Subject<object>();
            _outputIsFinished = new TaskCompletionSource<object>();
            _writerOptions = new JsonWriterOptions() {
                Encoder = JavaScriptEncoder.Default,
                Indented = false,
                SkipValidation = false
            };

            _disposable = new CompositeDisposable {
                _queue
                    .Select(value => Observable.FromAsync(ct => ProcessOutputStream(value, ct)))
                    .Concat()
                    .ObserveOn(new EventLoopScheduler(_ => new Thread(_) {IsBackground = true, Name = "OutputHandler"}))
                    .Subscribe(),
                _queue,
                Disposable.Create(() => _pipeWriter?.Complete())
            };
        }

        public void Send(object value)
        {
            _queue.OnNext(value);
        }

        /// <summary>
        /// For unit test use only
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal async Task WriteAndFlush()
        {
            await _pipeWriter.FlushAsync();
            await _pipeWriter.CompleteAsync();
        }

        private async Task ProcessOutputStream(object value, CancellationToken cancellationToken)
        {
            try
            {
                using var sequence = new Sequence<byte>();
                await using var jsonWriter = new Utf8JsonWriter(sequence, _writerOptions);
                JsonSerializer.Serialize(jsonWriter, _serializer.Options);

                await _pipeWriter.WriteAsync(ContentLengthHeader, cancellationToken);
                await _pipeWriter.WriteAsync(Encoding.UTF8.GetBytes(sequence.Length.ToString()), cancellationToken);
                await _pipeWriter.WriteAsync(EndHeaders, cancellationToken);
                await _pipeWriter.WriteAsync(sequence.GetMemory(0), cancellationToken);
                await _pipeWriter.FlushAsync(cancellationToken);
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

        public Task WaitForShutdown()
        {
            return _outputIsFinished.Task;
        }

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
