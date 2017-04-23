using System;
using System.Collections.Generic;
using JsonRpc;

namespace Lsp
{
    interface IHandlerCollection : IEnumerable<ILspHandlerInstance>
    {
        IDisposable Add(IJsonRpcHandler handler);

        IEnumerable<ILspHandlerInstance> Get(string method);
        IEnumerable<ILspHandlerInstance> Get(IJsonRpcHandler handler);
    }
}