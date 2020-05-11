using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nerdbank.Streams;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;
using Notification = OmniSharp.Extensions.JsonRpc.Server.Notification;

namespace OmniSharp.Extensions.JsonRpc
{
    public class InputHandler : IInputHandler
    {
        public static readonly byte[] HeadersFinished =
            new byte[] {(byte) '\r', (byte) '\n', (byte) '\r', (byte) '\n'}.ToArray();

        public const int HeadersFinishedLength = 4;
        public static readonly char[] HeaderKeys = {'\r', '\n', ':'};
        public const short MinBuffer = 21; // Minimum size of the buffer "Content-Length: X\r\n\r\n"
        public static readonly byte[] ContentLength = "Content-Length".Select(x => (byte) x).ToArray();
        public static readonly int ContentLengthLength = 14;

        private readonly PipeReader _pipeReader;
        protected readonly IOutputHandler _outputHandler;
        private readonly IRequestProcessIdentifier _requestProcessIdentifier;
        private readonly IRequestRouter<IHandlerDescriptor> _requestRouter;
        private readonly IResponseRouter _responseRouter;
        private readonly ISerializer _serializer;
        private readonly ILogger<InputHandler> _logger;
        internal readonly ProcessScheduler _scheduler;
        private readonly Memory<byte> _headersBuffer;
        private readonly Memory<byte> _contentLengthBuffer;
        private readonly byte[] _contentLengthValueBuffer;
        private readonly Memory<byte> _contentLengthValueMemory;
        private readonly CancellationTokenSource _stopProcessing;
        private readonly CompositeDisposable _disposable;
        private readonly JsonReaderOptions _readerOptions;

        public InputHandler(
            PipeReader pipeReader,
            IOutputHandler outputHandler,
            IRequestProcessIdentifier requestProcessIdentifier,
            IRequestRouter<IHandlerDescriptor> requestRouter,
            IResponseRouter responseRouter,
            ILoggerFactory loggerFactory,
            ISerializer serializer,
            int? concurrency
        )
        {
            _pipeReader = pipeReader;
            _outputHandler = outputHandler;
            _requestProcessIdentifier = requestProcessIdentifier;
            _requestRouter = requestRouter;
            _responseRouter = responseRouter;
            _serializer = serializer;
            _logger = loggerFactory.CreateLogger<InputHandler>();
            _scheduler = new ProcessScheduler(loggerFactory, concurrency,
                new EventLoopScheduler(_ => new Thread(_) {IsBackground = true, Name = "InputHandler"}));
            _headersBuffer = new Memory<byte>(new byte[HeadersFinishedLength]);
            _contentLengthBuffer = new Memory<byte>(new byte[ContentLengthLength]);
            _contentLengthValueBuffer = new byte[20]; // Max string length of the long value
            _contentLengthValueMemory =
                new Memory<byte>(_contentLengthValueBuffer); // Max string length of the long value
            _stopProcessing = new CancellationTokenSource();
            _readerOptions = new JsonReaderOptions() {
                AllowTrailingCommas = false,
                CommentHandling = JsonCommentHandling.Disallow,
            };


            _disposable = new CompositeDisposable {
                Disposable.Create(() => _stopProcessing.Cancel()),
                _stopProcessing,
                Disposable.Create(() => _pipeReader?.Complete()),
                _scheduler,
            };
        }

        public void Start()
        {
            ProcessInputStream(_stopProcessing.Token).ContinueWith(x => {
                if (x.IsFaulted) _logger.LogCritical(x.Exception, "unhandled exception");
            });
        }

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

                var next = buffer.Slice(start.Value, buffer.GetPosition(4, start.Value));
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

        static bool IsEqual(in Span<byte> headers, in byte[] bytes)
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

        private bool TryParseBodyString(in long length, ref ReadOnlySequence<byte> buffer,
            out ReadOnlySequence<byte> line)
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

        bool TryParseContentLength(ref ReadOnlySequence<byte> buffer, out long length)
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

