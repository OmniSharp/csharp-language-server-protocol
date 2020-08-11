using System;
using System.IO.Pipelines;
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.JsonRpc.Testing
{
    public static class JsonRpcTestOptionsExtensions
    {
        public static JsonRpcTestOptions WithServerLoggerFactory(this JsonRpcTestOptions options, ILoggerFactory serverLoggerFactory)
        {
            options.ServerLoggerFactory = serverLoggerFactory;
            return options;
        }

        public static JsonRpcTestOptions WithClientLoggerFactory(this JsonRpcTestOptions options, ILoggerFactory clientLoggerFactory)
        {
            options.ClientLoggerFactory = clientLoggerFactory;
            return options;
        }

        public static JsonRpcTestOptions WithSettleTimeSpan(this JsonRpcTestOptions options, TimeSpan settleTimeSpan)
        {
            options.SettleTimeSpan = settleTimeSpan;
            return options;
        }

        public static JsonRpcTestOptions WithSettleTimeout(this JsonRpcTestOptions options, TimeSpan timeout)
        {
            options.SettleTimeout = timeout;
            return options;
        }

        public static JsonRpcTestOptions WithTestTimeout(this JsonRpcTestOptions options, TimeSpan testTimeout)
        {
            options.TestTimeout = testTimeout;
            return options;
        }

        public static JsonRpcTestOptions WithDefaultPipeOptions(this JsonRpcTestOptions options, PipeOptions pipeOptions)
        {
            options.DefaultPipeOptions = pipeOptions;
            return options;
        }
    }
}
