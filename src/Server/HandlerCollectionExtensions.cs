using System;
using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public static class HandlerCollectionExtensions
    {
        public static IDisposable Add(this IHandlerCollection collection, IEnumerable<IJsonRpcHandler> handlers)
        {
            return collection.Add(handlers.ToArray());
        }

        public static IDisposable Add(this IHandlerCollection collection, IServiceProvider serviceProvider, IEnumerable<Type> handlerTypes)
        {
            return collection.Add(serviceProvider, handlerTypes.ToArray());
        }

        public static IDisposable Add(this ILanguageServer collection, IEnumerable<IJsonRpcHandler> handlers)
        {
            return collection.Add(handlers.ToArray());
        }

        public static IDisposable Add(this ILanguageServer collection, IEnumerable<Type> handlerTypes)
        {
            return collection.Add(handlerTypes.ToArray());
        }
    }
}
