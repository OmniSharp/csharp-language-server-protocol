using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public static class LanguageServerOptionsExtensions
    {
        public static LanguageServerOptions WithInput(this LanguageServerOptions options, Stream input)
        {
            options.Input = input;
            return options;
        }

        public static LanguageServerOptions WithOutput(this LanguageServerOptions options, Stream output)
        {
            options.Output = output;
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

        public static LanguageServerOptions WithReciever(this LanguageServerOptions options, ILspReciever reciever)
        {
            options.Reciever = reciever;
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

        public static LanguageServerOptions AddDefaultLoggingProvider(this LanguageServerOptions options)
        {
            options.AddDefaultLoggingProvider = true;
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

        public static LanguageServerOptions ConfigureLogging(this LanguageServerOptions options, Action<ILoggingBuilder> builderAction)
        {
            options.LoggingBuilderAction = builderAction;
            return options;
        }
    }
}
