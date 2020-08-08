using System;
using System.Diagnostics;

namespace OmniSharp.Extensions.JsonRpc
{
    [DebuggerDisplay("{Method}")]
    internal class LinkedHandler : IHandlerDescriptor, IDisposable
    {
        private readonly IHandlerDescriptor _descriptor;
        private readonly Action _disposeAction;

        public LinkedHandler(string method, IHandlerDescriptor descriptor, Action disposeAction)
        {
            _descriptor = descriptor;
            _disposeAction = disposeAction;
            Method = method;
        }
        public string Method { get; }
        public Type HandlerType => _descriptor.HandlerType;

        public Type ImplementationType => _descriptor.ImplementationType;

        public Type Params => _descriptor.Params;

        public Type Response => _descriptor.Response;

        public bool HasReturnType => _descriptor.HasReturnType;

        public bool IsDelegatingHandler => _descriptor.IsDelegatingHandler;

        public IJsonRpcHandler Handler => _descriptor.Handler;

        public RequestProcessType? RequestProcessType => _descriptor.RequestProcessType;

        public void Dispose()
        {
            _disposeAction();
        }
    }
}