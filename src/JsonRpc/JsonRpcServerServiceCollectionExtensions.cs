using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class JsonRpcServerServiceCollectionExtensions
    {
        internal static IServiceCollection AddJsonRpcServerInternals(this IServiceCollection services, JsonRpcServerOptions options, IServiceProvider outerServiceProvider)
        {
            if (options.Output == null)
            {
                throw new ArgumentException("Output is missing!", nameof(options));
            }
            if (options.Input == null)
            {
                throw new ArgumentException("Input is missing!", nameof(options));
            }
            if (options.Serializer == null)
            {
                throw new ArgumentException("Serializer is missing!", nameof(options));
            }
            if (options.RequestProcessIdentifier == null)
            {
                throw new ArgumentException("RequestProcessIdentifier is missing!", nameof(options));
            }
            if (options.Receiver == null)
            {
                throw new ArgumentException("Receiver is missing!", nameof(options));
            }
            if (options.Handlers == null)
            {
                throw new ArgumentException("Handlers is missing!", nameof(options));
            }
            services.AddSingleton<IOutputHandler>(_ => new OutputHandler(
                options.Output,
                options.Serializer,
                _.GetRequiredService<ILogger<OutputHandler>>()
            ));
            services.AddSingleton(_ => new Connection(
                options.Input,
                _.GetRequiredService<IOutputHandler>(),
                options.Receiver,
                options.RequestProcessIdentifier,
                _.GetRequiredService<IRequestRouter<IHandlerDescriptor>>(),
                _.GetRequiredService<IResponseRouter>(),
                _.GetRequiredService<ILoggerFactory>(),
                options.OnUnhandledException ?? (e => { }),
                options.CreateResponseException,
                options.MaximumRequestTimeout,
                options.SupportsContentModified,
                options.Concurrency
            ));
            if (outerServiceProvider != null)
            {
                services.AddSingleton(outerServiceProvider.GetRequiredService<ILoggerFactory>());
            }
            else if (options.LoggerFactory != null)
            {
                services.AddSingleton(options.LoggerFactory);
            }

            services
                .AddLogging()
                .AddOptions()
                .AddSingleton(_ => options.Handlers)
                .AddSingleton(_ => options.Serializer)
                .AddSingleton(_ => options.Receiver)
                .AddSingleton(_ => options.RequestProcessIdentifier)
                .AddJsonRpcMediatR()
                .AddSingleton<JsonRpcServer>()
                .AddSingleton<IJsonRpcServer>(_ => _.GetRequiredService<JsonRpcServer>())
                .AddSingleton<IRequestRouter<IHandlerDescriptor>, RequestRouter>()
                .AddSingleton<IResponseRouter, ResponseRouter>()
                .AddSingleton(_ => new HandlerCollection(options.Handlers, outerServiceProvider ?? _))
                .AddSingleton<IHandlersManager>(_ => _.GetRequiredService<HandlerCollection>())
                .AddSingleton<IOptionsFactory<JsonRpcServerOptions>>(new ValueOptionsFactory<JsonRpcServerOptions>(options));

            return services;
        }

        public static IServiceCollection AddJsonRpcServer(this IServiceCollection services, Action<JsonRpcServerOptions> configureOptions = null)
        {
            return AddJsonRpcServer(services, Options.DefaultName, configureOptions);
        }

        public static IServiceCollection AddJsonRpcServer(this IServiceCollection services, string name, Action<JsonRpcServerOptions> configureOptions = null)
        {
            services
                .AddOptions()
                .AddLogging()
                .AddSingleton(_ => {
                var options = _.GetRequiredService<IOptionsMonitor<JsonRpcServerOptions>>().Get(name);

                var serviceProvider = options.Services
                    .AddJsonRpcServerInternals(options, _)
                    .BuildServiceProvider();

                var server = serviceProvider.GetRequiredService<JsonRpcServer>();
                return server;
            });
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }

            return services;
        }
    }
}
