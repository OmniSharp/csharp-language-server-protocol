using System;
using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Shared
{
    static class HandlerCollectionExtensions
    {
        public static LspHandlerDescriptorDisposable Add(this IHandlerCollection collection, IEnumerable<IJsonRpcHandler> handlers)
        {
            return collection.Add(handlers.ToArray());
        }

        public static LspHandlerDescriptorDisposable Add(this IHandlerCollection collection, IServiceProvider serviceProvider, IEnumerable<Type> handlerTypes)
        {
            return collection.Add(serviceProvider, handlerTypes.ToArray());
        }

        public static LspHandlerDescriptorDisposable Add(this ILanguageServer collection, IEnumerable<IJsonRpcHandler> handlers)
        {
            return collection.Add(handlers.ToArray());
        }

        public static LspHandlerDescriptorDisposable Add(this ILanguageServer collection, IEnumerable<Type> handlerTypes)
        {
            return collection.Add(handlerTypes.ToArray());
        }

        public static LspHandlerDescriptorDisposable Add(this ILanguageClient collection, IEnumerable<IJsonRpcHandler> handlers)
        {
            return collection.Add(handlers.ToArray());
        }

        public static LspHandlerDescriptorDisposable Add(this ILanguageClient collection, IEnumerable<Type> handlerTypes)
        {
            return collection.Add(handlerTypes.ToArray());
        }
    }
}
