using System;
using System.IO;
using System.IO.Pipelines;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nerdbank.Streams;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public static class LanguageClientOptionsExtensions
    {
        public static LanguageClientOptions WithInput(this LanguageClientOptions options, Stream input)
        {
            options.Input = input.UsePipeReader();
            options.RegisterForDisposal(input);
            return options;
        }
        public static LanguageClientOptions WithInput(this LanguageClientOptions options, PipeReader input)
        {
            options.Input = input;
            return options;
        }

        public static LanguageClientOptions WithOutput(this LanguageClientOptions options, Stream output)
        {
            options.Output = output.UsePipeWriter();
            options.RegisterForDisposal(output);
            return options;
        }

        public static LanguageClientOptions WithOutput(this LanguageClientOptions options, PipeWriter output)
        {
            options.Output = output;
            return options;
        }

        public static LanguageClientOptions WithPipe(this LanguageClientOptions options, Pipe pipe)
        {
            options.Input = pipe.Reader;
            options.Output = pipe.Writer;
            return options;
        }

        public static LanguageClientOptions WithRequestProcessIdentifier(this LanguageClientOptions options,
            IRequestProcessIdentifier requestProcessIdentifier)
        {
            options.RequestProcessIdentifier = requestProcessIdentifier;
            return options;
        }

        public static LanguageClientOptions WithSerializer(this LanguageClientOptions options, ISerializer serializer)
        {
            options.Serializer = serializer;
            return options;
        }

        public static LanguageClientOptions WithReceiver(this LanguageClientOptions options,
            ILspClientReceiver serverReceiver)
        {
            options.Receiver = serverReceiver;
            return options;
        }

        public static LanguageClientOptions WithHandler<T>(this LanguageClientOptions options)
            where T : class, IJsonRpcHandler
        {
            options.Services.AddSingleton<IJsonRpcHandler, T>();
            return options;
        }

        public static LanguageClientOptions WithHandler<T>(this LanguageClientOptions options, T handler)
            where T : IJsonRpcHandler
        {
            options.Services.AddSingleton<IJsonRpcHandler>(handler);
            return options;
        }

        public static LanguageClientOptions WithHandlersFrom(this LanguageClientOptions options, Type type)
        {
            options.HandlerTypes.Add(type);
            return options;
        }

        public static LanguageClientOptions WithHandlersFrom(this LanguageClientOptions options, TypeInfo typeInfo)
        {
            options.HandlerTypes.Add(typeInfo.AsType());
            return options;
        }

        public static LanguageClientOptions WithHandlersFrom(this LanguageClientOptions options, Assembly assembly)
        {
            options.HandlerAssemblies.Add(assembly);
            return options;
        }

        public static LanguageClientOptions WithServices(this LanguageClientOptions options,
            Action<IServiceCollection> servicesAction)
        {
            servicesAction(options.Services);
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

        public static LanguageClientOptions WithTrace(this LanguageClientOptions options, InitializeTrace trace)
        {
            options.Trace = trace;
            return options;
        }

        public static LanguageClientOptions WithInitializationOptions(this LanguageClientOptions options,
            object initializationOptions)
        {
            options.InitializationOptions = initializationOptions;
            return options;
        }

        public static LanguageClientOptions WithCapability(this LanguageClientOptions options, params ICapability[] capabilities)
        {
            options.SupportedCapabilities.AddRange(capabilities);
            return options;
        }

        public static LanguageClientOptions WithClientCapabilities(this LanguageClientOptions options, ClientCapabilities clientCapabilities)
        {
            options.ClientCapabilities = clientCapabilities;
            return options;
        }

        /// <summary>
        /// Set maximum number of allowed parallel actions
        /// </summary>
        /// <param name="options"></param>
        /// <param name="concurrency"></param>
        /// <returns></returns>
        public static LanguageClientOptions WithConcurrency(this LanguageClientOptions options, int? concurrency)
        {
            options.Concurrency = concurrency;
            return options;
        }

        // public static LanguageClientOptions OnInitialize(this LanguageClientOptions options, InitializeDelegate @delegate)
        // {
        //     options.InitializeDelegates.Add(@delegate);
        //     return options;
        // }
        //
        //
        // public static LanguageClientOptions OnInitialized(this LanguageClientOptions options, InitializedDelegate @delegate)
        // {
        //     options.InitializedDelegates.Add(@delegate);
        //     return options;
        // }

        public static LanguageClientOptions OnStarted(this LanguageClientOptions options,
            OnClientStartedDelegate @delegate)
        {
            options.StartedDelegates.Add(@delegate);
            return options;
        }

        public static LanguageClientOptions ConfigureLogging(this LanguageClientOptions options,
            Action<ILoggingBuilder> builderAction)
        {
            options.LoggingBuilderAction = builderAction;
            return options;
        }

        public static LanguageClientOptions AddDefaultLoggingProvider(this LanguageClientOptions options)
        {
            options.AddDefaultLoggingProvider = true;
            return options;
        }

        public static LanguageClientOptions ConfigureConfiguration(this LanguageClientOptions options,
            Action<IConfigurationBuilder> builderAction)
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

        public static LanguageClientOptions WithErrorHandler(this LanguageClientOptions options, Func<ServerError, IHandlerDescriptor, Exception> handler)
        {
            options.OnServerError = handler;
            return options;
        }

        public static LanguageClientOptions WithContentModifiedSupport(this LanguageClientOptions options, bool supportsContentModified)
        {
            options.SupportsContentModified = supportsContentModified;
            return options;
        }

        public static LanguageClientOptions WithMaximumRequestTimeout(this LanguageClientOptions options, TimeSpan maximumRequestTimeout)
        {
            options.MaximumRequestTimeout = maximumRequestTimeout;
            return options;
        }
    }
}
