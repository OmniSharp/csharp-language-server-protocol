using System;
using System.Collections.Generic;
using JsonRpc;

namespace Lsp
{
    interface IHandlerCollection : IEnumerable<ILspHandlerInstance>
    {
        IDisposable Add(IJsonRpcHandler handler);
        void Remove(IJsonRpcHandler handler);
        ILspHandlerInstance Get(string method);
    }
}