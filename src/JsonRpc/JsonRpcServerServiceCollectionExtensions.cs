using System;
using System.IO.Pipelines;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Nerdbank.Streams;
using Newtonsoft.Json;
using StreamJsonRpc;

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

            container.RegisterInstance(options.DefaultScheduler);

            container.Register<StreamJsonRpc.JsonRpc>(
                made: Made.Of(() => new StreamJsonRpc.JsonRpc(Arg.Of<IJsonRpcMessageHandler>())),
                reuse: Reuse.Singleton
            );
            container.RegisterInitializer<StreamJsonRpc.JsonRpc>(
                (rpc, context) => {
                    rpc.AllowModificationWhileListening = true;
                    rpc.CancelLocallyInvokedMethodsWhenConnectionIsClosed = true;
                }
            );

            container.RegisterMany<HeaderDelimitedMessageHandler>(
                made: Parameters.Of
                                .Type<PipeReader>(serviceKey: nameof(options.Input))
                                .Type<PipeWriter>(serviceKey: nameof(options.Output)),
                reuse: Reuse.Singleton
            );

            container.RegisterMany<JsonMessageFormatter>(reuse: Reuse.Singleton);
            container.RegisterInitializer<JsonMessageFormatter>(
                (formatter, context) => {
                    foreach (var converter in context.ResolveMany<JsonConverter>().Union(options.JsonConverters))
                        formatter.JsonSerializer.Converters.Add(converter);
                }
            );

            container.RegisterMany<ResponseRouter>(
                serviceTypeCondition: type => type.IsInterface,
                reuse: Reuse.Singleton
            );

            container.RegisterInstance(options.Handlers);
            container.RegisterInitializer<IJsonRpcHandlerCollection>(
                (collection, context) => {
                    foreach (var description in context
                                               .ResolveMany<JsonRpcHandlerDescription>()
                                               .Concat(
                                                    context
                                                       .ResolveMany<IJsonRpcHandler>().Select(_ => JsonRpcHandlerDescription.Infer(_))
                                                ))
                    {
                        collection.Add(description);
                    }
                }
            );
            container.RegisterMany<InstanceHasStarted>(nonPublicServiceTypes: true, reuse: Reuse.Singleton);

            return container.AddJsonRpcMediatR();
        }

        internal static IContainer AddJsonRpcMediatR(this IContainer container)
        {
            container.RegisterMany(new[] { typeof(IMediator).GetAssembly() }, Registrator.Interfaces, Reuse.ScopedOrSingleton);
            container.RegisterMany<RequestContext>(Reuse.Scoped);
            container.RegisterDelegate<ServiceFactory>(context => context.Resolve, Reuse.ScopedOrSingleton);
            container.Register(typeof(IRequestHandler<,>), typeof(RequestHandler<,>));
            container.Register(typeof(IRequestHandler<,>), typeof(RequestHandlerDecorator<,>), setup: Setup.Decorator);

            return container;
        }

        class RequestHandler<T, TR> : IRequestHandler<T, TR> where T : IRequest<TR>
        {
            private readonly IRequestContext _requestContext;

            public RequestHandler(IRequestContext requestContext)
            {
                _requestContext = requestContext;
            }

            public Task<TR> Handle(T request, CancellationToken cancellationToken)
            {
                return ( (IRequestHandler<T, TR>) _requestContext.Descriptor.Handler ).Handle(request, cancellationToken);
            }
        }

        class RequestHandlerDecorator<T, TR> : IRequestHandler<T, TR> where T : IRequest<TR>
        {
            private readonly IRequestHandler<T, TR>? _handler;
            private readonly IRequestContext? _requestContext;

            public RequestHandlerDecorator(IRequestHandler<T, TR>? handler = null, IRequestContext? requestContext = null)
            {
                _handler = handler;
                _requestContext = requestContext;
            }

            public Task<TR> Handle(T request, CancellationToken cancellationToken)
            {
                if (_requestContext == null)
                {
                    if (_handler == null)
                    {
                        throw new NotImplementedException($"No request handler was registered for type {typeof(IRequestHandler<T, TR>).FullName}");
                    }

                    return _handler.Handle(request, cancellationToken);
                }

                return ( (IRequestHandler<T, TR>) _requestContext.Descriptor.Handler ).Handle(request, cancellationToken);
            }
        }

        internal static IContainer AddJsonRpcServerInternals(this IContainer container, JsonRpcServerOptions options)
        {
            if (options.Serializer == null)
            {
                throw new ArgumentException("Serializer is missing!", nameof(options));
            }

            if (options.RequestProcessIdentifier == null)
            {
                throw new ArgumentException("RequestProcessIdentifier is missing!", nameof(options));
            }

            container = container.AddJsonRpcServerCore(options);

            if (options.UseAssemblyAttributeScanning)
            {
                container.RegisterInstanceMany(new AssemblyAttributeHandlerTypeDescriptorProvider(options.Assemblies), nonPublicServiceTypes: true);
            }
            else
            {
                container.RegisterInstanceMany(new AssemblyScanningHandlerTypeDescriptorProvider(options.Assemblies), nonPublicServiceTypes: true);
            }


            container.RegisterInstance(options.Serializer);
            if (options.Receiver == null)
            {
                container.Register<IReceiver, Receiver>(Reuse.Singleton);
            }
            else
            {
                container.RegisterInstance(options.Receiver);
            }

            container.RegisterMany<AlwaysOutputFilter>(Reuse.Singleton, nonPublicServiceTypes: true);

            container.RegisterInstance(options.RequestProcessIdentifier);
            container.RegisterInstance(options.OnUnhandledException ?? ( _ => { } ));

            container.RegisterMany<RequestRouter>(Reuse.Singleton);
            container.RegisterMany<HandlerCollection>(
                nonPublicServiceTypes: true,
                serviceTypeCondition: type => typeof(IHandlersManager) == type || type == typeof(HandlerCollection),
                reuse: Reuse.Singleton
            );
            container.RegisterInitializer<IHandlersManager>(
                (manager, context) => {
                    var descriptions = context.Resolve<IJsonRpcHandlerCollection>();
                    descriptions.Populate(context, manager);
                }
            );

            container.Register<IJsonRpcServerFacade, DefaultJsonRpcServerFacade>(reuse: Reuse.Singleton);
            container.RegisterInstance<IOptionsFactory<JsonRpcServerOptions>>(new ValueOptionsFactory<JsonRpcServerOptions>(options));
            container.RegisterMany<JsonRpcServer>(
                serviceTypeCondition: type => type == typeof(IJsonRpcServer) || type == typeof(JsonRpcServer),
                reuse: Reuse.Singleton,
                setup: Setup.With(condition: req => req.IsResolutionRoot || req.Container.Resolve<IInsanceHasStarted>().Started)
            );

            return container;
        }

        public static IServiceCollection AddJsonRpcServer(this IServiceCollection services, Action<JsonRpcServerOptions>? configureOptions = null) =>
            AddJsonRpcServer(services, Options.DefaultName, configureOptions);

        public static IServiceCollection AddJsonRpcServer(this IServiceCollection services, string name, Action<JsonRpcServerOptions>? configureOptions = null)
        {
            // If we get called multiple times we're going to remove the default server
            // and force consumers to use the resolver.
            if (services.Any(d => d.ServiceType == typeof(JsonRpcServer) || d.ServiceType == typeof(IJsonRpcServer)))
            {
                services.RemoveAll<JsonRpcServer>();
                services.RemoveAll<IJsonRpcServer>();
                services.AddSingleton<IJsonRpcServer>(
                    _ => throw new NotSupportedException("JsonRpcServer has been registered multiple times, you must use JsonRpcServerResolver instead")
                );
                services.AddSingleton<JsonRpcServer>(
                    _ => throw new NotSupportedException("JsonRpcServer has been registered multiple times, you must use JsonRpcServerResolver instead")
                );
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
