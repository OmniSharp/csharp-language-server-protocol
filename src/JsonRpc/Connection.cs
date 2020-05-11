using System;
using System.IO.Pipelines;
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.JsonRpc
{
    public class Connection : IDisposable
    {
        private readonly IInputHandler _handler;
        private readonly IInputHandler _inputHandler;

        public Connection(IInputHandler handler)
        {
            _handler = handler;
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
