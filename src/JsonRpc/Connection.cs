using System;
using System.IO.Pipelines;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc.Server;

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
            IRequestRouter<IHandlerDescriptor> requestRouter,
            IResponseRouter responseRouter,
            ILoggerFactory loggerFactory,
            Func<ServerError, IHandlerDescriptor, Exception> getException,
            bool supportContentModified,
            int? concurrency)
        {
            _inputHandler = new InputHandler(
                input,
                outputHandler,
                receiver,
                requestProcessIdentifier,
                requestRouter,
                responseRouter,
                loggerFactory,
                getException,
                supportContentModified,
                concurrency
            );
        }

        public void Open()
        {
            // TODO: Throw if called twice?
            _inputHandler.Start();
            IsOpen = true;
        }

        public void Dispose()
        {
            _inputHandler?.Dispose();
            IsOpen = false;
        }
    }
}
