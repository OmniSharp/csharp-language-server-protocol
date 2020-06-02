using System;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IJsonRpcServer : IResponseRouter, IJsonRpcHandlerInstance<IJsonRpcServerRegistry>, IDisposable
    {

    }
}
