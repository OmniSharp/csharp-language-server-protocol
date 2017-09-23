using System;
using System.IO;

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
            IResponseRouter responseRouter)
        {
            _requestRouter = requestRouter;

            _inputHandler = new InputHandler(
                input,
                outputHandler,
                reciever,
                requestProcessIdentifier,
                requestRouter,
                responseRouter
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
