using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc.Server;
using JsonRpc.Server.Messages;
using Newtonsoft.Json.Linq;

namespace JsonRpc
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