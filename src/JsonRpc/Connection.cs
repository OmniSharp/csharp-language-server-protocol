using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.JsonRpc
{
    public class Connection : IDisposable
    {
        private readonly IInputHandler _inputHandler;
        private readonly IRequestRouter _requestRouter;

        public Connection(
            Stream input,
            IOutputHandler outputHandler,
            IReciever reciever,
            IRequestProcessIdentifier requestProcessIdentifier,
            IRequestRouter requestRouter,
            IResponseRouter responseRouter,
            ILoggerFactory loggerFactory)
        {
            _requestRouter = requestRouter;

            _inputHandler = new InputHandler(
                input,
                outputHandler,
                reciever,
                requestProcessIdentifier,
                requestRouter,
                responseRouter,
                loggerFactory
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
