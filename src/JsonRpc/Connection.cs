using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.JsonRpc
{
    public class Connection : IDisposable
    {
        private readonly IInputHandler _inputHandler;
        private readonly IRequestRouter<IHandlerDescriptor> _requestRouter;

        public Connection(
            Stream input,
            IOutputHandler outputHandler,
            IReceiver receiver,
            IRequestProcessIdentifier requestProcessIdentifier,
            IRequestRouter<IHandlerDescriptor> requestRouter,
            IResponseRouter responseRouter,
            ILoggerFactory loggerFactory,
            ISerializer serializer)
        {
            _requestRouter = requestRouter;

            _inputHandler = new InputHandler(
                input,
                outputHandler,
                receiver,
                requestProcessIdentifier,
                requestRouter,
                responseRouter,
                loggerFactory,
                serializer
            );
        }

        public void Open()
        {
            // TODO: Throw if called twice?
            _inputHandler.Start();
        }

        public void Dispose()
        {
            _inputHandler?.Dispose();
        }
    }
}
