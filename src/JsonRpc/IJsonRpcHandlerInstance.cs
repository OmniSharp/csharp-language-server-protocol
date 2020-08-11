using System;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IJsonRpcHandlerInstance<out TRegistry>
        where TRegistry : IJsonRpcHandlerRegistry<TRegistry>
    {
        public IDisposable Register(Action<TRegistry> registryAction);
    }
}
