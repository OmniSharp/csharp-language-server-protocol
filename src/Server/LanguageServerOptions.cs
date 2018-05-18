using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        public ILoggerFactory LoggerFactory { get; set; }
        public ISerializer Serializer { get; set; }
        public IHandlerCollection Handlers { get; set; } = new HandlerCollection();
        public IRequestProcessIdentifier RequestProcessIdentifier { get; set; }
        public ILspReciever Reciever { get; set; } = new LspReciever();
        public IServiceCollection Services { get; set; } = new ServiceCollection();
        internal List<Type> HandlerTypes { get; set; } = new List<Type>();
        internal List<Assembly> HandlerAssemblies { get; set; } = new List<Assembly>();
        internal bool AddDefaultLoggingProvider { get; set; }
    }
}