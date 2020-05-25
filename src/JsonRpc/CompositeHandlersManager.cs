using System;
using System.Reactive.Disposables;

namespace OmniSharp.Extensions.JsonRpc
{
    public class CompositeHandlersManager : IHandlersManager
    {
        private readonly IHandlersManager _parent;
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public CompositeHandlersManager(IHandlersManager parent)
        {
            _parent = parent;
        }

        public IDisposable Add(IJsonRpcHandler handler)
        {
            var result = _parent.Add(handler);
            _compositeDisposable.Add(result);
            return result;
        }

        public IDisposable Add(string method, IJsonRpcHandler handler)
        {
            var result = _parent.Add(method, handler);
            _compositeDisposable.Add(result);
            return result;
        }

        public CompositeDisposable GetDisposable() => _compositeDisposable;
    }
}
