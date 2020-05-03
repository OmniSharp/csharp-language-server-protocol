using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Disposables;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc.Serialization;

namespace OmniSharp.Extensions.JsonRpc
{
    public class JsonRpcServerOptions : IJsonRpcHandlerRegistry
    {
        public JsonRpcServerOptions()
        {
        }

        public Stream Input { get; set; }
        public Stream Output { get; set; }
        public ILoggerFactory LoggerFactory { get; set; } = new LoggerFactory();
        public ISerializer Serializer { get; set; } = new JsonRpcSerializer();
        public IRequestProcessIdentifier RequestProcessIdentifier { get; set; } = new ParallelRequestProcessIdentifier();
        public IReceiver Receiver { get; set; } = new Receiver();
        public IServiceCollection Services { get; set; } = new ServiceCollection();
        internal List<IJsonRpcHandler> Handlers { get; set; } = new List<IJsonRpcHandler>();
        internal List<(string name, IJsonRpcHandler handler)> NamedHandlers { get; set; } = new List<(string name, IJsonRpcHandler handler)>();
        internal List<(string name, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)> NamedServiceHandlers { get; set; } = new List<(string name, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)>();
        internal List<Type> HandlerTypes { get; set; } = new List<Type>();
        internal List<Assembly> HandlerAssemblies { get; set; } = new List<Assembly>();
        internal int? Concurrency { get; set; }

        public IDisposable AddHandler(string method, IJsonRpcHandler handler)
        {
            NamedHandlers.Add((method, handler));
            return Disposable.Empty;
        }

        public IDisposable AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)
        {
            NamedServiceHandlers.Add((method, handlerFunc));
            return Disposable.Empty;
        }

        public IDisposable AddHandlers(params IJsonRpcHandler[] handlers)
        {
            Handlers.AddRange(handlers);
            return Disposable.Empty;
        }

        public IDisposable AddHandler<T>() where T : IJsonRpcHandler
        {
            HandlerTypes.Add(typeof(T));
            return Disposable.Empty;
        }
    }
}
