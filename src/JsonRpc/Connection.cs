using System.IO.Pipelines;
using System.Reactive.Concurrency;
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.JsonRpc
{
    public class Connection : IDisposable
    {
        private readonly InputHandler _inputHandler;
        public bool IsOpen { get; private set; }

        [Obsolete("Use the other constructor that takes a request invoker")]
        public Connection(
            PipeReader input,
            IOutputHandler outputHandler,
            IReceiver receiver,
            IRequestProcessIdentifier requestProcessIdentifier,
            IRequestRouter<IHandlerDescriptor?> requestRouter,
            IResponseRouter responseRouter,
            ILoggerFactory loggerFactory,
            OnUnhandledExceptionHandler onUnhandledException,
            TimeSpan requestTimeout,
            bool supportContentModified,
            int concurrency,
            IScheduler scheduler,
            CreateResponseExceptionHandler? getException = null
        ) : this(
            input,
            outputHandler,
            receiver,
            requestRouter,
            responseRouter,
            new DefaultRequestInvoker(
                requestRouter,
                outputHandler,
                requestProcessIdentifier,
                new RequestInvokerOptions(
                    requestTimeout,
                    supportContentModified,
                    concurrency),
                loggerFactory,
                scheduler),
            loggerFactory,
            onUnhandledException,
            getException)
        {
        }

        public Connection(
            PipeReader input,
            IOutputHandler outputHandler,
            IReceiver receiver,
            IRequestRouter<IHandlerDescriptor?> requestRouter,
            IResponseRouter responseRouter,
            RequestInvoker requestInvoker,
            ILoggerFactory loggerFactory,
            OnUnhandledExceptionHandler onUnhandledException,
            CreateResponseExceptionHandler? getException = null
        ) =>
            _inputHandler = new InputHandler(
                input,
                outputHandler,
                receiver,
                requestRouter,
                responseRouter,
                requestInvoker,
                loggerFactory,
                onUnhandledException,
                getException
            );

        public void Open()
        {
            // TODO: Throw if called twice?
            _inputHandler.Start();
            IsOpen = true;
        }

        public Task StopAsync() => _inputHandler.StopAsync();

        public void Dispose()
        {
            _inputHandler.Dispose();
            IsOpen = false;
        }
    }
}