                    while (buffer.TryGet(ref position, out var memory, true) && !memory.Span.IsEmpty)
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
                else
                {
                    buffer = buffer.Slice(buffer.GetPosition(1, buffer.PositionOf((byte) '\n') ?? buffer.End));
                }
            } while (true);
        }

        internal async Task ProcessInputStream(CancellationToken cancellationToken)
        {
            // some time to attach a debugger
            // System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));

            var headersParsed = false;
            long length = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await _pipeReader.ReadAsync(cancellationToken);
                var buffer = result.Buffer;

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
                    }
                }

                _pipeReader.AdvanceTo(buffer.Start, buffer.End);

                // Stop reading if there's no more data coming.
                if (result.IsCompleted && buffer.GetPosition(0, buffer.Start).Equals(buffer.GetPosition(0, buffer.End)))
                {
                    break;
                }
            }
        }

        protected virtual void HandleRequest(in ReadOnlySequence<byte> request)
        {
            var count = 0;
            var reader = new Utf8JsonReader(request, _readerOptions);

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    continue;
                }

                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    continue;
                }

                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    HandleRequestObject(request, ref reader);
                }

                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    count++;
                }
            }

            void HandleRequestObject(in ReadOnlySequence<byte> requestData, ref Utf8JsonReader jsonReader)
            {
                if (jsonReader.TokenType != JsonTokenType.StartObject)
                {
                    throw new NotSupportedException("the reader must be processing an object");
                }

                object requestId = null;
                ReadOnlySequence<byte> data = default;
                string method = null;
                bool isResponse = false;
                bool isError = false;

                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonTokenType.PropertyName)
                    {
                        var propertyName = jsonReader.GetString();
                        jsonReader.Read();
                        switch (propertyName)
                        {
                            case "jsonrpc":
                                if (jsonReader.TokenType != JsonTokenType.String || jsonReader.GetString() != "2.0")
                                {
                                    _outputHandler.Send(new InvalidRequest(null, "Unexpected protocol"));
                                }

                                break;
                            case "method":
                                method = jsonReader.GetString();
                                break;
                            case "id":
                                requestId = jsonReader.TokenType switch {
                                    JsonTokenType.Number => jsonReader.GetInt64(),
                                    JsonTokenType.String => jsonReader.GetString(),
                                    _ => null,
                                };
                                break;
                            case "params":
                            case "result":
                            case "error":
                            {
                                isError = propertyName == "error";
                                isResponse = propertyName == "result" || isError;
                                if (propertyName == "params" && jsonReader.TokenType != JsonTokenType.StartArray &&
                                    jsonReader.TokenType != JsonTokenType.StartObject &&
                                    jsonReader.TokenType != JsonTokenType.Null)
                                {
                                    _outputHandler.Send(new InvalidRequest(requestId, "Invalid params"));
                                    continue;
                                }

                                var start = jsonReader.Position;
                                jsonReader.Skip();
                                var end = jsonReader.Position;
                                @data = requestData.Slice(start, end);
                            }
                                break;
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(method))
                {
                    _outputHandler.Send(new InvalidRequest(requestId, "Method not set"));
                }

                if (isResponse)
                {
                    if (requestId == null ||
                        !(requestId is long id) && !long.TryParse(requestId.ToString(), out id)) return;

                    var pendingResponse = _responseRouter.GetRequest(id);
                    if (pendingResponse == null)
                    {
                        return;
                    }

                    if (!isError)
                    {
                        if (EqualityComparer<ReadOnlySequence<byte>>.Default.Equals(data, default))
                        {
                            pendingResponse.SetResult(null);
                            return;
                        }

                        var innerReader = new Utf8JsonReader(@data, _readerOptions);
                        pendingResponse.SetResult(pendingResponse.IsVoid
                            ? null
                            : JsonSerializer.Deserialize(ref innerReader, pendingResponse.ResponseType, _serializer.Options));
                    }
                    else
                    {
                        if (EqualityComparer<ReadOnlySequence<byte>>.Default.Equals(data, default))
                        {
                            pendingResponse.SetException(new JsonRpcException(new ServerError(requestId, new JsonElement())));
                            return;
                        }

                        pendingResponse.SetException(new JsonRpcException(new ServerError(requestId, JsonDocument.Parse(data).RootElement)));
                    }
                }
                else
                {
                    var paramsType = _requestRouter.GetParamsType(method);
                    object @params = null;
                    if (!EqualityComparer<ReadOnlySequence<byte>>.Default.Equals(data, default))
                    {
                        var innerReader = new Utf8JsonReader(@data, _readerOptions);
                        @params = JsonSerializer.Deserialize(ref innerReader, paramsType, _serializer.Options);
                    }
                    if (requestId != null)
                    {
                        var request = new Request(requestId, method, @params);
                        var descriptor = _requestRouter.GetDescriptor(request);
                        if (descriptor is null) return;
                        HandleRequest(descriptor, request);
                    }
                    else
                    {
                        var notification = new Notification(method, @params);
                        var descriptor = _requestRouter.GetDescriptor(notification);
                        if (descriptor is null) return;

                        // We need to special case cancellation so that we can cancel any request that is currently in flight.
                        if (descriptor.Method == JsonRpcNames.CancelRequest && notification.Params is CancelParams cancelParams)
                        {
                            _requestRouter.CancelRequest(cancelParams.Id);
                            return;
                        }

                        HandleNotification(descriptor, notification);
                    }
                }
            }
        }

        protected virtual void HandleNotification(IHandlerDescriptor descriptor, Notification notification)
        {
            var type = _requestProcessIdentifier.Identify(descriptor);
            _scheduler.Add(
                type,
                notification.Method,
                Observable.FromAsync((ct) => _requestRouter.RouteNotification(descriptor, notification, ct))
            );
        }

        protected virtual void  HandleRequest(IHandlerDescriptor descriptor, Request request)
        {
            var type = _requestProcessIdentifier.Identify(descriptor);
            _requestRouter.StartRequest(request.Id);
            _scheduler.Add(
                type,
                request.Method,
                Observable.FromAsync(async (ct) =>
                    {
                        var result =
                            await _requestRouter.RouteRequest(descriptor, request, ct);
                        if (result.IsError && result.Error is RequestCancelled)
                        {
                            return;
                        }

                        _outputHandler.Send(result.Value);
                    }
                ));
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
