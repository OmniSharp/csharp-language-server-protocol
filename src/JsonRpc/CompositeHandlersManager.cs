using System;
using System.Collections.Generic;
using System.Reactive.Disposables;

namespace OmniSharp.Extensions.JsonRpc
{
    public class CompositeHandlersManager : IHandlersManager
    {
        private readonly IHandlersManager _parent;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public CompositeHandlersManager(IHandlersManager parent) => _parent = parent;

        public IEnumerable<IHandlerDescriptor> Descriptors => _parent.Descriptors;

        public IDisposable Add(IJsonRpcHandler handler, JsonRpcHandlerOptions options)
        {
            var result = _parent.Add(handler, options);
            _compositeDisposable.Add(result);
            return result;
        }

        public IDisposable Add(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options)
        {
            var result = _parent.Add(method, handler, options);
            _compositeDisposable.Add(result);
            return result;
        }

        public IDisposable Add(JsonRpcHandlerFactory factory, JsonRpcHandlerOptions options) => _parent.Add(factory, options);

        public IDisposable Add(string method, JsonRpcHandlerFactory factory, JsonRpcHandlerOptions options) => _parent.Add(method, factory, options);

        public IDisposable Add(Type handlerType, JsonRpcHandlerOptions options) => _parent.Add(handlerType, options);

        public IDisposable Add(string method, Type handlerType, JsonRpcHandlerOptions options) => _parent.Add(method, handlerType, options);

        public IDisposable AddLink(string sourceMethod, string destinationMethod) => _parent.AddLink(sourceMethod, destinationMethod);

        public CompositeDisposable GetDisposable() => _compositeDisposable;
    }
}
