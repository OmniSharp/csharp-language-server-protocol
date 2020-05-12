using System;
using System.IO;
using System.IO.Pipelines;
using Microsoft.Extensions.Logging;
using Nerdbank.Streams;

namespace OmniSharp.Extensions.JsonRpc
{
    public class Connection : IDisposable
    {
        private readonly InputHandler _inputHandler;

        public Connection(
            PipeReader input,
            IOutputHandler outputHandler,
            IReceiver receiver,
            IRequestProcessIdentifier requestProcessIdentifier,
            IRequestRouter<IHandlerDescriptor> requestRouter,
            IResponseRouter responseRouter,
            ILoggerFactory loggerFactory,
            ISerializer serializer,
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
                serializer,
                concurrency
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
