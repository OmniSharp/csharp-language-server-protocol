using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace JsonRpc
{
    public class OutputHandler : IOutputHandler
    {
        private readonly Stream _output;
        private Thread _thread;
        private readonly BlockingCollection<object> _queue;
        private readonly CancellationTokenSource _cancel;

        public OutputHandler(Stream output)
        {
            if (!output.CanWrite) throw new ArgumentException($"must provide a writable stream for {nameof(output)}", nameof(output));
            _output = output;
            _queue = new BlockingCollection<object>();
            _cancel = new CancellationTokenSource();
            _thread = new Thread(ProcessOutputQueue) {
                IsBackground = true
            };
        }

        public void Start()
        {
            _thread.Start();
        }

        public void Send(object value)
        {
            _queue.Add(value);
        }

        private void ProcessOutputQueue()
        {
            var token = _cancel.Token;
            while (true)
            {
                if (_thread == null) return;
                try
                {
                    if (_queue.TryTake(out var value, Timeout.Infinite, token))
                    {
                        var content = JsonConvert.SerializeObject(value);
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
                            _output.Write(ms.ToArray(), 0, (int)ms.Position);
                        }
                    }
                }
                catch (OperationCanceledException) { }
            }
        }

        public void Dispose()
        {
            _output?.Dispose();
            _thread = null;
            _cancel.Cancel();
        }
    }
}
