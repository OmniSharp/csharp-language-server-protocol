using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class JsonRpcServerOptionsExtensions
    {
        public static JsonRpcServerOptions WithInput(this JsonRpcServerOptions options, Stream input)
        {
            options.Input = input;
            return options;
        }

        public static JsonRpcServerOptions WithOutput(this JsonRpcServerOptions options, Stream output)
        {
            options.Output = output;
            return options;
        }

        public static JsonRpcServerOptions WithLoggerFactory(this JsonRpcServerOptions options, ILoggerFactory loggerFactory)
        {
            options.LoggerFactory = loggerFactory;
            return options;
        }

        public static JsonRpcServerOptions WithRequestProcessIdentifier(this JsonRpcServerOptions options, IRequestProcessIdentifier requestProcessIdentifier)
        {
            options.RequestProcessIdentifier = requestProcessIdentifier;
            return options;
        }

        public static JsonRpcServerOptions WithSerializer(this JsonRpcServerOptions options, ISerializer serializer)
        {
            options.Serializer = serializer;
            return options;
        }

        public static JsonRpcServerOptions WithReciever(this JsonRpcServerOptions options, IReciever reciever)
        {
            options.Reciever = reciever;
            return options;
        }

        public static JsonRpcServerOptions WithHandler<T>(this JsonRpcServerOptions options)
            where T : class, IJsonRpcHandler
        {
            options.Services.AddSingleton<IJsonRpcHandler, T>();
            return options;
        }

        public static JsonRpcServerOptions WithHandler<T>(this JsonRpcServerOptions options, T handler)
            where T : IJsonRpcHandler
        {
            options.Services.AddSingleton<IJsonRpcHandler>(handler);
            return options;
        }

        public static JsonRpcServerOptions WithHandlersFrom(this JsonRpcServerOptions options, Type type)
        {
            options.HandlerTypes.Add(type);
            return options;
        }

        public static JsonRpcServerOptions WithHandlersFrom(this JsonRpcServerOptions options, TypeInfo typeInfo)
        {
            options.HandlerTypes.Add(typeInfo.AsType());
            return options;
        }

        public static JsonRpcServerOptions WithHandlersFrom(this JsonRpcServerOptions options, Assembly assembly)
        {
            options.HandlerAssemblies.Add(assembly);
            return options;
        }

        public static JsonRpcServerOptions WithServices(this JsonRpcServerOptions options, Action<IServiceCollection> servicesAction)
        {
            servicesAction(options.Services);
            return options;
        }
    }
}
