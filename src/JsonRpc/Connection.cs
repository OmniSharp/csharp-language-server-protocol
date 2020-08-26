using System;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.JsonRpc
{
    public class Connection : IDisposable
    {
        private readonly InputHandler _inputHandler;
        public bool IsOpen { get; private set; }

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
            CreateResponseExceptionHandler? getException = null
        ) =>
            _inputHandler = new InputHandler(
                input,
                outputHandler,
                receiver,
                requestProcessIdentifier,
                requestRouter,
                responseRouter,
                loggerFactory,
                onUnhandledException,
                getException,
                requestTimeout,
                supportContentModified,
                concurrency > 1 ? (int?) concurrency : null
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
