using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.JsonRpc
{
    public class JsonRpcServerOptions
    {
        public JsonRpcServerOptions()
        {
        }

        public Stream Input { get; set; }
        public Stream Output { get; set; }
        public ILoggerFactory LoggerFactory { get; set; } = new LoggerFactory();
        public ISerializer Serializer { get; set; } = new Serializer();
        public IRequestProcessIdentifier RequestProcessIdentifier { get; set; } = new ParallelRequestProcessIdentifier();
        public IReciever Reciever { get; set; } = new Reciever();
        public IServiceCollection Services { get; set; } = new ServiceCollection();
        internal List<Type> HandlerTypes { get; set; } = new List<Type>();
        internal List<Assembly> HandlerAssemblies { get; set; } = new List<Assembly>();
    }
}
