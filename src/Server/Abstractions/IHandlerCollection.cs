using System;
using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Server.Abstractions
{
    public interface IHandlerCollection : IEnumerable<ILspHandlerDescriptor>
    {
        IDisposable Add(string method, IJsonRpcHandler handler);
        IDisposable Add(params IJsonRpcHandler[] handlers);
        IDisposable Add(IEnumerable<IJsonRpcHandler> handlers);
    }
}
