using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc
{
    public class InputHandler : IInputHandler
    {
        public const char CR = '\r';
        public const char LF = '\n';
        public static char[] CRLF = { CR, LF };
        public static char[] HeaderKeys = { CR, LF, ':' };
        public const short MinBuffer = 21; // Minimum size of the buffer "Content-Length: X\r\n\r\n"

        private readonly Stream _input;
        private readonly IOutputHandler _outputHandler;
        private readonly IReciever _reciever;
        private readonly IRequestProcessIdentifier _requestProcessIdentifier;
        private Thread _inputThread;
        private readonly IRequestRouter _requestRouter;
        private readonly IResponseRouter _responseRouter;
        private readonly ILogger<InputHandler> _logger;
        private readonly IScheduler _scheduler;

        public InputHandler(
            Stream input,
            IOutputHandler outputHandler,
            IReciever reciever,
            IRequestProcessIdentifier requestProcessIdentifier,
            IRequestRouter requestRouter,
            IResponseRouter responseRouter,
            ILoggerFactory loggerFactory
            )
        {
            if (!input.CanRead) throw new ArgumentException($"must provide a readable stream for {nameof(input)}", nameof(input));
            _input = input;
            _outputHandler = outputHandler;
            _reciever = reciever;
            _requestProcessIdentifier = requestProcessIdentifier;
            _requestRouter = requestRouter;
            _responseRouter = responseRouter;
            _logger = loggerFactory.CreateLogger<InputHandler>();
            _scheduler = new ProcessScheduler(loggerFactory);
            _inputThread = new Thread(ProcessInputStream) { IsBackground = true, Name = "ProcessInputStream" };
        }

        public void Start()
        {
            _outputHandler.Start();
            _inputThread.Start();
            _scheduler.Start();
        }

        // don't be async: We already allocated a seperate thread for this.
        private void ProcessInputStream()
        {
            // some time to attach a debugger
            // System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));

            // header is encoded in ASCII
            // "Content-Length: 0" counts bytes for the following content
            // content is encoded in UTF-8
            while (true)
            {
                if (_inputThread == null) return;

                var buffer = new byte[300];
                var current = _input.Read(buffer, 0, MinBuffer);
                if (current == 0) return; // no more _input
                while (current < MinBuffer ||
                       buffer[current - 4] != CR || buffer[current - 3] != LF ||
                       buffer[current - 2] != CR || buffer[current - 1] != LF)
                {
                    var n = _input.Read(buffer, current, 1);
                    if (n == 0) return; // no more _input, mitigates endless loop here.
                    current += n;
                }

                var headersContent = System.Text.Encoding.ASCII.GetString(buffer, 0, current);
                var headers = headersContent.Split(HeaderKeys, StringSplitOptions.RemoveEmptyEntries);
                long length = 0;
                for (var i = 1; i < headers.Length; i += 2)
                {
                    // starting at i = 1 instead of 0 won't throw, if we have uneven headers' length
                    var header = headers[i - 1];
                    var value = headers[i].Trim();
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
                    var requestBuffer = new byte[length];
                    var received = 0;
                    while (received < length)
                    {
                        var n = _input.Read(requestBuffer, received, requestBuffer.Length - received);
                        if (n == 0) return; // no more _input
                        received += n;
                    }
                    // TODO sometimes: encoding should be based on the respective header (including the wrong "utf8" value)
                    var payload = System.Text.Encoding.UTF8.GetString(requestBuffer);
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

                    if (response is ServerResponse serverResponse)
                    {
                        tcs.SetResult(serverResponse.Result);
                    }
                    else if (response is ServerError serverError)
                    {
                        tcs.SetException(new Exception(JsonConvert.SerializeObject(serverError.Error)));
                    }
                }

                return;
            }

            foreach (var (type, item) in requests.Select(x => (type: _requestProcessIdentifier.Identify(x), item: x)))
            {
                if (item.IsRequest)
                {
                    _scheduler.Add(
                        type,
                        item.Request.Method,
                        async () => {
                            try
                            {
                                var result = await _requestRouter.RouteRequest(item.Request);
                                _outputHandler.Send(result.Value);
                            }
                            catch (Exception e)
                            {
                                _logger.LogCritical(Events.UnhandledRequest, e, "Unhandled exception executing request {Method}@{Id}", item.Request.Method, item.Request.Id);
                                // TODO: Should we rethrow or swallow?
                                // If an exception happens... the whole system could be in a bad state, hence this throwing currently.
                                throw;
                            }
                        }
                    );
                }
                else if (item.IsNotification)
                {
                    _scheduler.Add(
                        type,
                        item.Notification.Method,
                        async () => {
                            try
                            {
                                await _requestRouter.RouteNotification(item.Notification);
                            }
                            catch (Exception e)
                            {
                                _logger.LogCritical(Events.UnhandledNotification, e, "Unhandled exception executing notification {Method}@{Id}", item.Notification.Method);
                                // TODO: Should we rethrow or swallow?
                                // If an exception happens... the whole system could be in a bad state, hence this throwing currently.
                                throw;
                            }
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
