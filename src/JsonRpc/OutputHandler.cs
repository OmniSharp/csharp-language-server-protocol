using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.JsonRpc
{
    public class OutputHandler : IOutputHandler
    {
        private readonly Stream _output;
        private readonly ISerializer _serializer;
        private readonly ILogger<OutputHandler> _outputHandler;
        private readonly Thread _thread;
        private readonly BlockingCollection<object> _queue;
        private readonly CancellationTokenSource _cancel;
        private readonly TaskCompletionSource<object> _outputIsFinished;

        public OutputHandler(Stream output, ISerializer serializer, ILogger<OutputHandler> outputHandler)
        {
            if (!output.CanWrite)
                throw new ArgumentException($"must provide a writable stream for {nameof(output)}", nameof(output));
            _output = output;
            _serializer = serializer;
            _outputHandler = outputHandler;
            _queue = new BlockingCollection<object>();
            _cancel = new CancellationTokenSource();
            _outputIsFinished = new TaskCompletionSource<object>();
            _thread = new Thread(ProcessOutputQueue) {IsBackground = true, Name = "ProcessOutputQueue"};
        }

        public void Start()
        {
            _thread.Start();
        }

        public void Send(object value, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
                _queue.Add(value);
        }

        private void ProcessOutputQueue()
        {
            var token = _cancel.Token;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (_queue.TryTake(out var value, Timeout.Infinite, token))
                    {
                        var content = _serializer.SerializeObject(value);
                        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

                        // TODO: Is this lsp specific??
                        var sb = new StringBuilder();
                        sb.Append($"Content-Length: {contentBytes.Length}\r\n");
                        sb.Append($"\r\n");
                        var headerBytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());

                        // only one write to _output
                        using (var ms = new MemoryStream(headerBytes.Length + contentBytes.Length))
                        {
                            ms.Write(headerBytes, 0, headerBytes.Length);
                            ms.Write(contentBytes, 0, contentBytes.Length);
                            if (!token.IsCancellationRequested)
                            {
                                _output.Write(ms.ToArray(), 0, (int) ms.Position);
                            }
                        }

                        _output.Flush();
                    }
                }
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken != token)
            {
                _outputHandler.LogTrace(ex, "Cancellation happened");
                _outputIsFinished.TrySetException(ex);
                // _thread.Abort(ex);
                // else ignore. Exceptions: OperationCanceledException - The CancellationToken has been canceled.
            }
            catch (Exception e)
            {
                _outputHandler.LogTrace(e, "Could not write to output handler, perhaps serialization failed?");
                _outputIsFinished.TrySetException(e);
                // _thread.Abort(e);
            }
        }

        public Task WaitForShutdown()
        {
            return _outputIsFinished.Task;
        }

        public void Dispose()
        {
            _outputIsFinished.TrySetResult(null);
            _cancel.Cancel();
            _thread.Join();
            _cancel.Dispose();
            _output.Dispose();
        }
    }
}
