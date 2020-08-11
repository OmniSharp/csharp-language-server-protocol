﻿using System;
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
