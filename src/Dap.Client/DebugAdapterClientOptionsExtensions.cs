using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.DebugAdapter.Protocol;
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

        public static DebugAdapterClientOptions WithServices(this DebugAdapterClientOptions options, Action<IServiceCollection> servicesAction)
        {
            servicesAction(options.Services);
            return options;
        }

        public static DebugAdapterClientOptions OnStarted(this DebugAdapterClientOptions options,
            OnClientStartedDelegate @delegate)
        {
            options.StartedDelegates.Add(@delegate);
            return options;
        }

        public static DebugAdapterClientOptions ConfigureLogging(this DebugAdapterClientOptions options,
            Action<ILoggingBuilder> builderAction)
        {
            options.LoggingBuilderAction = builderAction;
            return options;
        }

        public static DebugAdapterClientOptions AddDefaultLoggingProvider(this DebugAdapterClientOptions options)
        {
            options.AddDefaultLoggingProvider = true;
            return options;
        }

        public static DebugAdapterClientOptions ConfigureConfiguration(this DebugAdapterClientOptions options,
            Action<IConfigurationBuilder> builderAction)
        {
            options.ConfigurationBuilderAction = builderAction;
            return options;
        }
    }
}