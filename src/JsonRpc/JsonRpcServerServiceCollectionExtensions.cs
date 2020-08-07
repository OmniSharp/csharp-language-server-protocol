using System;
using System.IO.Pipelines;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using DryIoc;
using MediatR.Pipeline;
using MediatR.Registration;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc.Pipelines;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Serialization;
using MediatR;
using OmniSharp.Extensions.JsonRpc.DryIoc;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class JsonRpcServerServiceCollectionExtensions
    {
        internal static IContainer AddJsonRpcServerCore<T>(this IContainer container, JsonRpcServerOptionsBase<T> options) where T : IJsonRpcHandlerRegistry<T>
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

            container = container.Populate(options.Services);

            container.RegisterInstance(options.Output, serviceKey: nameof(options.Output));
            container.RegisterInstance(options.Input, serviceKey: nameof(options.Input));
            container.RegisterInstance(options.MaximumRequestTimeout, serviceKey: nameof(options.MaximumRequestTimeout));
            container.RegisterInstance(options.SupportsContentModified, serviceKey: nameof(options.SupportsContentModified));
            container.RegisterInstance(options.Concurrency ?? -1, serviceKey: nameof(options.Concurrency));
            if (options.CreateResponseException != null)
            {
                container.RegisterInstance(options.CreateResponseException);
            }

            container.RegisterMany<OutputHandler>(
                serviceTypeCondition: type => type.IsInterface,
                made: Parameters.Of
                    .Type<PipeWriter>(serviceKey: nameof(options.Output)),
                reuse: Reuse.Singleton
            );
            container.Register<Connection>(
                made: new Made.TypedMade<Connection>().Parameters
                    .Type<PipeReader>(serviceKey: nameof(options.Input))
                    .Type<TimeSpan>(serviceKey: nameof(options.MaximumRequestTimeout))
                    .Type<bool>(serviceKey: nameof(options.SupportsContentModified))
                    .Name("concurrency", serviceKey: nameof(options.Concurrency)),
                reuse: Reuse.Singleton
            );

            container.RegisterMany<ResponseRouter>(
                serviceTypeCondition: type => type.IsInterface,
                reuse: Reuse.Singleton
            );

            container.RegisterInstance(options.Handlers);
            container.RegisterInitializer<IJsonRpcHandlerCollection>((collection, context) => {
                foreach (var description in context.ResolveMany<JsonRpcHandlerDescription>()
                    .Concat(context.ResolveMany<IJsonRpcHandler>().Select(_ => JsonRpcHandlerDescription.Infer(_)))
                )
                {
                    collection.Add(description);
                }
            });

            if (options.LoggerFactory != null)
            {
                container.RegisterInstance(options.LoggerFactory, IfAlreadyRegistered.Keep);
            }

            return container.AddJsonRpcMediatR();
        }

        internal static IContainer AddJsonRpcMediatR(this IContainer container)
        {
            container.RegisterMany(new[] { typeof(IMediator).GetAssembly() }, Registrator.Interfaces, reuse: Reuse.ScopedOrSingleton);
            container.RegisterMany(new[] { typeof(RequestMustNotBeNullProcessor<>), typeof(ResponseMustNotBeNullProcessor<,>) }, reuse: Reuse.ScopedOrSingleton);
            container.RegisterMany<RequestContext>();
            container.RegisterDelegate<ServiceFactory>(context => context.Resolve);

            return container.With(rules => rules.WithUnknownServiceResolvers(request => {
                if (request.ServiceType.IsGenericType && typeof(IRequestHandler<,>).IsAssignableFrom(request.ServiceType.GetGenericTypeDefinition()))
                {
                    var context = request.Container.Resolve<IRequestContext>();
                    if (context != null)
                    {
                        return new RegisteredInstanceFactory(context.Descriptor.Handler);
                    }
                }
                return null;
            }));
        }

        internal static IContainer AddJsonRpcServerInternals(this IContainer container, JsonRpcServerOptions options)
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

            container = container.AddJsonRpcServerCore(options);

            container.RegisterInstance(options.Serializer ?? new JsonRpcSerializer());
            container.RegisterInstance(options.Receiver);
            container.RegisterInstance(options.RequestProcessIdentifier);
            container.RegisterInstance(options.OnUnhandledException ?? (e => { }));

            container.RegisterMany<RequestRouter>(reuse: Reuse.Singleton);
            container.RegisterMany<HandlerCollection>(
                nonPublicServiceTypes: true,
                serviceTypeCondition: type => typeof(IHandlersManager) == type || type == typeof(HandlerCollection),
                reuse: Reuse.Singleton
            );
            container.RegisterInitializer<IHandlersManager>((manager, context) => {
                var descriptions = context.Resolve<IJsonRpcHandlerCollection>();
                descriptions.Populate(context, manager);
            });

            container.RegisterInstance<IOptionsFactory<JsonRpcServerOptions>>(new ValueOptionsFactory<JsonRpcServerOptions>(options));
            container.RegisterMany<JsonRpcServer>(serviceTypeCondition: type => type == typeof(IJsonRpcServer) || type == typeof(JsonRpcServer), reuse: Reuse.Singleton);

            return container;
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
