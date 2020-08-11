using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    public abstract class DebugAdapterRpcOptionsBase<T> : JsonRpcServerOptionsBase<T> where T : IJsonRpcHandlerRegistry<T>
    {
        public DebugAdapterRpcOptionsBase() => Services.AddLogging(builder => LoggingBuilderAction?.Invoke(builder));
        public ISerializer Serializer { get; set; } = new DapSerializer();
        public override IRequestProcessIdentifier RequestProcessIdentifier { get; set; } = new ParallelRequestProcessIdentifier();
        internal bool AddDefaultLoggingProvider { get; set; }
        internal Action<ILoggingBuilder> LoggingBuilderAction { get; set; } = _ => { };
        internal Action<IConfigurationBuilder> ConfigurationBuilderAction { get; set; } = _ => { };
    }
}
