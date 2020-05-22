using System;
using System.IO;
using System.IO.Pipelines;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nerdbank.Streams;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class JsonRpcServerOptionsExtensions
    {
        public static JsonRpcServerOptions WithInput(this JsonRpcServerOptions options, Stream input)
        {
            options.Input = input.UsePipeReader();
            return options;
        }
        public static JsonRpcServerOptions WithInput(this JsonRpcServerOptions options, PipeReader input)
        {
            options.Input = input;
            return options;
        }

        public static JsonRpcServerOptions WithOutput(this JsonRpcServerOptions options, Stream output)
        {
            options.Output = output.UsePipeWriter();
            return options;
        }

        public static JsonRpcServerOptions WithOutput(this JsonRpcServerOptions options, PipeWriter output)
        {
            options.Output = output;
            return options;
        }

        public static JsonRpcServerOptions WithPipe(this JsonRpcServerOptions options, Pipe pipe)
        {
            options.Input = pipe.Reader;
            options.Output = pipe.Writer;
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

        public static JsonRpcServerOptions WithReceiver(this JsonRpcServerOptions options, IReceiver receiver)
        {
            options.Receiver = receiver;
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

        public static JsonRpcServerOptions WithErrorHandler(this JsonRpcServerOptions options, Func<ServerError, IHandlerDescriptor, Exception> handler)
        {
            options.OnServerError = handler;
            return options;
        }

        public static JsonRpcServerOptions WithContentModifiedSupport(this JsonRpcServerOptions options, bool supportsContentModified)
        {
            options.SupportsContentModified = supportsContentModified;
            return options;
        }

        public static JsonRpcServerOptions WithMaximumRequestTimeout(this JsonRpcServerOptions options, TimeSpan maximumRequestTimeout)
        {
            options.MaximumRequestTimeout = maximumRequestTimeout;
            return options;
        }
    }
}
