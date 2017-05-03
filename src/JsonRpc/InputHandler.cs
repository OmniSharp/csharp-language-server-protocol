using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc.Server;
using JsonRpc.Server.Messages;
using Newtonsoft.Json.Linq;

namespace JsonRpc
{
    public class InputHandler : IInputHandler
    {
        private readonly TimeSpan _sleepTime = TimeSpan.FromMilliseconds(50);
        public const char CR = '\r';
        public const char LF = '\n';
        public static char[] CRLF = { CR, LF };
        public static char[] HeaderKeys = { CR, LF, ':' };
        public const short MinBuffer = 21; // Minimum size of the buffer "Content-Length: X\r\n\r\n"

        private readonly TextReader _input;
        private readonly IOutputHandler _outputHandler;
        private readonly IReciever _reciever;
        private readonly IRequestProcessIdentifier _requestProcessIdentifier;
        private Thread _inputThread;
        private readonly IRequestRouter _requestRouter;
        private readonly IResponseRouter _responseRouter;
        private readonly ConcurrentQueue<(RequestProcessType type, Func<Task> request)> _queue;
        private Thread _queueThread;

        public InputHandler(
            TextReader input,
            IOutputHandler outputHandler,
            IReciever reciever,
            IRequestProcessIdentifier requestProcessIdentifier,
            IRequestRouter requestRouter,
            IResponseRouter responseRouter
            )
        {
            _input = input;
            _outputHandler = outputHandler;
            _reciever = reciever;
            _requestProcessIdentifier = requestProcessIdentifier;
            _requestRouter = requestRouter;
            _responseRouter = responseRouter;
            _queue = new ConcurrentQueue<(RequestProcessType type, Func<Task> request)>();

            _inputThread = new Thread(ProcessInputStream) { IsBackground = true };

            _queueThread = new Thread(ProcessRequestQueue) { IsBackground = true };
        }

        internal InputHandler(
            TextReader input,
            IOutputHandler outputHandler,
            IReciever reciever,
            IRequestProcessIdentifier requestProcessIdentifier,
            IRequestRouter requestRouter,
            IResponseRouter responseRouter,
            TimeSpan sleepTime
        ) : this(input, outputHandler, reciever, requestProcessIdentifier, requestRouter, responseRouter)
        {
            _sleepTime = sleepTime;
        }

        public void Start()
        {
            _outputHandler.Start();
            _inputThread.Start();
            _queueThread.Start();
        }

        private async void ProcessInputStream()
        {
            while (true)
            {
                if (_inputThread == null) return;

                var buffer = new char[300];
                var current = await _input.ReadBlockAsync(buffer, 0, MinBuffer);
                while (current < MinBuffer || buffer[current - 4] != CR || buffer[current - 3] != LF ||
                       buffer[current - 2] != CR || buffer[current - 1] != LF)
                {
                    current += await _input.ReadBlockAsync(buffer, current, 1);
                }

                var headersContent = new string(buffer, 0, current);
                var headers = headersContent.Split(HeaderKeys, StringSplitOptions.RemoveEmptyEntries);
                long length = 0;
                for (var i = 0; i < headers.Length; i += 2)
                {
                    var header = headers[0];
                    var value = headers[i + 1].Trim();
                    if (header.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
                    {
                        length = long.Parse(value);
                    }
                }

                var requestBuffer = new char[length];

                await _input.ReadBlockAsync(requestBuffer, 0, requestBuffer.Length);

                var payload = new string(requestBuffer);

                HandleRequest(payload);
            }
        }

        private void HandleRequest(string request)
        {
            JToken payload;
            try
            {
                payload = JToken.Parse(request);
            }
            catch
            {
                _outputHandler.Send(new ParseError());
                return;
            }

            if (!_reciever.IsValid(payload))
            {
                _outputHandler.Send(new InvalidRequest());
                return;
            }

            var (requests, hasResponse) = _reciever.GetRequests(payload);
            if (hasResponse)
            {
                foreach (var response in requests.Where(x => x.IsResponse).Select(x => x.Response))
                {
                    var id = response.Id is string s ? long.Parse(s) : response.Id is long l ? l : -1;
                    if (id < 0) continue;

                    var tcs = _responseRouter.GetRequest(id);
                    if (tcs is null) continue;

                    if (response.Error is null)
                    {
                        tcs.SetResult(response.Result);
                    }
                    else
                    {
                        tcs.SetException(new Exception(response.Error));
                    }
                }

                return;
            }

            foreach (var (type, item) in requests.Select(x => (_requestProcessIdentifier.Identify(x), x)))
            {
                if (item.IsRequest)
                {
                    _queue.Enqueue((
                        type,
                        async () => {
                            var result = await _requestRouter.RouteRequest(item.Request);

                            _outputHandler.Send(result.Value);
                        }
                    ));
                }
                else if (item.IsNotification)
                {
                    _queue.Enqueue((
                        type,
                        () => {
                            _requestRouter.RouteNotification(item.Notification);
                            return Task.CompletedTask;
                        }
                    ));
                }
                else if (item.IsError)
                {
                    // TODO:
                    _outputHandler.Send(item.Error);
                }
            }
        }

        private bool IsNextSerial()
        {
            return _queue.TryPeek(out var queueResult) && queueResult.type == RequestProcessType.Serial;
        }

        private async void ProcessRequestQueue()
        {
            while (true)
            {
                if (_queueThread == null) return;
                var items = new List<Func<Task>>();
                while (!_queue.IsEmpty)
                {
                    if (IsNextSerial() && items.Count > 0)
                    {
                        break;
                    }

                    if (_queue.TryDequeue(out var queueResult))
                        items.Add(queueResult.request);
                }

                await Task.WhenAll(items.Select(x => x()));

                if (_queue.IsEmpty)
                {
                    await Task.Delay(_sleepTime);
                }
            }
        }

        public void Dispose()
        {
            _outputHandler.Dispose();
            _inputThread = null;
            _queueThread = null;
        }
    }
}