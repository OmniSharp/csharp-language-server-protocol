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
        private readonly TimeSpan _sleepTime = TimeSpan.FromMilliseconds(50);
        private readonly TextWriter _output;
        private Thread _thread;
        private readonly ConcurrentQueue<object> _queue;

        public OutputHandler(TextWriter output)
        {
            _output = output;
            _queue = new ConcurrentQueue<object>();
            _thread = new Thread(ProcessOutputQueue) {
                IsBackground = true
            };
        }

        internal OutputHandler(TextWriter output, TimeSpan sleepTime)
            : this(output)
        {
            _sleepTime = sleepTime;
        }

        public void Start()
        {
            _thread.Start();
        }

        public void Send(object value)
        {
            _queue.Enqueue(value);
        }
        private void ProcessOutputQueue()
        {
            while (true)
            {
                if (_thread == null) return;

                if (_queue.TryDequeue(out var value))
                {
                    var content = JsonConvert.SerializeObject(value);

                    // TODO: Is this lsp specific??
                    var sb = new StringBuilder();
                    sb.Append($"Content-Length: {content.Length}\r\n");
                    sb.Append($"\r\n");
                    sb.Append(content);

                    _output.Write(sb.ToString());
                }

                if (_queue.IsEmpty)
                {
                    Thread.Sleep(_sleepTime);
                }
            }
        }

        public void Dispose()
        {
            _output?.Dispose();
            _thread = null;
        }
    }
}