using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Client.Dispatcher;
using OmniSharp.Extensions.LanguageServer.Client.Handlers;
using OmniSharp.Extensions.LanguageServer.Client.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using JsonRpcMessages = OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.LanguageServer.Client.Protocol
{
    /// <summary>
    ///     An asynchronous connection using the LSP protocol over <see cref="Stream"/>s.
    /// </summary>
    public sealed class LspConnection
        : IDisposable
    {
        /// <summary>
        ///     The buffer size to use when receiving headers.
        /// </summary>
        const short HeaderBufferSize = 300;

        /// <summary>
        ///     Minimum size of the buffer for receiving headers ("Content-Length: 1\r\n\r\n").
        /// </summary>
        const short MinimumHeaderLength = 21;

        /// <summary>
        ///     The length of time to wait for the outgoing message queue to drain.
        /// </summary>
        public static TimeSpan FlushTimeout { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        ///     The encoding used for message headers.
        /// </summary>
        public static Encoding HeaderEncoding = Encoding.ASCII;

        /// <summary>
        ///     The encoding used for message payloads.
        /// </summary>
        public static Encoding PayloadEncoding = Encoding.UTF8;

        /// <summary>
        ///     The queue of outgoing requests.
        /// </summary>
        readonly BlockingCollection<object> _outgoing = new BlockingCollection<object>(new ConcurrentQueue<object>());

        /// <summary>
        ///     The queue of incoming responses.
        /// </summary>
        readonly BlockingCollection<ServerMessage> _incoming = new BlockingCollection<ServerMessage>(new ConcurrentQueue<ServerMessage>());

        /// <summary>
        ///     <see cref="CancellationTokenSource"/>s representing cancellation of requests from the language server (keyed by request Id).
        /// </summary>
        readonly ConcurrentDictionary<string, CancellationTokenSource> _requestCancellations = new ConcurrentDictionary<string, CancellationTokenSource>();

        /// <summary>
        ///     <see cref="TaskCompletionSource{TResult}"/>s representing completion of responses from the language server (keyed by request Id).
        /// </summary>
        readonly ConcurrentDictionary<string, TaskCompletionSource<ServerMessage>> _responseCompletions = new ConcurrentDictionary<string, TaskCompletionSource<ServerMessage>>();

        /// <summary>
        ///     The input stream.
        /// </summary>
        readonly Stream _input;

        /// <summary>
        ///     The output stream.
        /// </summary>
        readonly Stream _output;

        /// <summary>
        ///     The next available request Id.
        /// </summary>
        int _nextRequestId = 0;

        /// <summary>
        ///     Has the connection been disposed?
        /// </summary>
        bool _disposed;

        /// <summary>
        ///     The cancellation source for the read and write loops.
        /// </summary>
        CancellationTokenSource _cancellationSource;

        /// <summary>
        ///     Cancellation for the read and write loops.
        /// </summary>
        CancellationToken _cancellation;

        /// <summary>
        ///     A <see cref="Task"/> representing the stopping of the connection's send, receive, and dispatch loops.
        /// </summary>
        Task _hasDisconnectedTask = Task.CompletedTask;

        /// <summary>
        ///     The <see cref="LspDispatcher"/> used to dispatch messages to handlers.
        /// </summary>
        LspDispatcher _dispatcher;

        /// <summary>
        ///     A <see cref="Task"/> representing the connection's receive loop.
        /// </summary>
        Task _sendLoop;

        /// <summary>
        ///     A <see cref="Task"/> representing the connection's send loop.
        /// </summary>
        Task _receiveLoop;

        /// <summary>
        ///     A <see cref="Task"/> representing the connection's dispatch loop.
        /// </summary>
        Task _dispatchLoop;

        private readonly Serializer _serializer;

        /// <summary>
        ///     Create a new <see cref="LspConnection"/>.
        /// </summary>
        /// <param name="loggerFactory">
        ///     The factory for loggers used by the connection and its components.
        /// </param>
        /// <param name="input">
        ///     The input stream.
        /// </param>
        /// <param name="output">
        ///     The output stream.
        /// </param>
        public LspConnection(ILoggerFactory loggerFactory, Stream input, Stream output)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (!input.CanRead)
                throw new ArgumentException("Input stream does not support reading.", nameof(input));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            if (!output.CanWrite)
                throw new ArgumentException("Output stream does not support reading.", nameof(output));

            Log = loggerFactory.CreateLogger<LspConnection>();
            _input = input;
            _output = output;
            // What does client version do? Do we have to negotaite this?
            _serializer = new Serializer(ClientVersion.Lsp3);
        }

        /// <summary>
        ///     Dispose of resources being used by the connection.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            try
            {
                Disconnect();

                _cancellationSource?.Dispose();
            }
            finally
            {
                _disposed = true;
            }
        }

        /// <summary>
        ///     The connection's logger.
        /// </summary>
        ILogger Log { get; }

        /// <summary>
        ///     Is the connection open?
        /// </summary>
        public bool IsOpen => _sendLoop != null || _receiveLoop != null || _dispatchLoop != null;

        /// <summary>
        ///     A task that completes when the connection is closed.
        /// </summary>
        public Task HasHasDisconnected => _hasDisconnectedTask;

        /// <summary>
        ///     Register a message handler.
        /// </summary>
        /// <param name="handler">
        ///     The message handler.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> representing the registration.
        /// </returns>
        public IDisposable RegisterHandler(IHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            LspDispatcher dispatcher = _dispatcher;
            if (dispatcher == null)
                throw new InvalidOperationException("The connection has not been opened.");

            return dispatcher.RegisterHandler(handler);
        }

        /// <summary>
        ///     Open the connection.
        /// </summary>
        /// <param name="dispatcher">
        ///     The <see cref="LspDispatcher"/> used to dispatch messages to handlers.
        /// </param>
        public void Connect(LspDispatcher dispatcher)
        {
            if (dispatcher == null)
                throw new ArgumentNullException(nameof(dispatcher));

            if (IsOpen)
                throw new InvalidOperationException("Connection is already open.");

            _cancellationSource = new CancellationTokenSource();
            _cancellation = _cancellationSource.Token;

            _dispatcher = dispatcher;
            _sendLoop = SendLoop();
            _receiveLoop = ReceiveLoop();
            _dispatchLoop = DispatchLoop();

            _hasDisconnectedTask = Task.WhenAll(_sendLoop, _receiveLoop, _dispatchLoop);
        }

        /// <summary>
        ///     Close the connection.
        /// </summary>
        /// <param name="flushOutgoing">
        ///     If <c>true</c>, stop receiving and block until all outgoing messages have been sent.
        /// </param>
        public void Disconnect(bool flushOutgoing = false)
        {
            if (flushOutgoing)
            {
                // Stop receiving.
                _incoming.CompleteAdding();

                // Wait for the outgoing message queue to drain.
                int remainingMessageCount = 0;
                DateTime then = DateTime.Now;
                while (DateTime.Now - then < FlushTimeout)
                {
                    remainingMessageCount = _outgoing.Count;
                    if (remainingMessageCount == 0)
                        break;

                    Thread.Sleep(
                        TimeSpan.FromMilliseconds(200)
                    );
                }

                if (remainingMessageCount > 0)
                    Log.LogWarning("Failed to flush outgoing messages ({RemainingMessageCount} messages remaining).", _outgoing.Count);
            }

            // Cancel all outstanding requests.
            // This should not be necessary because request cancellation tokens should be linked to _cancellationSource, but better to be sure we won't leave a caller hanging.
            foreach (TaskCompletionSource<ServerMessage> responseCompletion in _responseCompletions.Values)
            {
                responseCompletion.TrySetException(
                    new OperationCanceledException("The request was canceled because the underlying connection was closed.")
                );
            }

            _cancellationSource?.Cancel();
            _sendLoop = null;
            _receiveLoop = null;
            _dispatchLoop = null;
            _dispatcher = null;
        }

        /// <summary>
        ///     Send an empty notification to the language server.
        /// </summary>
        /// <param name="method">
        ///     The notification method name.
        /// </param>
        public void SendEmptyNotification(string method)
        {
            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (!IsOpen)
                throw new LspException("Not connected to the language server.");

            _outgoing.TryAdd(new ClientMessage
            {
                // No Id means it's a notification.
                Method = method
            });
        }

        /// <summary>
        ///     Send a notification message to the language server.
        /// </summary>
        /// <param name="method">
        ///     The notification method name.
        /// </param>
        /// <param name="notification">
        ///     The notification message.
        /// </param>
        public void SendNotification(string method, object notification)
        {
            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (notification == null)
                throw new ArgumentNullException(nameof(notification));

            if (!IsOpen)
                throw new LspException("Not connected to the language server.");

            _outgoing.TryAdd(new ClientMessage
            {
                // No Id means it's a notification.
                Method = method,
                Params = JObject.FromObject(notification, _serializer.JsonSerializer)
            });
        }

        /// <summary>
        ///     Send a request to the language server.
        /// </summary>
        /// <param name="method">
        ///     The request method name.
        /// </param>
        /// <param name="request">
        ///     The request message.
        /// </param>
        /// <param name="cancellationToken">
        ///     An optional cancellation token that can be used to cancel the request.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the request.
        /// </returns>
        public async Task SendRequest(string method, object request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (!IsOpen)
                throw new LspException("Not connected to the language server.");

            string requestId = Interlocked.Increment(ref _nextRequestId).ToString();

            TaskCompletionSource<ServerMessage> responseCompletion = new TaskCompletionSource<ServerMessage>(state: requestId);
            cancellationToken.Register(() =>
            {
                responseCompletion.TrySetException(
                    new OperationCanceledException("The request was canceled via the supplied cancellation token.", cancellationToken)
                );

                // Send notification telling server to cancel the request, if possible.
                if (!_outgoing.IsAddingCompleted)
                {
                    _outgoing.TryAdd(new ClientMessage
                    {
                        Method = GeneralNames.CancelRequest,
                        Params = new JObject(
                            new JProperty("id", requestId)
                        )
                    });
                }
            });

            _responseCompletions.TryAdd(requestId, responseCompletion);

            _outgoing.TryAdd(new ClientMessage
            {
                Id = requestId,
                Method = method,
                Params = request != null ? JObject.FromObject(request, _serializer.JsonSerializer) : null
            });

            await responseCompletion.Task;
        }

        /// <summary>
        ///     Send a request to the language server.
        /// </summary>
        /// <typeparam name="TResponse">
        ///     The response message type.
        /// </typeparam>
        /// <param name="method">
        ///     The request method name.
        /// </param>
        /// <param name="request">
        ///     The request message.
        /// </param>
        /// <param name="cancellationToken">
        ///     An optional cancellation token that can be used to cancel the request.
        /// </param>
        /// <returns>
        ///     A <see cref="Task{TResult}"/> representing the response.
        /// </returns>
        public async Task<TResponse> SendRequest<TResponse>(string method, object request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (!IsOpen)
                throw new LspException("Not connected to the language server.");

            string requestId = Interlocked.Increment(ref _nextRequestId).ToString();

            TaskCompletionSource<ServerMessage> responseCompletion = new TaskCompletionSource<ServerMessage>(state: requestId);
            cancellationToken.Register(() =>
            {
                responseCompletion.TrySetException(
                    new OperationCanceledException("The request was canceled via the supplied cancellation token.", cancellationToken)
                );

                // Send notification telling server to cancel the request, if possible.
                if (!_outgoing.IsAddingCompleted)
                {
                    _outgoing.TryAdd(new ClientMessage
                    {
                        Method = GeneralNames.CancelRequest,
                        Params = new JObject(
                            new JProperty("id", requestId)
                        )
                    });
                }
            });

            _responseCompletions.TryAdd(requestId, responseCompletion);

            _outgoing.TryAdd(new ClientMessage
            {
                Id = requestId,
                Method = method,
                Params = request != null ? JObject.FromObject(request, _serializer.JsonSerializer) : null
            });

            ServerMessage response = await responseCompletion.Task;

            if (response.Result != null)
                return response.Result.ToObject<TResponse>(_serializer.JsonSerializer);
            else
                return default(TResponse);
        }

        /// <summary>
        ///     The connection's message-send loop.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task"/> representing the loop's activity.
        /// </returns>
        async Task SendLoop()
        {
            await Task.Yield();

            Log.LogInformation("Send loop started.");

            try
            {
                while (_outgoing.TryTake(out object outgoing, -1, _cancellation))
                {
                    try
                    {
                        if (outgoing is ClientMessage message)
                        {
                            if (message.Id != null)
                                Log.LogDebug("Sending outgoing {RequestMethod} request {RequestId}...", message.Method, message.Id);
                            else
                                Log.LogDebug("Sending outgoing {RequestMethod} notification...", message.Method);

                            await SendMessage(message);

                            if (message.Id != null)
                                Log.LogDebug("Sent outgoing {RequestMethod} request {RequestId}.", message.Method, message.Id);
                            else
                                Log.LogDebug("Sent outgoing {RequestMethod} notification.", message.Method);
                        }
                        else if (outgoing is RpcError errorResponse)
                        {
                            Log.LogDebug("Sending outgoing error response {RequestId} ({ErrorMessage})...", errorResponse.Id, errorResponse.Error?.Message);

                            await SendMessage(errorResponse);

                            Log.LogDebug("Sent outgoing error response {RequestId}.", errorResponse.Id);
                        }
                        else
                            Log.LogError("Unexpected outgoing message type '{0}'.", outgoing.GetType().AssemblyQualifiedName);
                    }
                    catch (Exception sendError)
                    {
                        Log.LogError(sendError, "Unexpected error sending outgoing message {@Message}.", outgoing);
                    }
                }
            }
            catch (OperationCanceledException operationCanceled)
            {
                // Like tears in rain
                if (operationCanceled.CancellationToken != _cancellation)
                    throw; // time to die
            }
            finally
            {
                Log.LogInformation("Send loop terminated.");
            }
        }

        /// <summary>
        ///     The connection's message-receive loop.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task"/> representing the loop's activity.
        /// </returns>
        async Task ReceiveLoop()
        {
            await Task.Yield();

            Log.LogInformation("Receive loop started.");

            try
            {
                while (!_cancellation.IsCancellationRequested && !_incoming.IsAddingCompleted)
                {
                    ServerMessage message = await ReceiveMessage();
                    if (message == null)
                        continue;

                    _cancellation.ThrowIfCancellationRequested();

                    try
                    {
                        if (message.Id != null)
                        {
                            // Request or response.
                            if (message.Params != null)
                            {
                                // Request.
                                Log.LogDebug("Received {RequestMethod} request {RequestId} from language server: {RequestParameters}",
                                    message.Method,
                                    message.Id,
                                    message.Params?.ToString(Formatting.None)
                                );

                                // Publish.
                                if (!_incoming.IsAddingCompleted)
                                    _incoming.TryAdd(message);
                            }
                            else
                            {
                                // Response.
                                string requestId = message.Id.ToString();
                                TaskCompletionSource<ServerMessage> completion;
                                if (_responseCompletions.TryGetValue(requestId, out completion))
                                {
                                    if (message.Error != null)
                                    {
                                        Log.LogDebug("Received error response {RequestId} from language server: {@ErrorMessage}",
                                            requestId,
                                            message.Error
                                        );

                                        Log.LogDebug("Faulting request {RequestId}.", requestId);

                                        completion.TrySetException(
                                            CreateLspException(message)
                                        );
                                    }
                                    else
                                    {
                                        Log.LogDebug("Received response {RequestId} from language server: {ResponseResult}",
                                            requestId,
                                            message.Result?.ToString(Formatting.None)
                                        );

                                        Log.LogDebug("Completing request {RequestId}.", requestId);

                                        completion.TrySetResult(message);
                                    }
                                }
                                else
                                {
                                    Log.LogDebug("Received unexpected response {RequestId} from language server: {ResponseResult}",
                                        requestId,
                                        message.Result?.ToString(Formatting.None)
                                    );
                                }
                            }
                        }
                        else
                        {
                            // Notification.
                            Log.LogDebug("Received {NotificationMethod} notification from language server: {NotificationParameters}",
                                message.Method,
                                message.Params?.ToString(Formatting.None)
                            );

                            // Publish.
                            if (!_incoming.IsAddingCompleted)
                                _incoming.TryAdd(message);
                        }
                    }
                    catch (Exception dispatchError)
                    {
                        Log.LogError(dispatchError, "Unexpected error processing incoming message {@Message}.", message);
                    }
                }
            }
            catch (OperationCanceledException operationCanceled)
            {
                // Like tears in rain
                if (operationCanceled.CancellationToken != _cancellation)
                    throw; // time to die
            }
            finally
            {
                Log.LogInformation("Receive loop terminated.");
            }
        }

        /// <summary>
        ///     Send a message to the language server.
        /// </summary>
        /// <typeparam name="TMessage">
        ///     The type of message to send.
        /// </typeparam>
        /// <param name="message">
        ///     The message to send.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the operation.
        /// </returns>
        async Task SendMessage<TMessage>(TMessage message)
            where TMessage : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            string payload = JsonConvert.SerializeObject(message, _serializer.Settings);
            byte[] payloadBuffer = PayloadEncoding.GetBytes(payload);

            byte[] headerBuffer = HeaderEncoding.GetBytes(
                $"Content-Length: {payloadBuffer.Length}\r\n\r\n"
            );

            Log.LogDebug("Sending outgoing header ({HeaderSize} bytes)...", headerBuffer.Length);
            await _output.WriteAsync(headerBuffer, 0, headerBuffer.Length, _cancellation);
            Log.LogDebug("Sent outgoing header ({HeaderSize} bytes).", headerBuffer.Length);

            Log.LogDebug("Sending outgoing payload ({PayloadSize} bytes)...", payloadBuffer.Length);
            await _output.WriteAsync(payloadBuffer, 0, payloadBuffer.Length, _cancellation);
            Log.LogDebug("Sent outgoing payload ({PayloadSize} bytes).", payloadBuffer.Length);

            Log.LogDebug("Flushing output stream...");
            await _output.FlushAsync(_cancellation);
            Log.LogDebug("Flushed output stream.");
        }

        /// <summary>
        ///     Receive a message from the language server.
        /// </summary>
        /// <returns>
        ///     A <see cref="ServerMessage"/> representing the message,
        /// </returns>
        async Task<ServerMessage> ReceiveMessage()
        {
            Log.LogDebug("Reading response headers...");

            byte[] headerBuffer = new byte[HeaderBufferSize];
            int bytesRead = await _input.ReadAsync(headerBuffer, 0, MinimumHeaderLength, _cancellation);

            Log.LogDebug("Read {ByteCount} bytes from input stream.", bytesRead);

            if (bytesRead == 0)
                return null; // Stream closed.

            const byte CR = (byte)'\r';
            const byte LF = (byte)'\n';

            while (bytesRead < MinimumHeaderLength ||
                   headerBuffer[bytesRead - 4] != CR || headerBuffer[bytesRead - 3] != LF ||
                   headerBuffer[bytesRead - 2] != CR || headerBuffer[bytesRead - 1] != LF)
            {
                Log.LogDebug("Reading additional data from input stream...");

                // Read single bytes until we've got a valid end-of-header sequence.
                var additionalBytesRead = await _input.ReadAsync(headerBuffer, bytesRead, 1, _cancellation);
                if (additionalBytesRead == 0)
                    return null; // no more _input, mitigates endless loop here.

                Log.LogDebug("Read {ByteCount} bytes of additional data from input stream.", additionalBytesRead);

                bytesRead += additionalBytesRead;
            }

            string headers = HeaderEncoding.GetString(headerBuffer, 0, bytesRead);
            Log.LogDebug("Got raw headers: {Headers}", headers);

            if (string.IsNullOrWhiteSpace(headers))
                return null; // Stream closed.

            Log.LogDebug("Read response headers {Headers}.", headers);

            Dictionary<string, string> parsedHeaders = ParseHeaders(headers);

            string contentLengthHeader;
            if (!parsedHeaders.TryGetValue("Content-Length", out contentLengthHeader))
            {
                Log.LogDebug("Invalid request headers (missing 'Content-Length' header).");

                return null;
            }

            int contentLength = Int32.Parse(contentLengthHeader);

            Log.LogDebug("Reading response body ({ExpectedByteCount} bytes expected).", contentLength);

            var requestBuffer = new byte[contentLength];
            var received = 0;
            while (received < contentLength)
            {
                Log.LogDebug("Reading segment of incoming request body ({ReceivedByteCount} of {TotalByteCount} bytes so far)...", received, contentLength);

                var payloadBytesRead = await _input.ReadAsync(requestBuffer, received, requestBuffer.Length - received, _cancellation);
                if (payloadBytesRead == 0)
                {
                    Log.LogWarning("Bailing out of reading payload (no_more_input after {ByteCount} bytes)...", received);

                    return null;
                }
                received += payloadBytesRead;

                Log.LogDebug("Read segment of incoming request body ({ReceivedByteCount} of {TotalByteCount} bytes so far).", received, contentLength);
            }

            Log.LogDebug("Received entire payload ({ReceivedByteCount} bytes).", received);

            string responseBody = PayloadEncoding.GetString(requestBuffer);
            ServerMessage message = JsonConvert.DeserializeObject<ServerMessage>(responseBody, _serializer.Settings);

            Log.LogDebug("Read response body {ResponseBody}.", responseBody);

            return message;
        }

        /// <summary>
        ///     Parse request headers.
        /// </summary>
        /// <param name="rawHeaders">
        /// </param>
        /// <returns>
        ///     A <see cref="Dictionary{TKey, TValue}"/> containing the header names and values.
        /// </returns>
        private Dictionary<string, string> ParseHeaders(string rawHeaders)
        {
            if (rawHeaders == null)
                throw new ArgumentNullException(nameof(rawHeaders));

            Dictionary<string, string> headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); // Header names are case-insensitive.
            string[] rawHeaderEntries = rawHeaders.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string rawHeaderEntry in rawHeaderEntries)
            {
                string[] nameAndValue = rawHeaderEntry.Split(new char[] { ':' }, count: 2);
                if (nameAndValue.Length != 2)
                    continue;

                headers[nameAndValue[0].Trim()] = nameAndValue[1].Trim();
            }

            return headers;
        }

        /// <summary>
        ///     The connection's message-dispatch loop.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task"/> representing the loop's activity.
        /// </returns>
        async Task DispatchLoop()
        {
            await Task.Yield();

            Log.LogInformation("Dispatch loop started.");

            try
            {
                while (_incoming.TryTake(out ServerMessage message, -1, _cancellation))
                {
                    if (message.Id != null)
                    {
                        // Request.
                        if (message.Method == GeneralNames.CancelRequest)
                            CancelRequest(message);
                        else
                            DispatchRequest(message);
                    }
                    else
                    {
                        // Notification.
                        DispatchNotification(message);
                    }
                }
            }
            catch (OperationCanceledException operationCanceled)
            {
                // Like tears in rain
                if (operationCanceled.CancellationToken != _cancellation)
                    throw; // time to die
            }
            finally
            {
                Log.LogInformation("Dispatch loop terminated.");
            }
        }

        /// <summary>
        ///     Dispatch a request.
        /// </summary>
        /// <param name="requestMessage">
        ///     The request message.
        /// </param>
        private void DispatchRequest(ServerMessage requestMessage)
        {
            if (requestMessage == null)
                throw new ArgumentNullException(nameof(requestMessage));

            string requestId = requestMessage.Id.ToString();
            Log.LogDebug("Dispatching incoming {RequestMethod} request {RequestId}...", requestMessage.Method, requestId);

            CancellationTokenSource requestCancellation = CancellationTokenSource.CreateLinkedTokenSource(_cancellation);
            _requestCancellations.TryAdd(requestId, requestCancellation);

            Task<object> handlerTask = _dispatcher.TryHandleRequest(requestMessage.Method, requestMessage.Params, requestCancellation.Token);
            if (handlerTask == null)
            {
                Log.LogWarning("Unable to dispatch incoming {RequestMethod} request {RequestId} (no handler registered).", requestMessage.Method, requestId);

                _outgoing.TryAdd(
                    new JsonRpcMessages.MethodNotFound(requestMessage.Id, requestMessage.Method)
                );

                return;
            }

#pragma warning disable CS4014 // Continuation does the work we need; no need to await it as this would tie up the dispatch loop.
            handlerTask.ContinueWith(_ =>
            {
                if (handlerTask.IsCanceled)
                    Log.LogDebug("{RequestMethod} request {RequestId} canceled.", requestMessage.Method, requestId);
                else if (handlerTask.IsFaulted)
                {
                    Exception handlerError = handlerTask.Exception.Flatten().InnerExceptions[0];

                    Log.LogError(handlerError, "{RequestMethod} request {RequestId} failed (unexpected error raised by handler).", requestMessage.Method, requestId);

                    _outgoing.TryAdd(new RpcError(requestId,
                        new JsonRpcMessages.ErrorMessage(
                            code: 500,
                            message: "Error processing request: " + handlerError.Message,
                            data: handlerError.ToString()
                        )
                    ));
                }
                else if (handlerTask.IsCompleted)
                {
                    Log.LogDebug("{RequestMethod} request {RequestId} complete (Result = {@Result}).", requestMessage.Method, requestId, handlerTask.Result);

                    _outgoing.TryAdd(new ClientMessage
                    {
                        Id = requestMessage.Id,
                        Method = requestMessage.Method,
                        Result = handlerTask.Result != null ? JObject.FromObject(handlerTask.Result, _serializer.JsonSerializer) : null
                    });
                }

                _requestCancellations.TryRemove(requestId, out CancellationTokenSource cancellation);
                cancellation.Dispose();
            });
#pragma warning restore CS4014 // Continuation does the work we need; no need to await it as this would tie up the dispatch loop.

            Log.LogDebug("Dispatched incoming {RequestMethod} request {RequestId}.", requestMessage.Method, requestMessage.Id);
        }

        /// <summary>
        ///     Cancel a request.
        /// </summary>
        /// <param name="requestMessage">
        ///     The request message.
        /// </param>
        void CancelRequest(ServerMessage requestMessage)
        {
            if (requestMessage == null)
                throw new ArgumentNullException(nameof(requestMessage));

            string cancelRequestId = requestMessage.Params?.Value<object>("id")?.ToString();
            if (cancelRequestId != null)
            {
                if (_requestCancellations.TryRemove(cancelRequestId, out CancellationTokenSource requestCancellation))
                {
                    Log.LogDebug("Cancel request {RequestId}", requestMessage.Id);
                    requestCancellation.Cancel();
                    requestCancellation.Dispose();
                }
                else
                    Log.LogDebug("Received cancellation message for non-existent (or already-completed) request ");
            }
            else
            {
                Log.LogWarning("Received invalid request cancellation message {MessageId} (missing 'id' parameter).", requestMessage.Id);

                _outgoing.TryAdd(
                    new JsonRpcMessages.InvalidParams(requestMessage.Id)
                );
            }
        }

        /// <summary>
        ///     Dispatch a notification.
        /// </summary>
        /// <param name="notificationMessage">
        ///     The notification message.
        /// </param>
        void DispatchNotification(ServerMessage notificationMessage)
        {
            if (notificationMessage == null)
                throw new ArgumentNullException(nameof(notificationMessage));

            Log.LogDebug("Dispatching incoming {NotificationMethod} notification...", notificationMessage.Method);

            Task<bool> handlerTask;
            if (notificationMessage.Params != null)
                handlerTask = _dispatcher.TryHandleNotification(notificationMessage.Method, notificationMessage.Params);
            else
                handlerTask = _dispatcher.TryHandleEmptyNotification(notificationMessage.Method);

#pragma warning disable CS4014 // Continuation does the work we need; no need to await it as this would tie up the dispatch loop.
            handlerTask.ContinueWith(completedHandler =>
            {
                if (handlerTask.IsCanceled)
                    Log.LogDebug("{NotificationMethod} notification canceled.", notificationMessage.Method);
                else if (handlerTask.IsFaulted)
                {
                    Exception handlerError = handlerTask.Exception.Flatten().InnerExceptions[0];

                    Log.LogError(handlerError, "Failed to dispatch {NotificationMethod} notification (unexpected error raised by handler).", notificationMessage.Method);
                }
                else if (handlerTask.IsCompleted)
                {
                    Log.LogDebug("{NotificationMethod} notification complete.", notificationMessage.Method);

                    if (completedHandler.Result)
                        Log.LogDebug("Dispatched incoming {NotificationMethod} notification.", notificationMessage.Method);
                    else
                        Log.LogDebug("Ignored incoming {NotificationMethod} notification (no handler registered).", notificationMessage.Method);
                }
            });
#pragma warning restore CS4014 // Continuation does the work we need; no need to await it as this would tie up the dispatch loop.
        }

        /// <summary>
        ///     Create an <see cref="LspException"/> to represent the specified message.
        /// </summary>
        /// <param name="message">
        ///     The <see cref="ServerMessage"/> (<see cref="ServerMessage.Error"/> must be populated).
        /// </param>
        /// <returns>
        ///     The new <see cref="LspException"/>.
        /// </returns>
        static LspException CreateLspException(ServerMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            Trace.Assert(message.Error != null, "message.Error != null");

            string requestId = message.Id?.ToString();

            switch (message.Error.Code)
            {
                case LspErrorCodes.InvalidRequest:
                {
                    return new LspInvalidRequestException(requestId);
                }
                case LspErrorCodes.InvalidParameters:
                {
                    return new LspInvalidParametersException(requestId);
                }
                case LspErrorCodes.InternalError:
                {
                    return new LspInternalErrorException(requestId);
                }
                case LspErrorCodes.MethodNotSupported:
                {
                    return new LspMethodNotSupportedException(requestId, message.Method);
                }
                case LspErrorCodes.RequestCancelled:
                {
                    return new LspRequestCancelledException(requestId);
                }
                default:
                {
                    string exceptionMessage = $"Error processing request '{message.Id}' ({message.Error.Code}): {message.Error.Message}";

                    return new LspRequestException(exceptionMessage, requestId, message.Error.Code);
                }
            }
        }
    }
}
