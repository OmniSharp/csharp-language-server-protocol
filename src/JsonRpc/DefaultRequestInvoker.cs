using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;
using Notification = OmniSharp.Extensions.JsonRpc.Server.Notification;

namespace OmniSharp.Extensions.JsonRpc
{
    public class DefaultRequestInvoker : RequestInvoker
    {
        private readonly IRequestRouter<IHandlerDescriptor?> _requestRouter;
        private readonly IOutputHandler _outputHandler;
        private readonly ProcessScheduler _processScheduler;
        private readonly IRequestProcessIdentifier _requestProcessIdentifier;
        private readonly RequestInvokerOptions _options;
        private readonly ILogger<DefaultRequestInvoker> _logger;

        public DefaultRequestInvoker(
            IRequestRouter<IHandlerDescriptor?> requestRouter,
            IOutputHandler outputHandler,
            IRequestProcessIdentifier requestProcessIdentifier,
            RequestInvokerOptions options,
            ILoggerFactory loggerFactory,
            IScheduler scheduler)
        {
            _requestRouter = requestRouter;
            _outputHandler = outputHandler;
            _requestProcessIdentifier = requestProcessIdentifier;
            _options = options;
            _processScheduler = new ProcessScheduler(loggerFactory, _options.SupportContentModified, _options.Concurrency, scheduler);
            _logger = loggerFactory.CreateLogger<DefaultRequestInvoker>();
        }

        public override RequestInvocationHandle InvokeRequest(IRequestDescriptor<IHandlerDescriptor?> descriptor, Request request)
        {
            if (descriptor.Default is null)
            {
                throw new ArgumentNullException(nameof(descriptor.Default));
            }

            var handle = new RequestInvocationHandle(request);
            var type = _requestProcessIdentifier.Identify(descriptor.Default);

            var schedulerDelegate = RouteRequest(descriptor, request, handle);
            _processScheduler.Add(type, $"{request.Method}:{request.Id}", schedulerDelegate);

            return handle;
        }

        public override void InvokeNotification(IRequestDescriptor<IHandlerDescriptor?> descriptor, Notification notification)
        {
            if (descriptor.Default is null)
            {
                throw new ArgumentNullException(nameof(descriptor.Default));
            }

            var type = _requestProcessIdentifier.Identify(descriptor.Default);
            var schedulerDelegate = RouteNotification(descriptor, notification);
            _processScheduler.Add(type, notification.Method, schedulerDelegate);
        }

        public override void Dispose()
        {
            _processScheduler.Dispose();
        }

        private SchedulerDelegate RouteRequest(
            IRequestDescriptor<IHandlerDescriptor?> descriptor,
            Request request,
            RequestInvocationHandle handle)
        {
            var cts = handle.CancellationTokenSource;
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
                                                            Observable.Timer(_options.RequestTimeout, scheduler).Select(
                                                                _ => new ErrorResponse(new RequestCancelled(request.Id, request.Method))
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
                                                                        var result = await _requestRouter.RouteRequest(
                                                                            descriptor, request, cts.Token
                                                                        ).ConfigureAwait(false);
                                                                        return result;
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
                                   return new CompositeDisposable(sub, handle);
                               }
                           )
                          .Select(
                               response => {
                                   _outputHandler.Send(response.Value);
                                   return Unit.Default;
                               }
                           );
        }

        private SchedulerDelegate RouteNotification(
            IRequestDescriptor<IHandlerDescriptor?> descriptors,
            Notification notification) =>
            (_, scheduler) =>
                // ITS A RACE!
                Observable.Amb(
                    Observable.Timer(_options.RequestTimeout, scheduler)
                              .Select(_ => Unit.Default)
                              .Do(
                                   _ => _logger.LogTrace("Notification was cancelled due to timeout")
                               ),
                    Observable.FromAsync(
                        async ct => {
                            using var timer = _logger.TimeDebug("Processing notification {Method}", notification.Method);
                            try
                            {
                                await _requestRouter.RouteNotification(descriptors, notification, ct).ConfigureAwait(false);
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
    }
}
