using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc.Server.Messages;
using Newtonsoft.Json.Linq;

namespace JsonRpc
{
    public class InputHandler : IInputHandler
    {
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
        private readonly IScheduler _scheduler;

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

            _scheduler = new ProcessScheduler();
            _inputThread = new Thread(ProcessInputStream) { IsBackground = true, Name = "ProcessInputStream" };
            }

        public void Start()
        {
            _outputHandler.Start();
            _inputThread.Start();
            _scheduler.Start();
        }

        private async void ProcessInputStream()
        {
            while (true)
            {
                if (_inputThread == null) return;

                var buffer = new char[300];
                var current = await _input.ReadBlockAsync(buffer, 0, MinBuffer);
                if (current == 0) return; // no more _input

                while (current < MinBuffer || buffer[current - 4] != CR || buffer[current - 3] != LF ||
                       buffer[current - 2] != CR || buffer[current - 1] != LF)
                {
                    var n = await _input.ReadBlockAsync(buffer, current, 1);
                    if (n == 0) return; // no more _input, mitigates endless loop here.
                    current += n;
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
                        length = 0;
                        long.TryParse(value, out length);
                    }
                }

                if (length == 0 || length >= int.MaxValue)
                {
                    HandleRequest(string.Empty);
                }
                else
                {
                    var requestBuffer = new char[length];
                    var received = 0;
                    while (received < length)
                    {
                        var n = await _input.ReadBlockAsync(requestBuffer, received, requestBuffer.Length - received);
                        if (n == 0) return; // no more _input
                        received += n;
                    }
                    var payload = new string(requestBuffer);
                    HandleRequest(payload);
                }
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
                    _scheduler.Add(
                        type,
                        async () => {
                            var result = await _requestRouter.RouteRequest(item.Request);
                            _outputHandler.Send(result.Value);
                        }
                    );
                }
                else if (item.IsNotification)
                {
                    _scheduler.Add(
                        type,
                        () => {
                            _requestRouter.RouteNotification(item.Notification);
                            return Task.CompletedTask;
                        }
                    );
                }
                else if (item.IsError)
                {
                    // TODO:
                    _outputHandler.Send(item.Error);
                }
            }
        }


        public void Dispose()
        {
            _outputHandler.Dispose();
            _inputThread = null;
            _scheduler.Dispose();
        }
    }
}