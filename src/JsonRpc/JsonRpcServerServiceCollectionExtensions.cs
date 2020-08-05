using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class JsonRpcServerServiceCollectionExtensions
    {
        internal static IServiceCollection AddJsonRpcServerInternalsCore<T>(this IServiceCollection services, JsonRpcServerOptionsBase<T> options,
            IServiceProvider outerServiceProvider) where T : IJsonRpcHandlerRegistry<T>
        {
            if (options.Output == null)
            {
                throw new ArgumentException("Output is missing!", nameof(options));
            }

            if (options.Input == null)
            {
                throw new ArgumentException("Input is missing!", nameof(options));
            }

            if (options.Handlers == null)
            {
                throw new ArgumentException("Handlers is missing!", nameof(options));
            }

            services.AddSingleton<IOutputHandler>(_ => new OutputHandler(
                options.Output,
                _.GetRequiredService<ISerializer>(),
                _.GetRequiredService<ILogger<OutputHandler>>()
            ));
            services.AddSingleton(_ => new Connection(
                options.Input,
                _.GetRequiredService<IOutputHandler>(),
                _.GetRequiredService<IReceiver>(),
                options.RequestProcessIdentifier,
                _.GetRequiredService<IRequestRouter<IHandlerDescriptor>>(),
                _.GetRequiredService<IResponseRouter>(),
                _.GetRequiredService<ILoggerFactory>(),
                _.GetService<OnUnhandledExceptionHandler>() ?? options.OnUnhandledException ?? (e => { }),
                _.GetService<CreateResponseExceptionHandler>() ?? options.CreateResponseException,
                options.MaximumRequestTimeout,
                options.SupportsContentModified,
                options.Concurrency
            ));

            services
                .AddLogging()
                .AddOptions();

            if (outerServiceProvider != null)
            {
                services.AddSingleton(outerServiceProvider.GetService<ILoggerFactory>() ?? options.LoggerFactory);
            }
            else if (options.LoggerFactory != null)
            {
                services.AddSingleton(options.LoggerFactory);
            }

            services.TryAddSingleton<IFallbackServiceProvider>(_ => new FallbackServiceProvider(_, outerServiceProvider));
            services.AddJsonRpcMediatR();
            services.TryAddSingleton<IResponseRouter, ResponseRouter>();
            services.TryAddSingleton(_ => {
                var logger = _.GetRequiredService<ILoggerFactory>().CreateLogger("abcd");
                var descriptions = _.GetServices<JsonRpcHandlerDescription>();
                logger.LogCritical("hello");
                foreach (var description in descriptions)
                {
                    logger.LogCritical(description.GetType().FullName);
                    options.Handlers.Add(description);
                }

                return options.Handlers;
            });


            return services;
        }

        internal static IServiceCollection AddJsonRpcServerInternals(this IServiceCollection services, JsonRpcServerOptions options, IServiceProvider outerServiceProvider)
        {
            if (options.Serializer == null)
            {
                throw new ArgumentException("Serializer is missing!", nameof(options));
            }

            if (options.Receiver == null)
            {
                throw new ArgumentException("Receiver is missing!", nameof(options));
            }

            if (options.RequestProcessIdentifier == null)
            {
                throw new ArgumentException("RequestProcessIdentifier is missing!", nameof(options));
            }

            AddJsonRpcServerInternalsCore(services, options, outerServiceProvider);
            services.TryAddSingleton(_ => options.Serializer);
            services.TryAddSingleton(_ => options.Receiver);
            services.TryAddSingleton(_ => options.RequestProcessIdentifier);

            services.TryAddSingleton<JsonRpcServer>();
            services.TryAddSingleton<IJsonRpcServer>(_ => _.GetRequiredService<JsonRpcServer>());
            services.TryAddSingleton<IRequestRouter<IHandlerDescriptor>, RequestRouter>();
            services.TryAddSingleton<HandlerCollection>();
            services.TryAddSingleton<IHandlersManager>(_ => _.GetRequiredService<HandlerCollection>());
            services.TryAddSingleton<IOptionsFactory<JsonRpcServerOptions>>(new ValueOptionsFactory<JsonRpcServerOptions>(options));

            return services;
        }

        public static IServiceCollection AddJsonRpcServer(this IServiceCollection services, Action<JsonRpcServerOptions> configureOptions = null)
        {
            return AddJsonRpcServer(services, Options.DefaultName, configureOptions);
        }

        public static IServiceCollection AddJsonRpcServer(this IServiceCollection services, string name, Action<JsonRpcServerOptions> configureOptions = null)
        {
            // If we get called multiple times we're going to remove the default server
            // and force consumers to use the resolver.
            if (services.Any(d => d.ServiceType == typeof(JsonRpcServer) || d.ServiceType == typeof(IJsonRpcServer)))
            {
                services.RemoveAll<JsonRpcServer>();
                services.RemoveAll<IJsonRpcServer>();
                services.AddSingleton<IJsonRpcServer>(_ =>
                    throw new NotSupportedException("JsonRpcServer has been registered multiple times, you must use JsonRpcServerResolver instead"));
                services.AddSingleton<JsonRpcServer>(_ =>
                    throw new NotSupportedException("JsonRpcServer has been registered multiple times, you must use JsonRpcServerResolver instead"));
            }

            services
                .AddOptions()
                .AddLogging();
            services.TryAddSingleton<JsonRpcServerResolver>();
            services.TryAddSingleton(_ => _.GetRequiredService<JsonRpcServerResolver>().Get(name));
            services.TryAddSingleton<IJsonRpcServer>(_ => _.GetRequiredService<JsonRpcServerResolver>().Get(name));

            if (configureOptions != null)
            {
                services.Configure(name, configureOptions);
            }

            return services;
        }
    }
}
