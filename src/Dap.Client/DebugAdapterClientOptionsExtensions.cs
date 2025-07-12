using System.Reactive.Concurrency;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Client
{
    public static class DebugAdapterClientOptionsExtensions
    {
        public static DebugAdapterClientOptions WithSerializer(this DebugAdapterClientOptions options, ISerializer serializer)
        {
            options.Serializer = serializer;
            return options;
        }

        public static DebugAdapterClientOptions WithRequestProcessIdentifier(this DebugAdapterClientOptions options, IRequestProcessIdentifier requestProcessIdentifier)
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
        public static DebugAdapterClientOptions WithScheduler(this DebugAdapterClientOptions options, IScheduler inputScheduler)
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
        public static DebugAdapterClientOptions WithInputScheduler(this DebugAdapterClientOptions options, IScheduler inputScheduler)
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
        public static DebugAdapterClientOptions WithDefaultScheduler(this DebugAdapterClientOptions options, IScheduler defaultScheduler)
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
        public static DebugAdapterClientOptions WithOutputScheduler(this DebugAdapterClientOptions options, IScheduler outputScheduler)
        {
            options.OutputScheduler = outputScheduler;
            return options;
        }

        public static DebugAdapterClientOptions OnInitialize(this DebugAdapterClientOptions options, OnDebugAdapterClientInitializeDelegate @delegate)
        {
            options.Services.AddSingleton(@delegate);
            return options;
        }

        public static DebugAdapterClientOptions OnInitialized(this DebugAdapterClientOptions options, OnDebugAdapterClientInitializedDelegate initializedDelegate)
        {
            options.Services.AddSingleton(initializedDelegate);
            return options;
        }

        public static DebugAdapterClientOptions OnStarted(this DebugAdapterClientOptions options, OnDebugAdapterClientStartedDelegate @delegate)
        {
            options.Services.AddSingleton(@delegate);
            return options;
        }

        public static DebugAdapterClientOptions ConfigureLogging(
            this DebugAdapterClientOptions options,
            Action<ILoggingBuilder> builderAction
        )
        {
            options.LoggingBuilderAction = builderAction;
            return options;
        }

        public static DebugAdapterClientOptions AddDefaultLoggingProvider(this DebugAdapterClientOptions options)
        {
            options.AddDefaultLoggingProvider = true;
            return options;
        }

        public static DebugAdapterClientOptions ConfigureConfiguration(
            this DebugAdapterClientOptions options,
            Action<IConfigurationBuilder> builderAction
        )
        {
            options.ConfigurationBuilderAction = builderAction;
            return options;
        }
    }
}
