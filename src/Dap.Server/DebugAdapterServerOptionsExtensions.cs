using System;
using System.Reactive.Concurrency;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Server
{
    public static class DebugAdapterServerOptionsExtensions
    {
        public static DebugAdapterServerOptions WithSerializer(this DebugAdapterServerOptions options, ISerializer serializer)
        {
            options.Serializer = serializer;
            return options;
        }

        public static DebugAdapterServerOptions WithRequestProcessIdentifier(this DebugAdapterServerOptions options, IRequestProcessIdentifier requestProcessIdentifier)
        {
            options.RequestProcessIdentifier = requestProcessIdentifier;
            return options;
        }

        /// <summary>
        /// Sets both input and output schedulers to the same scheduler
        /// </summary>
        /// <param name="options"></param>
        /// <param name="inputScheduler"></param>
        /// <returns></returns>
        public static DebugAdapterServerOptions WithScheduler(this DebugAdapterServerOptions options, IScheduler inputScheduler)
        {
            options.InputScheduler = options.OutputScheduler = options.DefaultScheduler = inputScheduler;
            return options;
        }

        /// <summary>
        /// Sets the scheduler used during reading input
        /// </summary>
        /// <param name="options"></param>
        /// <param name="inputScheduler"></param>
        /// <returns></returns>
        public static DebugAdapterServerOptions WithInputScheduler(this DebugAdapterServerOptions options, IScheduler inputScheduler)
        {
            options.InputScheduler = inputScheduler;
            return options;
        }

        /// <summary>
        /// Sets the default scheduler to be used when scheduling other tasks
        /// </summary>
        /// <param name="options"></param>
        /// <param name="defaultScheduler"></param>
        /// <returns></returns>
        public static DebugAdapterServerOptions WithDefaultScheduler(this DebugAdapterServerOptions options, IScheduler defaultScheduler)
        {
            options.DefaultScheduler = defaultScheduler;
            return options;
        }

        /// <summary>
        /// Sets the scheduler use during writing output
        /// </summary>
        /// <param name="options"></param>
        /// <param name="outputScheduler"></param>
        /// <returns></returns>
        public static DebugAdapterServerOptions WithOutputScheduler(this DebugAdapterServerOptions options, IScheduler outputScheduler)
        {
            options.OutputScheduler = outputScheduler;
            return options;
        }

        public static DebugAdapterServerOptions OnInitialize(this DebugAdapterServerOptions options, OnDebugAdapterServerInitializeDelegate @delegate)
        {
            options.Services.AddSingleton(@delegate);
            return options;
        }

        public static DebugAdapterServerOptions OnInitialized(this DebugAdapterServerOptions options, OnDebugAdapterServerInitializedDelegate initializedDelegate)
        {
            options.Services.AddSingleton(initializedDelegate);
            return options;
        }

        public static DebugAdapterServerOptions OnStarted(this DebugAdapterServerOptions options, OnDebugAdapterServerStartedDelegate @delegate)
        {
            options.Services.AddSingleton(@delegate);
            return options;
        }

        public static DebugAdapterServerOptions ConfigureLogging(this DebugAdapterServerOptions options, Action<ILoggingBuilder> builderAction)
        {
            options.LoggingBuilderAction = builderAction;
            return options;
        }

        public static DebugAdapterServerOptions AddDefaultLoggingProvider(this DebugAdapterServerOptions options)
        {
            options.AddDefaultLoggingProvider = true;
            return options;
        }

        public static DebugAdapterServerOptions ConfigureConfiguration(this DebugAdapterServerOptions options, Action<IConfigurationBuilder> builderAction)
        {
            options.ConfigurationBuilderAction = builderAction;
            return options;
        }
    }
}
