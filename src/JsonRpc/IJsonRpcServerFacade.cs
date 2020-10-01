using System;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IJsonRpcServerFacade : IResponseRouter, IJsonRpcHandlerInstance<IJsonRpcServerRegistry>, IServiceProvider
    {
    }
}