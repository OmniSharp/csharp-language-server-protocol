using System;
using System.Reactive.Concurrency;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public static class LanguageClientOptionsExtensions
    {
        public static LanguageClientOptions WithSerializer(this LanguageClientOptions options, LspSerializer serializer)
        {
            options.Serializer = serializer;
            return options;
        }

        public static LanguageClientOptions WithReceiver(this LanguageClientOptions options, IReceiver serverReceiver)
        {
            options.Services.AddSingleton(serverReceiver);
            return options;
        }

        public static LanguageClientOptions WithClientInfo(this LanguageClientOptions options, ClientInfo clientInfo)
        {
            options.ClientInfo = clientInfo;
            return options;
        }

        public static LanguageClientOptions WithRootUri(this LanguageClientOptions options, DocumentUri rootUri)
        {
            options.RootUri = rootUri;
            return options;
        }

        public static LanguageClientOptions WithRootPath(this LanguageClientOptions options, string rootPath)
        {
            options.RootPath = rootPath;
            return options;
        }

        public static LanguageClientOptions WithWorkspaceFolder(this LanguageClientOptions options, WorkspaceFolder workspaceFolder)
        {
            options.Services.AddSingleton(workspaceFolder);
            return options;
        }

        public static LanguageClientOptions WithWorkspaceFolder(this LanguageClientOptions options, DocumentUri documentUri, string name)
        {
            options.Services.AddSingleton(new WorkspaceFolder { Name = name, Uri = documentUri });
            return options;
        }

        public static LanguageClientOptions WithTrace(this LanguageClientOptions options, InitializeTrace trace)
        {
            options.Trace = trace;
            return options;
        }

        public static LanguageClientOptions WithInitializationOptions(
            this LanguageClientOptions options,
            object initializationOptions
        )
        {
            options.InitializationOptions = initializationOptions;
            return options;
        }

        public static LanguageClientOptions WithCapability(this LanguageClientOptions options, ICapability capability, params ICapability[] capabilities)
        {
            options.Services.AddSingleton(capability);
            foreach (var item in capabilities)
            {
                options.Services.AddSingleton(item);
            }

            return options;
        }

        public static LanguageClientOptions WithClientCapabilities(this LanguageClientOptions options, ClientCapabilities clientCapabilities)
        {
            options.ClientCapabilities = clientCapabilities;
            return options;
        }

        /// <summary>
        /// Sets both input and output schedulers to the same scheduler
        /// </summary>
        /// <param name="options"></param>
        /// <param name="inputScheduler"></param>
        /// <returns></returns>
        public static LanguageClientOptions WithScheduler(this LanguageClientOptions options, IScheduler inputScheduler)
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
        public static LanguageClientOptions WithInputScheduler(this LanguageClientOptions options, IScheduler inputScheduler)
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
        public static LanguageClientOptions WithDefaultScheduler(this LanguageClientOptions options, IScheduler defaultScheduler)
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
        public static LanguageClientOptions WithOutputScheduler(this LanguageClientOptions options, IScheduler outputScheduler)
        {
            options.OutputScheduler = outputScheduler;
            return options;
        }

        public static LanguageClientOptions OnInitialize(this LanguageClientOptions options, OnLanguageClientInitializeDelegate @delegate)
        {
            options.Services.AddSingleton(@delegate);
            return options;
        }


        public static LanguageClientOptions OnInitialized(this LanguageClientOptions options, OnLanguageClientInitializedDelegate @delegate)
        {
            options.Services.AddSingleton(@delegate);
            return options;
        }

        public static LanguageClientOptions OnStarted(this LanguageClientOptions options, OnLanguageClientStartedDelegate @delegate)
        {
            options.Services.AddSingleton(@delegate);
            return options;
        }

        public static LanguageClientOptions ConfigureLogging(
            this LanguageClientOptions options,
            Action<ILoggingBuilder> builderAction
        )
        {
            options.LoggingBuilderAction = builderAction;
            return options;
        }

        public static LanguageClientOptions AddDefaultLoggingProvider(this LanguageClientOptions options)
        {
            options.AddDefaultLoggingProvider = true;
            return options;
        }

        public static LanguageClientOptions ConfigureConfiguration(
            this LanguageClientOptions options,
            Action<IConfigurationBuilder> builderAction
        )
        {
            options.ConfigurationBuilderAction = builderAction;
            return options;
        }

        public static LanguageClientOptions EnableWorkspaceFolders(this LanguageClientOptions options)
        {
            options.WorkspaceFolders = true;
            return options;
        }

        public static LanguageClientOptions DisableWorkspaceFolders(this LanguageClientOptions options)
        {
            options.WorkspaceFolders = false;
            return options;
        }

        public static LanguageClientOptions EnableDynamicRegistration(this LanguageClientOptions options)
        {
            options.DynamicRegistration = true;
            return options;
        }

        public static LanguageClientOptions DisableDynamicRegistration(this LanguageClientOptions options)
        {
            options.DynamicRegistration = false;
            return options;
        }

        public static LanguageClientOptions EnableProgressTokens(this LanguageClientOptions options)
        {
            options.ProgressTokens = true;
            return options;
        }

        public static LanguageClientOptions DisableProgressTokens(this LanguageClientOptions options)
        {
            options.ProgressTokens = false;
            return options;
        }
    }
}
