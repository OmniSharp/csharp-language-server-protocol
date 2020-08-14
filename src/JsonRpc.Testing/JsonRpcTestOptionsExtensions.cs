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

        public static JsonRpcTestOptions WithWaitTime(this JsonRpcTestOptions options, TimeSpan waitTime)
        {
            options.WaitTime = waitTime;
            return options;
        }

        public static JsonRpcTestOptions WithTimeout(this JsonRpcTestOptions options, TimeSpan timeout)
        {
            options.Timeout = timeout;
            return options;
        }

        public static JsonRpcTestOptions WithCancellationTimeout(this JsonRpcTestOptions options, TimeSpan cancellationTimeout)
        {
            options.CancellationTimeout = cancellationTimeout;
            return options;
        }

        public static JsonRpcTestOptions WithDefaultPipeOptions(this JsonRpcTestOptions options, PipeOptions pipeOptions)
        {
            options.DefaultPipeOptions = pipeOptions;
            return options;
        }
    }
}
