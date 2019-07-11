using System;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IJsonRpcServer : IResponseRouter, IDisposable, IJsonRpcHandlerRegistry
    {

    }
}
