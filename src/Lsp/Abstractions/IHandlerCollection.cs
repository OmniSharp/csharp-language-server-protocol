using System;
using System.Collections.Generic;
using JsonRpc;

namespace Lsp
{
    interface IHandlerCollection : IEnumerable<ILspHandlerDescriptor>
    {
        IDisposable Add(IJsonRpcHandler handler);

        IEnumerable<ILspHandlerDescriptor> Get(string method);
        IEnumerable<ILspHandlerDescriptor> Get(IJsonRpcHandler handler);
    }
}