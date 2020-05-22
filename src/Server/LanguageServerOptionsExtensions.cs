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
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public static class LanguageServerOptionsExtensions
    {
        public static LanguageServerOptions WithInput(this LanguageServerOptions options, Stream input)
        {
            options.Input = input.UsePipeReader();
            return options;
        }
        public static LanguageServerOptions WithInput(this LanguageServerOptions options, PipeReader input)
        {
            options.Input = input;
            return options;
        }

        public static LanguageServerOptions WithOutput(this LanguageServerOptions options, Stream output)
        {
            options.Output = output.UsePipeWriter();
            return options;
        }

        public static LanguageServerOptions WithOutput(this LanguageServerOptions options, PipeWriter output)
        {
            options.Output = output;
            return options;
        }

        public static LanguageServerOptions WithPipe(this LanguageServerOptions options, Pipe pipe)
        {
            options.Input = pipe.Reader;
            options.Output = pipe.Writer;
            return options;
        }

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

        public static LanguageServerOptions WithReceiver(this LanguageServerOptions options, ILspServerReceiver serverReceiver)
        {
            options.Receiver = serverReceiver;
            return options;
        }

        public static LanguageServerOptions WithHandler<T>(this LanguageServerOptions options)
            where T : class, IJsonRpcHandler
        {
            options.Services.AddSingleton<IJsonRpcHandler, T>();
            return options;
        }

        public static LanguageServerOptions WithHandler<T>(this LanguageServerOptions options, T handler)
            where T : IJsonRpcHandler
        {
            options.Services.AddSingleton<IJsonRpcHandler>(handler);
            return options;
        }

        public static LanguageServerOptions WithHandlersFrom(this LanguageServerOptions options, Type type)
        {
            options.HandlerTypes.Add(type);
            return options;
        }

        public static LanguageServerOptions WithHandlersFrom(this LanguageServerOptions options, TypeInfo typeInfo)
        {
            options.HandlerTypes.Add(typeInfo.AsType());
            return options;
        }

        public static LanguageServerOptions WithHandlersFrom(this LanguageServerOptions options, Assembly assembly)
        {
            options.HandlerAssemblies.Add(assembly);
            return options;
        }

        public static LanguageServerOptions WithServices(this LanguageServerOptions options, Action<IServiceCollection> servicesAction)
        {
            servicesAction(options.Services);
            return options;
        }

        public static LanguageServerOptions WithServerInfo(this LanguageServerOptions options, ServerInfo serverInfo)
        {
            options.ServerInfo = serverInfo;
            return options;
        }

        /// <summary>
        /// Set maximum number of allowed parallel actions
        /// </summary>
        /// <param name="options"></param>
        /// <param name="concurrency"></param>
        /// <returns></returns>
        public static LanguageServerOptions WithConcurrency(this LanguageServerOptions options, int? concurrency)
        {
            options.Concurrency = concurrency;
            return options;
        }

        public static LanguageServerOptions OnInitialize(this LanguageServerOptions options, InitializeDelegate @delegate)
        {
            options.InitializeDelegates.Add(@delegate);
            return options;
        }


        public static LanguageServerOptions OnInitialized(this LanguageServerOptions options, InitializedDelegate @delegate)
        {
            options.InitializedDelegates.Add(@delegate);
            return options;
        }

        public static LanguageServerOptions OnStarted(this LanguageServerOptions options, OnServerStartedDelegate @delegate)
        {
            options.StartedDelegates.Add(@delegate);
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

        public static LanguageServerOptions WithErrorHandler(this LanguageServerOptions options, Func<ServerError, IHandlerDescriptor, Exception> handler)
        {
            options.OnServerError = handler;
            return options;
        }

        public static LanguageServerOptions WithContentModifiedSupport(this LanguageServerOptions options, bool supportsContentModified)
        {
            options.SupportsContentModified = supportsContentModified;
            return options;
        }
    }
}
