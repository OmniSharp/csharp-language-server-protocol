using System;
using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;

namespace JsonRpc.Tests
{
    public class TestLanguageServerRegistry : JsonRpcCommonMethodsBase<IJsonRpcServerRegistry>, IJsonRpcServerRegistry
    {
        private List<IJsonRpcHandler> Handlers { get; } = new List<IJsonRpcHandler>();
        private List<(string name, IJsonRpcHandler handler)> NamedHandlers { get; } = new List<(string name, IJsonRpcHandler handler)>();

        private List<(string name, JsonRpcHandlerFactory handlerFunc)> NamedServiceHandlers { get; } =
            new List<(string name, JsonRpcHandlerFactory handlerFunc)>();

        public override IJsonRpcServerRegistry AddHandler(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions? options = null)
        {
            NamedHandlers.Add(( method, handler ));
            return this;
        }

        public override IJsonRpcServerRegistry AddHandler(IJsonRpcHandler handler, JsonRpcHandlerOptions? options = null) => throw new NotImplementedException();

        public override IJsonRpcServerRegistry AddHandler<T>(JsonRpcHandlerOptions options) => throw new NotImplementedException();

        public override IJsonRpcServerRegistry AddHandler<TTHandler>(string method, JsonRpcHandlerOptions? options = null) => throw new NotImplementedException();

        public override IJsonRpcServerRegistry AddHandler(Type type, JsonRpcHandlerOptions? options = null) => throw new NotImplementedException();

        public override IJsonRpcServerRegistry AddHandler(string method, Type type, JsonRpcHandlerOptions? options = null) => throw new NotImplementedException();
        public override IJsonRpcServerRegistry AddHandlerLink(string fromMethod, string toMethod) => throw new NotImplementedException();

        public override IJsonRpcServerRegistry AddHandler(string method, JsonRpcHandlerFactory handlerFunc, JsonRpcHandlerOptions? options = null)
        {
            NamedServiceHandlers.Add(( method, handlerFunc ));
            return this;
        }

        public override IJsonRpcServerRegistry AddHandlers(params IJsonRpcHandler[] handlers)
        {
            Handlers.AddRange(handlers);
            return this;
        }

        public override IJsonRpcServerRegistry AddHandler(JsonRpcHandlerFactory handlerFunc, JsonRpcHandlerOptions? options = null) => throw new NotImplementedException();

        internal void Populate(HandlerCollection collection, IServiceProvider serviceProvider, JsonRpcHandlerOptions? options = null)
        {
            collection.Add(Handlers.ToArray());
            foreach (var (name, handler) in NamedHandlers)
            {
                collection.Add(name, handler, options);
            }

            foreach (var (name, handlerFunc) in NamedServiceHandlers)
            {
                collection.Add(name, handlerFunc(serviceProvider), options);
            }
        }
    }
}
