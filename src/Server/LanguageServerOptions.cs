using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public class LanguageServerOptions
    {
        public LanguageServerOptions()
        {
        }

        public Stream Input { get; set; }
        public Stream Output { get; set; }
        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;
        public ILoggerFactory LoggerFactory { get; set; } = new LoggerFactory();
        public ISerializer Serializer { get; set; } = Protocol.Serialization.Serializer.Instance;
        public IRequestProcessIdentifier RequestProcessIdentifier { get; set; } = new RequestProcessIdentifier();
        public ILspReciever Reciever { get; set; } = new LspReciever();
        public IServiceCollection Services { get; set; } = new ServiceCollection();
        internal List<Type> HandlerTypes { get; set; } = new List<Type>();
        internal List<Assembly> HandlerAssemblies { get; set; } = new List<Assembly>();
        internal bool AddDefaultLoggingProvider { get; set; }
        internal readonly List<InitializeDelegate> InitializeDelegates = new List<InitializeDelegate>();
        internal readonly List<InitializedDelegate> InitializedDelegates = new List<InitializedDelegate>();
    }
}
