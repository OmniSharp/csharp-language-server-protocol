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
            container.RegisterInstance(options.InputScheduler, serviceKey: nameof(options.InputScheduler));
            container.RegisterInstance(options.OutputScheduler, serviceKey: nameof(options.OutputScheduler));
            if (options.CreateResponseException != null)
            {
                container.RegisterInstance(options.CreateResponseException);
            }

            container.RegisterMany<OutputHandler>(
                nonPublicServiceTypes: true,
                made: Parameters.Of
                                .Type<PipeWriter>(serviceKey: nameof(options.Output))
                                .Type<IScheduler>(serviceKey: nameof(options.OutputScheduler)),
                reuse: Reuse.Singleton
            );

            container.Register<RequestInvokerOptions>(
                made: Made.Of().Parameters
                          .Type<TimeSpan>(serviceKey: nameof(options.MaximumRequestTimeout))
                          .Type<bool>(serviceKey: nameof(options.SupportsContentModified))
                          .Name("concurrency", serviceKey: nameof(options.Concurrency)),
                reuse: Reuse.Singleton);

            if (!container.IsRegistered<RequestInvoker>())
            {
                container.Register<RequestInvoker, DefaultRequestInvoker>(
                    made: Made.Of().Parameters
                              .Type<IScheduler>(serviceKey: nameof(options.InputScheduler)),
                    reuse: Reuse.Singleton);
            }

            container.Register<Connection>(
                made: Made.Of().Parameters
                          .Type<PipeReader>(serviceKey: nameof(options.Input))
               ,
                reuse: Reuse.Singleton
            );

            container.RegisterInstance(options.DefaultScheduler);

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
            // Select the desired constructor
            container.Register<IMediator, Mediator>(made: Made.Of(() => new Mediator(Arg.Of<IServiceProvider>())));
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
                return ((IRequestHandler<T, TR>) _requestContext.Descriptor.Handler).Handle(request, cancellationToken);
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

                return ((IRequestHandler<T, TR>) _requestContext.Descriptor.Handler).Handle(request, cancellationToken);
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
