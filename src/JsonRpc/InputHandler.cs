using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nerdbank.Streams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;
using Notification = OmniSharp.Extensions.JsonRpc.Server.Notification;

namespace OmniSharp.Extensions.JsonRpc
{
    public class InputHandler : IInputHandler, IDisposable
    {
        public static readonly byte[] HeadersFinished =
            new[] { (byte) '\r', (byte) '\n', (byte) '\r', (byte) '\n' }.ToArray();

        public const int HeadersFinishedLength = 4;
        public static readonly char[] HeaderKeys = { '\r', '\n', ':' };
        public const short MinBuffer = 21; // Minimum size of the buffer "Content-Length: X\r\n\r\n"
        public static readonly byte[] ContentLength = "Content-Length".Select(x => (byte) x).ToArray();
        public static readonly int ContentLengthLength = 14;

        private readonly PipeReader _pipeReader;
        private readonly IOutputHandler _outputHandler;
        private readonly IReceiver _receiver;
        private readonly IRequestProcessIdentifier _requestProcessIdentifier;
        private readonly IRequestRouter<IHandlerDescriptor> _requestRouter;
        private readonly IResponseRouter _responseRouter;
        private readonly OnUnhandledExceptionHandler _unhandledInputProcessException;
        private readonly CreateResponseExceptionHandler _getException;
        private readonly TimeSpan _requestTimeout;
        private readonly ILogger<InputHandler> _logger;
        private readonly ProcessScheduler _scheduler;
        private readonly Memory<byte> _headersBuffer;
        private readonly Memory<byte> _contentLengthBuffer;
        private readonly byte[] _contentLengthValueBuffer;
        private readonly Memory<byte> _contentLengthValueMemory;
        private readonly CancellationTokenSource _stopProcessing;
        private readonly CompositeDisposable _disposable;
        private readonly AsyncSubject<Unit> _inputActive;

        private readonly ConcurrentDictionary<object, (CancellationTokenSource cancellationTokenSource, IRequestDescriptor<IHandlerDescriptor> descriptor)> _requests =
            new ConcurrentDictionary<object, (CancellationTokenSource cancellationTokenSource, IRequestDescriptor<IHandlerDescriptor> descriptor)>();

        private readonly Subject<IObservable<Unit>> _inputQueue;

        public InputHandler(
            PipeReader pipeReader,
            IOutputHandler outputHandler,
            IReceiver receiver,
            IRequestProcessIdentifier requestProcessIdentifier,
            IRequestRouter<IHandlerDescriptor> requestRouter,
            IResponseRouter responseRouter,
            ILoggerFactory loggerFactory,
            OnUnhandledExceptionHandler unhandledInputProcessException,
            CreateResponseExceptionHandler getException,
            TimeSpan requestTimeout,
            bool supportContentModified,
            int? concurrency
        )
        {
            _pipeReader = pipeReader;
            _outputHandler = outputHandler;
            _receiver = receiver;
            _requestProcessIdentifier = requestProcessIdentifier;
            _requestRouter = requestRouter;
            _responseRouter = responseRouter;
            _unhandledInputProcessException = unhandledInputProcessException;
            _getException = getException;
            _requestTimeout = requestTimeout;
            _logger = loggerFactory.CreateLogger<InputHandler>();
            _scheduler = new ProcessScheduler(
                loggerFactory,
                supportContentModified,
                concurrency,
                requestTimeout,
                TaskPoolScheduler.Default
                // new EventLoopScheduler(_ => new Thread(_) {IsBackground = true, Name = "InputHandler"})
            );
            _headersBuffer = new Memory<byte>(new byte[HeadersFinishedLength]);
            _contentLengthBuffer = new Memory<byte>(new byte[ContentLengthLength]);
            _contentLengthValueBuffer = new byte[20]; // Max string length of the long value
            _contentLengthValueMemory =
                new Memory<byte>(_contentLengthValueBuffer); // Max string length of the long value
            _stopProcessing = new CancellationTokenSource();

            _disposable = new CompositeDisposable {
                Disposable.Create(() => _stopProcessing.Cancel()),
                _stopProcessing,
                _scheduler,
            };

            _inputActive = new AsyncSubject<Unit>();
            _inputQueue = new Subject<IObservable<Unit>>();
        }

