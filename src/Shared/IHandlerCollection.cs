using System;
using System.Collections.Generic;
using System.Reflection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;

namespace OmniSharp.Extensions.LanguageServer.Shared
{
    internal interface IHandlerCollection : IEnumerable<ILspHandlerDescriptor>, IHandlersManager
    {
        LspHandlerDescriptorDisposable Add(params IJsonRpcHandler[] handlers);
        LspHandlerDescriptorDisposable Add(params Type[] handlerTypes);
        LspHandlerDescriptorDisposable Add(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options);
        LspHandlerDescriptorDisposable Add(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc, JsonRpcHandlerOptions options);
        LspHandlerDescriptorDisposable Add(string method, Type handlerType, JsonRpcHandlerOptions options);
        bool ContainsHandler(Type type);
        bool ContainsHandler(TypeInfo typeInfo);
    }
}
