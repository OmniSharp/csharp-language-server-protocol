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
        private readonly TextWriter _output;
        private Thread _thread;
        private readonly BlockingCollection<object> _queue;
        private readonly CancellationTokenSource _cancel;

        public OutputHandler(TextWriter output)
        {
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
                if (_queue.TryTake(out var value, -1, token))
                {
                    var content = JsonConvert.SerializeObject(value);

                    // TODO: Is this lsp specific??
                    var sb = new StringBuilder();
                    sb.Append($"Content-Length: {content.Length}\r\n");
                    sb.Append($"\r\n");
                    sb.Append(content);

                    _output.Write(sb.ToString());
                }
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