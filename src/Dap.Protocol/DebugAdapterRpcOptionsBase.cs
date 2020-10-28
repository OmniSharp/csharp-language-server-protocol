using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using ISerializer = OmniSharp.Extensions.JsonRpc.ISerializer;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    public abstract class DebugAdapterRpcOptionsBase<T> : JsonRpcServerOptionsBase<T> where T : IJsonRpcHandlerRegistry<T>
    {
        protected DebugAdapterRpcOptionsBase()
        {
            Services.AddLogging(builder => LoggingBuilderAction.Invoke(builder));
            WithAssemblies(typeof(DebugAdapterRpcOptionsBase<T>).Assembly);
            RequestProcessIdentifier = new ParallelRequestProcessIdentifier();
        }

        public ISerializer Serializer { get; set; } = new DapProtocolSerializer();
        internal bool AddDefaultLoggingProvider { get; set; }
        internal Action<ILoggingBuilder> LoggingBuilderAction { get; set; } = _ => { };
        internal Action<IConfigurationBuilder> ConfigurationBuilderAction { get; set; } = _ => { };
    }
}
