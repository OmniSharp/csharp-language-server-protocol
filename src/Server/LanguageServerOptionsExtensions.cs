using System.Reactive.Concurrency;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public static class LanguageServerOptionsExtensions
    {
        public static LanguageServerOptions WithRequestProcessIdentifier(this LanguageServerOptions options, IRequestProcessIdentifier requestProcessIdentifier)
        {
            options.RequestProcessIdentifier = requestProcessIdentifier;
            return options;
        }

        public static LanguageServerOptions WithSerializer(this LanguageServerOptions options, LspSerializer serializer)
        {
            options.Serializer = serializer;
            return options;
        }

        public static LanguageServerOptions WithReceiver(this LanguageServerOptions options, IReceiver serverReceiver)
        {
            options.Services.AddSingleton(serverReceiver);
            return options;
        }

        public static LanguageServerOptions WithServerInfo(this LanguageServerOptions options, ServerInfo serverInfo)
        {
            options.ServerInfo = serverInfo;
            return options;
        }

        /// <summary>
        /// Sets both input and output schedulers to the same scheduler
        /// </summary>
        /// <param name="options"></param>
        /// <param name="inputScheduler"></param>
        /// <returns></returns>
        public static LanguageServerOptions WithScheduler(this LanguageServerOptions options, IScheduler inputScheduler)
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
        public static LanguageServerOptions WithInputScheduler(this LanguageServerOptions options, IScheduler inputScheduler)
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
        public static LanguageServerOptions WithDefaultScheduler(this LanguageServerOptions options, IScheduler defaultScheduler)
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
        public static LanguageServerOptions WithOutputScheduler(this LanguageServerOptions options, IScheduler outputScheduler)
        {
            options.OutputScheduler = outputScheduler;
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
            options.Services.AddSingleton(new ConfigurationItem { Section = sectionName });
            return options;
        }

        public static LanguageServerOptions WithConfigurationItem(this LanguageServerOptions options, ConfigurationItem configurationItem)
        {
            options.Services.AddSingleton(configurationItem);
            return options;
        }
    }
}
