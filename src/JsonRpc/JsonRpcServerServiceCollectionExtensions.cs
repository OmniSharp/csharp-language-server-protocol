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
            services.AddLogging().AddOptions();
            if (outerServiceProvider != null)
            {
                services.AddSingleton(outerServiceProvider.GetRequiredService<ILoggerFactory>());
            }
            else if (options.LoggerFactory != null)
            {
                services.AddSingleton(options.LoggerFactory);
            }

            services.AddSingleton(_ => options.Handlers);
            services.AddSingleton(_ => options.Serializer);
            services.AddSingleton(_ => options.Receiver);
            services.AddSingleton(_ => options.RequestProcessIdentifier);

            // _disposable = options.CompositeDisposable;
            services.AddJsonRpcMediatR();
            services.AddSingleton<JsonRpcServer>();
            services.AddSingleton<IJsonRpcServer>(_ => _.GetRequiredService<JsonRpcServer>());
            services.AddSingleton<IRequestRouter<IHandlerDescriptor>, RequestRouter>();
            services.AddSingleton<IResponseRouter, ResponseRouter>();
            services.AddSingleton(_ => new HandlerCollection(options.Handlers, outerServiceProvider ?? _));
            services.AddSingleton<IHandlersManager>(_ => _.GetRequiredService<HandlerCollection>());

            return services;
        }

        public static IServiceCollection AddJsonRpcServer(this IServiceCollection services, Action<JsonRpcServerOptions> configureOptions, string name = "Default")
        {
            services.AddSingleton(_ => {
                var options = _.GetRequiredService<IOptionsSnapshot<JsonRpcServerOptions>>().Get(name);

                var serviceProvider = options.Services
                    .AddJsonRpcServerInternals(options, _)
                    .Configure(configureOptions)
                    .BuildServiceProvider();

                var server = serviceProvider.GetRequiredService<JsonRpcServer>();
                return server;
            });

            return services;
        }
    }
}
