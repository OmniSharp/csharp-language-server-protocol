using System;
using System.Collections.Generic;
using System.Reflection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace OmniSharp.Extensions.LanguageServer.Server.Abstractions
{
    public interface IHandlerCollection : IEnumerable<ILspHandlerDescriptor>
    {
        LspHandlerDescriptorDisposable Add(params IJsonRpcHandler[] handlers);
        LspHandlerDescriptorDisposable Add(IServiceProvider serviceProvider, params Type[] handlerTypes);
        LspHandlerDescriptorDisposable Add(string method, IJsonRpcHandler handler);
        LspHandlerDescriptorDisposable Add(string method, IServiceProvider serviceProvider, Type handlerType);
        bool ContainsHandler(Type type);
        bool ContainsHandler(TypeInfo typeInfo);
        IEnumerable<ITextDocumentIdentifier> TextDocumentIdentifiers();
    }
}