        public void Start()
        {
            _disposable.Add(
                Observable.FromAsync(() => ProcessInputStream(_stopProcessing.Token))
                          .Do(_ => { }, e => _logger.LogCritical(e, "unhandled exception"))
                          .Subscribe(_inputActive)
            );
            _disposable.Add(
                _inputQueue
                   .Concat()
                   .Subscribe()
            );
        }

        public async Task StopAsync()
        {
            await _outputHandler.StopAsync();
            await _pipeReader.CompleteAsync();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _pipeReader?.Complete();
            _outputHandler?.Dispose();
        }

        public Task InputCompleted => _inputActive.ToTask();

        private bool TryParseHeaders(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
        {
            if (buffer.Length < MinBuffer || buffer.Length < HeadersFinishedLength)
            {
                line = default;
                return false;
            }

            var rentedSpan = _headersBuffer.Span;

            var start = buffer.PositionOf((byte) '\r');
            do
            {
                if (!start.HasValue)
                {
                    line = default;
                    return false;
                }

                var startSlice = buffer.Slice(start.Value);
                if (startSlice.Length < HeadersFinishedLength)
                {
                    line = default;
                    return false;
                }

                var next = buffer.Slice(start.Value, buffer.GetPosition(HeadersFinishedLength, start.Value));
                next.CopyTo(rentedSpan);
                if (IsEqual(rentedSpan, HeadersFinished))
                {
                    line = buffer.Slice(0, next.End);
                    buffer = buffer.Slice(next.End);
                    return true;
                }

                start = buffer.Slice(buffer.GetPosition(HeadersFinishedLength, start.Value)).PositionOf((byte) '\r');
            } while (start.HasValue && buffer.Length > MinBuffer);

            line = default;
            return false;
        }

        private static bool IsEqual(in Span<byte> headers, in byte[] bytes)
        {
            var isEqual = true;
            var len = bytes.Length;
            for (var i = 0; i < len; i++)
            {
                if (bytes[i] == headers[i]) continue;
                isEqual = false;
                break;
            }

            return isEqual;
        }

        private bool TryParseBodyString(
            in long length, ref ReadOnlySequence<byte> buffer,
            out ReadOnlySequence<byte> line
        )
        {
            if (buffer.Length < length)
            {
                line = default;
                return false;
            }


            line = buffer.Slice(0, length);
            buffer = buffer.Slice(length);
            return true;
        }

        private bool TryParseContentLength(ref ReadOnlySequence<byte> buffer, out long length)
        {
            do
            {
                var colon = buffer.PositionOf((byte) ':');
                if (!colon.HasValue)
                {
                    length = -1;
                    return false;
                }

                var slice = buffer.Slice(0, colon.Value);
                slice.CopyTo(_contentLengthBuffer.Span);

                if (IsEqual(_contentLengthBuffer.Span, ContentLength))
                {
                    var position = buffer.GetPosition(1, colon.Value);
                    var offset = 1;

                    while (buffer.TryGet(ref position, out var memory) && !memory.Span.IsEmpty)
                    {
                        foreach (var t in memory.Span)
                        {
                            if (t == (byte) ' ')
                            {
                                offset++;
                                continue;
                            }

                            break;
                        }
                    }

                    var lengthSlice = buffer.Slice(
                        buffer.GetPosition(offset, colon.Value),
                        buffer.PositionOf((byte) '\r') ?? buffer.End
                    );

                    var whitespacePosition = lengthSlice.PositionOf((byte) ' ');
                    if (whitespacePosition.HasValue)
                    {
                        lengthSlice = lengthSlice.Slice(0, whitespacePosition.Value);
                    }

                    lengthSlice.CopyTo(_contentLengthValueMemory.Span);
                    if (long.TryParse(Encoding.ASCII.GetString(_contentLengthValueBuffer), out length))
                    {
                        // Reset the array otherwise smaller numbers will be inflated;
                        for (var i = 0; i < lengthSlice.Length; i++) _contentLengthValueMemory.Span[i] = 0;
                        return true;
                    }

                    // Reset the array otherwise smaller numbers will be inflated;
                    for (var i = 0; i < lengthSlice.Length; i++) _contentLengthValueMemory.Span[i] = 0;

                    _logger.LogError("Unable to get length from content length header...");
                    return false;
                }

                buffer = buffer.Slice(buffer.GetPosition(1, buffer.PositionOf((byte) '\n') ?? buffer.End));
            } while (true);
        }

        internal async Task ProcessInputStream(CancellationToken cancellationToken)
        {
            // some time to attach a debugger
            // System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
            ReadOnlySequence<byte> buffer = default;
            try
            {
                var headersParsed = false;
                long length = 0;
                do
                {
                    var result = await _pipeReader.ReadAsync(cancellationToken);
                    buffer = result.Buffer;

                    var dataParsed = true;
                    do
                    {
                        dataParsed = false;
                        if (!headersParsed)
                        {
                            if (TryParseHeaders(ref buffer, out var line))
                            {
                                if (TryParseContentLength(ref line, out length))
                                {
                                    headersParsed = true;
                                }
                            }
                        }

                        if (headersParsed && length == 0)
                        {
                            HandleRequest(new ReadOnlySequence<byte>(Array.Empty<byte>()));
                            headersParsed = false;
                        }

                        if (headersParsed)
                        {
                            if (TryParseBodyString(length, ref buffer, out var line))
                            {
                                headersParsed = false;
                                length = 0;
                                HandleRequest(line);
                                dataParsed = true;
                            }
                        }
                    } while (!buffer.IsEmpty && dataParsed);

                    _pipeReader.AdvanceTo(buffer.Start, buffer.End);

                    // Stop reading if there's no more data coming.
                    if (result.IsCompleted && buffer.IsEmpty)
                    {
                        break;
                    }
                } while (!cancellationToken.IsCancellationRequested);
            }
            catch (Exception e)
            {
                var outerException = new InputProcessingException(Encoding.UTF8.GetString(buffer.ToArray()), e);
                _unhandledInputProcessException(outerException);
                throw outerException;
            }
            finally
            {
                await _outputHandler.StopAsync();
                await _pipeReader.CompleteAsync();
            }
        }

        private void HandleRequest(in ReadOnlySequence<byte> request)
        {
            JToken payload;
            try
            {
                using var textReader = new StreamReader(request.AsStream());
                using var reader = new JsonTextReader(textReader);
                payload = JToken.Load(reader);
            }
            catch
            {
                _outputHandler.Send(new ParseError(string.Empty));
                return;
            }

            if (!_receiver.IsValid(payload))
            {
                _outputHandler.Send(new InvalidRequest(string.Empty));
                return;
            }

            // using (_logger.TimeDebug("InputHandler is handling the request"))
            // {
            var (requests, hasResponse) = _receiver.GetRequests(payload);
            if (hasResponse)
            {
                foreach (var response in requests.Where(x => x.IsResponse).Select(x => x.Response))
                {
                    // _logger.LogDebug("Handling Response for request {ResponseId}", response.Id);
                    var id = response.Id is string s ? long.Parse(s) : response.Id is long l ? l : -1;
                    if (id < 0)
                    {
                        // _logger.LogDebug("Id was out of range, skipping request {ResponseId}", response.Id);
                        continue;
                    }

                    if (!_responseRouter.TryGetRequest(id, out var method, out var tcs))
                    {
                        // _logger.LogDebug("Request {ResponseId} was not found in the response router, unable to complete", response.Id);
                        continue;
                    }

                    _inputQueue.OnNext(
                        Observable.Create<Unit>(
                            observer => {
                                if (response is ServerResponse serverResponse)
                                {
                                    // _logger.LogDebug("Setting successful Response for {ResponseId}", response.Id);
                                    tcs.TrySetResult(serverResponse.Result);
                                }
                                else if (response is ServerError serverError)
                                {
                                    // _logger.LogDebug("Setting error for {ResponseId}", response.Id);
                                    tcs.TrySetException(DefaultErrorParser(method, serverError, _getException));
                                }

                                observer.OnCompleted();
                                return Disposable.Empty;
                            }
                        )
                    );
                }

                return;
            }

            foreach (var item in requests)
            {
                if (item.IsRequest)
                {
                    // _logger.LogDebug("Handling Request {Method} {ResponseId}", item.Request.Method, item.Request.Id);
                    var descriptor = _requestRouter.GetDescriptors(item.Request);
                    if (descriptor.Default is null)
                    {
                        _logger.LogDebug("Request handler was not found (or not setup) {Method} {ResponseId}", item.Request.Method, item.Request.Id);
                        _outputHandler.Send(new MethodNotFound(item.Request.Id, item.Request.Method));
                        return;
                    }

                    var type = _requestProcessIdentifier.Identify(descriptor.Default);
                    _scheduler.Add(type, $"{item.Request.Method}:{item.Request.Id}", RouteRequest(descriptor, item.Request));
                }

                if (item.IsNotification)
                {
                    // We need to special case cancellation so that we can cancel any request that is currently in flight.
                    if (item.Notification.Method == JsonRpcNames.CancelRequest)
                    {
                        _logger.LogDebug("Found cancellation request {Method}", item.Notification.Method);
                        var cancelParams = item.Notification.Params?.ToObject<CancelParams>();
                        if (cancelParams == null)
                        {
                            _logger.LogDebug("Got incorrect cancellation params", item.Notification.Method);
                            continue;
                        }

                        _logger.LogDebug("Cancelling pending request", item.Notification.Method);
                        if (_requests.TryGetValue(cancelParams.Id, out var d))
                        {
                            d.cancellationTokenSource.Cancel();
                        }

                        continue;
                    }

                    // _logger.LogDebug("Handling Request {Method}", item.Notification.Method);
                    var descriptor = _requestRouter.GetDescriptors(item.Notification);
                    if (descriptor.Default is null)
                    {
                        _logger.LogDebug("Notification handler was not found (or not setup) {Method}", item.Notification.Method);
                        // TODO: Figure out a good way to send this feedback back.
                        // _outputHandler.Send(new RpcError(null, new ErrorMessage(-32601, $"Method not found - {item.Notification.Method}")));
                        return;
                    }

                    var type = _requestProcessIdentifier.Identify(descriptor.Default);
                    _scheduler.Add(type, item.Notification.Method, RouteNotification(descriptor, item.Notification));
                }

                if (item.IsError)
                {
                    _outputHandler.Send(item.Error);
                }
            }
        }

        private SchedulerDelegate RouteRequest(IRequestDescriptor<IHandlerDescriptor> descriptors, Request request)
        {
            // start request, create cts, etc
            var cts = new CancellationTokenSource();
            _requests.TryAdd(request.Id, ( cts, descriptors ));

            return (contentModifiedToken, scheduler) =>
                Observable.Create<ErrorResponse>(
                               observer => {
                                   // ITS A RACE!
                                   var sub = Observable.Amb(
                                                            contentModifiedToken.Select(
                                                                _ => {
                                                                    _logger.LogTrace(
                                                                        "Request {Id} was abandoned due to content be modified", request.Id
                                                                    );
                                                                    return new ErrorResponse(
                                                                        new ContentModified(request.Id, request.Method)
                                                                    );
                                                                }
                                                            ),
                                                            Observable.Timer(_requestTimeout, scheduler).Select(
                                                                z => new ErrorResponse(new RequestCancelled(request.Id, request.Method))
                                                            ),
                                                            Observable.FromAsync(
                                                                async ct => {
                                                                    using var timer = _logger.TimeDebug(
                                                                        "Processing request {Method} {ResponseId}", request.Method,
                                                                        request.Id
                                                                    );
                                                                    ct.Register(cts.Cancel);
                                                                    // ObservableToToken(contentModifiedToken).Register(cts.Cancel);
                                                                    try
                                                                    {
                                                                        return await _requestRouter.RouteRequest(
                                                                            descriptors, request, cts.Token
                                                                        );
                                                                    }
                                                                    catch (OperationCanceledException)
                                                                    {
                                                                        _logger.LogTrace("Request {Id} was cancelled", request.Id);
                                                                        return new RequestCancelled(request.Id, request.Method);
                                                                    }
                                                                    catch (RpcErrorException e)
                                                                    {
                                                                        _logger.LogCritical(
                                                                            Events.UnhandledRequest, e,
                                                                            "Failed to handle request {Method} {RequestId}", request.Method,
                                                                            request.Id
                                                                        );
                                                                        return new RpcError(
                                                                            request.Id, request.Method,
                                                                            new ErrorMessage(e.Code, e.Message, e.Error)
                                                                        );
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        _logger.LogCritical(
                                                                            Events.UnhandledRequest, e,
                                                                            "Failed to handle request {Method} {RequestId}", request.Method,
                                                                            request.Id
                                                                        );
                                                                        return new InternalError(request.Id, request.Method, e.ToString());
                                                                    }
                                                                }
                                                            )
                                                        )
                                                       .Subscribe(observer);
                                   return new CompositeDisposable {
                                       sub,
                                       Disposable.Create(
                                           () => {
                                               if (_requests.TryRemove(request.Id, out var v))
                                               {
                                                   v.cancellationTokenSource.Dispose();
                                               }
                                           }
                                       )
                                   };
                               }
                           )
                          .Select(
                               response => {
                                   _outputHandler.Send(response.Value);
                                   return Unit.Default;
                               }
                           );
        }

        private SchedulerDelegate RouteNotification(IRequestDescriptor<IHandlerDescriptor> descriptors, Notification notification) =>
            (contentModifiedToken, scheduler) =>
                // ITS A RACE!
                Observable.Amb(
                    Observable.Timer(_requestTimeout, scheduler)
                              .Select(z => Unit.Default)
                              .Do(
                                   z => _logger.LogTrace("Notification was cancelled due to timeout")
                               ),
                    Observable.FromAsync(
                        async ct => {
                            using var timer = _logger.TimeDebug("Processing notification {Method}", notification.Method);
                            try
                            {
                                await _requestRouter.RouteNotification(descriptors, notification, ct);
                            }
                            catch (OperationCanceledException)
                            {
                                _logger.LogTrace("Notification was cancelled");
                            }
                            catch (Exception e)
                            {
                                _logger.LogCritical(Events.UnhandledRequest, e, "Failed to handle request {Method}", notification.Method);
                            }
                        }
                    )
                );

        private static Exception DefaultErrorParser(string method, ServerError error, CreateResponseExceptionHandler customHandler) =>
            error.Error?.Code switch {
                ErrorCodes.ServerNotInitialized => new ServerNotInitializedException(error.Id),
                ErrorCodes.MethodNotSupported   => new MethodNotSupportedException(error.Id, method ?? "UNKNOWN"),
                ErrorCodes.InvalidRequest       => new InvalidRequestException(error.Id),
                ErrorCodes.InvalidParameters    => new InvalidParametersException(error.Id),
                ErrorCodes.InternalError        => new InternalErrorException(error.Id, error.Error.Data?.ToString() ?? string.Empty),
                ErrorCodes.ParseError           => new ParseErrorException(error.Id),
                ErrorCodes.RequestCancelled     => new RequestCancelledException(error.Id),
                ErrorCodes.ContentModified      => new ContentModifiedException(error.Id),
                ErrorCodes.UnknownErrorCode     => new UnknownErrorException(error.Id),
                ErrorCodes.Exception            => new JsonRpcException(ErrorCodes.Exception, error.Id, error.Error.Message ?? string.Empty, error.Error.Data.ToString()),
                _ => customHandler?.Invoke(error, method) ??
                     new JsonRpcException(
                         error.Error?.Code ?? ErrorCodes.UnknownErrorCode, error.Id, error.Error?.Message ?? string.Empty,
                         error.Error?.Data.ToString() ?? string.Empty
                     )
            };
    }
}
