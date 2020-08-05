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
        LspHandlerDescriptorDisposable Add(IJsonRpcHandler handler, JsonRpcHandlerOptions options);
        LspHandlerDescriptorDisposable Add(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options);
        LspHandlerDescriptorDisposable Add(params JsonRpcHandlerFactory[] handlerFactories);
        LspHandlerDescriptorDisposable Add(JsonRpcHandlerFactory handlerFactory, JsonRpcHandlerOptions options);
        LspHandlerDescriptorDisposable Add(string method, JsonRpcHandlerFactory handlerFactory, JsonRpcHandlerOptions options);
        LspHandlerDescriptorDisposable Add(params Type[] handlerTypes);
        LspHandlerDescriptorDisposable Add(Type handlerType, JsonRpcHandlerOptions options);
        LspHandlerDescriptorDisposable Add(string method, Type handlerType, JsonRpcHandlerOptions options);
        bool ContainsHandler(Type type);
        bool ContainsHandler(TypeInfo typeInfo);
    }
}
