using System.IO.Pipelines;
using System.Reactive.Concurrency;
using System.Reflection;
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

        public static JsonRpcTestOptions WithLoggerFactory(this JsonRpcTestOptions options, ILoggerFactory loggerFactory)
        {
            options.ClientLoggerFactory = options.ServerLoggerFactory = loggerFactory;
            return options;
        }

        public static JsonRpcTestOptions WithServerScheduler(this JsonRpcTestOptions options, IScheduler scheduler)
        {
            options.ServerScheduler = scheduler;
            return options;
        }

        public static JsonRpcTestOptions WithClientScheduler(this JsonRpcTestOptions options, IScheduler scheduler)
        {
            options.ClientScheduler = scheduler;
            return options;
        }

        public static JsonRpcTestOptions WithScheduler(this JsonRpcTestOptions options, IScheduler scheduler)
        {
            options.ClientScheduler = options.ServerScheduler = scheduler;
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

        public static JsonRpcTestOptions WithAssemblies(this JsonRpcTestOptions options, IEnumerable<Assembly> assemblies)
        {
            options.Assemblies = options.Assemblies.Concat(assemblies).ToArray();
            return options;
        }

        public static JsonRpcTestOptions WithAssemblies(this JsonRpcTestOptions options, params Assembly[] assemblies)
        {
            options.Assemblies = options.Assemblies.Concat(assemblies).ToArray();
            return options;
        }
    }
}
