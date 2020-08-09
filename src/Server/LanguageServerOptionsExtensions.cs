using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Pipelines;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nerdbank.Streams;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public static class LanguageServerOptionsExtensions
    {
        public static LanguageServerOptions WithRequestProcessIdentifier(this LanguageServerOptions options, IRequestProcessIdentifier requestProcessIdentifier)
        {
            options.RequestProcessIdentifier = requestProcessIdentifier;
            return options;
        }

        public static LanguageServerOptions WithSerializer(this LanguageServerOptions options, ISerializer serializer)
        {
            options.Serializer = serializer;
            return options;
        }

        public static LanguageServerOptions WithServerInfo(this LanguageServerOptions options, ServerInfo serverInfo)
        {
            options.ServerInfo = serverInfo;
            return options;
        }

        public static LanguageServerOptions OnInitialize(this LanguageServerOptions options, OnLanguageServerInitializeDelegate @delegate)
        {
            options.Services.AddSingleton(@delegate);
            return options;
        }


        public static LanguageServerOptions OnInitialized(this LanguageServerOptions options, OnLanguageServerInitializedDelegate @delegate)
        {
            options.Services.AddSingleton(@delegate);
            return options;
        }

        public static LanguageServerOptions OnStarted(this LanguageServerOptions options, OnLanguageServerStartedDelegate @delegate)
        {
            options.Services.AddSingleton(@delegate);
            return options;
        }

        public static LanguageServerOptions ConfigureLogging(this LanguageServerOptions options, Action<ILoggingBuilder> builderAction)
        {
            options.LoggingBuilderAction = builderAction;
            return options;
        }

        public static LanguageServerOptions AddDefaultLoggingProvider(this LanguageServerOptions options)
        {
            options.AddDefaultLoggingProvider = true;
            return options;
        }

        public static LanguageServerOptions ConfigureConfiguration(this LanguageServerOptions options, Action<IConfigurationBuilder> builderAction)
        {
            options.ConfigurationBuilderAction = builderAction;
            return options;
        }

        public static LanguageServerOptions WithConfigurationSection(this LanguageServerOptions options, string sectionName)
        {
            options.Services.AddSingleton(new ConfigurationItem() {Section = sectionName});
            return options;
        }

        public static LanguageServerOptions WithConfigurationItem(this LanguageServerOptions options, ConfigurationItem configurationItem)
        {
            options.Services.AddSingleton(configurationItem);
            return options;
        }
    }
}
