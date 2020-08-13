using System;
using System.IO.Pipelines;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace OmniSharp.Extensions.JsonRpc.Testing
{
    public sealed class JsonRpcTestOptions
    {
        public JsonRpcTestOptions()
        {
        }

        public JsonRpcTestOptions(ILoggerFactory loggerFactory) => ServerLoggerFactory = ClientLoggerFactory = loggerFactory;

        public JsonRpcTestOptions(ILoggerFactory clientLoggerFactory, ILoggerFactory serverLoggerFactory)
        {
            ClientLoggerFactory = clientLoggerFactory;
            ServerLoggerFactory = serverLoggerFactory;
        }

        public ILoggerFactory ClientLoggerFactory { get; internal set; } = NullLoggerFactory.Instance;
        public ILoggerFactory ServerLoggerFactory { get; internal set; } = NullLoggerFactory.Instance;
        public TimeSpan SettleTimeSpan { get; internal set; } = TimeSpan.FromMilliseconds(60);
        public TimeSpan SettleTimeout { get; internal set; } = TimeSpan.FromMilliseconds(300);
        public TimeSpan TestTimeout { get; internal set; } = TimeSpan.FromMinutes(5);
        public PipeOptions DefaultPipeOptions { get; internal set; } = new PipeOptions();
    }
}
