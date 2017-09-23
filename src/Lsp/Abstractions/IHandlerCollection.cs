using System;
using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Abstractions
{
    interface IHandlerCollection : IEnumerable<ILspHandlerDescriptor>
    {
        IDisposable Add(IJsonRpcHandler handler);

        IEnumerable<ILspHandlerDescriptor> Get(string method);
        IEnumerable<ILspHandlerDescriptor> Get(IJsonRpcHandler handler);
    }
}