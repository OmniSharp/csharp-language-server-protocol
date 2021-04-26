using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using FastExpressionCompiler.LightExpression;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Client;
using StreamJsonRpc;

namespace OmniSharp.Extensions.JsonRpc
{
    internal class HandlerCollection : IHandlersManager, IEnumerable<IHandlerDescriptor>
    {
        private readonly StreamJsonRpc.JsonRpc _rpc;
        private readonly IResolverContext _resolverContext;
        private readonly IHandlerTypeDescriptorProvider<IHandlerTypeDescriptor?> _handlerTypeDescriptorProvider;
        private ImmutableArray<IHandlerDescriptor> _descriptors = ImmutableArray<IHandlerDescriptor>.Empty;
        private ILookup<string, IHandlerDescriptor>? _lookup;
        private ImmutableHashSet<string> _rpcMethods = ImmutableHashSet<string>.Empty;

        internal ILookup<string, IHandlerDescriptor> GetLookup() => _lookup ??= _descriptors.ToLookup(z => z.Method);

        public IEnumerable<IHandlerDescriptor> Descriptors => _descriptors;

        public HandlerCollection(
            StreamJsonRpc.JsonRpc rpc,
            IResolverContext resolverContext,
            IHandlerTypeDescriptorProvider<IHandlerTypeDescriptor?> handlerTypeDescriptorProvider
        )
        {
            _rpc = rpc;
            _resolverContext = resolverContext;
            _handlerTypeDescriptorProvider = handlerTypeDescriptorProvider;
        }

        private void Remove(IJsonRpcHandler handler)
        {
            var descriptors = _descriptors.ToBuilder();
            foreach (var item in _descriptors.Where(instance => instance.Handler == handler))
            {
                descriptors.Remove(item);
            }

            ImmutableInterlocked.InterlockedExchange(ref _descriptors, descriptors.ToImmutableArray());
            Interlocked.Exchange(ref _lookup, null);
        }

        public IDisposable Add(params IJsonRpcHandler[] handlers)
        {
            var cd = new CompositeDisposable();
            foreach (var handler in handlers)
            {
                if (_descriptors.Any(z => z.Handler == handler)) continue;
                cd.Add(Add(_handlerTypeDescriptorProvider.GetMethodName(handler.GetType())!, handler, null));
            }

            return cd;
        }

        public IDisposable Add(IJsonRpcHandler handler, JsonRpcHandlerOptions? options) => Add(_handlerTypeDescriptorProvider.GetMethodName(handler.GetType())!, handler, options);

        public IDisposable Add(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions? options)
        {
            var type = handler.GetType();
            var @interface = HandlerTypeDescriptorHelper.GetHandlerInterface(type);

            Type? @params = null;
            Type? response = null;
            if (@interface.GetTypeInfo().IsGenericType)
            {
                @params = @interface.GetTypeInfo().GetGenericArguments()[0];
                var requestInterface = @params.GetInterfaces()
                                              .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>));
                if (requestInterface != null)
                {
                    response = requestInterface.GetGenericArguments()[0];
                }
            }

            var requestProcessType =
                options?.RequestProcessType ??
                _handlerTypeDescriptorProvider.GetHandlerTypeDescriptor(type)?.RequestProcessType ??
                type.GetCustomAttributes(true)
                    .Concat(@interface.GetCustomAttributes(true))
                    .OfType<ProcessAttribute>()
                    .FirstOrDefault()?.Type;

            var descriptor = new HandlerInstance(method, handler, @interface, @params!, response!, requestProcessType, () => Remove(handler));
            ImmutableInterlocked.InterlockedExchange(ref _descriptors, _descriptors.Add(descriptor));
            Interlocked.Exchange(ref _lookup, null);

            if (!_rpcMethods.Contains(method))
            {
                AddRpcMethod(method, descriptor);
            }

            return descriptor;
        }

        private void AddRpcMethod(string method, IHandlerDescriptor descriptor)
        {
            Type rpcMethodType = null;
            if (typeof(IEnumerable).IsAssignableFrom(descriptor.Response)
             && typeof(string) != descriptor.Response
             && !typeof(JToken).IsAssignableFrom(descriptor.Response))
            {
                rpcMethodType = typeof(RpcRequestAggregateMethod<,>).MakeGenericType(descriptor.Params, descriptor.Response);
            }
            else if (descriptor.HasReturnType)
            {
                rpcMethodType = typeof(RpcRequestMethod<,>).MakeGenericType(descriptor.Params, descriptor.Response);
            }
            else
            {
                rpcMethodType = typeof(RpcNotificationMethod<>).MakeGenericType(descriptor.Params);
            }

            var instance = (IRpcMethod) ActivatorUtilities.CreateInstance(
                _resolverContext,
                rpcMethodType,
                new CustomDescriptorContainer(method, this)
            );
            if (instance == null) return;
            _rpc.AddLocalRpcMethod(method, instance.Method, instance);
        }

        class CustomDescriptorContainer : IRequestDescriptor<IHandlerDescriptor>
        {
            private readonly string _method;
            private readonly HandlerCollection _collection;

            public CustomDescriptorContainer(string method, HandlerCollection collection)
            {
                _method = method;
                _collection = collection;
            }

            public IEnumerator<IHandlerDescriptor> GetEnumerator() => _collection.GetLookup()[_method].GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public IHandlerDescriptor Default => _collection.GetLookup()[_method].FirstOrDefault();
        }

        interface IRpcMethod
        {
            MethodInfo Method { get; }
        }

        class RpcNotificationMethod<TRequest> : IRpcMethod
            where TRequest : IRequest
        {
            private readonly IServiceScopeFactory _serviceScopeFactory;
            private readonly IRequestDescriptor<IHandlerDescriptor> _descriptors;

            public RpcNotificationMethod(
                IServiceScopeFactory serviceScopeFactory,
                IRequestDescriptor<IHandlerDescriptor> descriptors
            )
            {
                _serviceScopeFactory = serviceScopeFactory;
                _descriptors = descriptors;
            }

            public async Task Request(TRequest request, CancellationToken cancellationToken)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                context.Descriptor = _descriptors.Default;
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                cancellationToken.ThrowIfCancellationRequested();

                await mediator.Send(request, cancellationToken).ConfigureAwait(false);
            }

            public MethodInfo Method { get; } = typeof(RpcNotificationMethod<TRequest>).GetMethod(nameof(Request), BindingFlags.Instance | BindingFlags.Public);
        }

        class RpcRequestMethod<TRequest, TResponse> : IRpcMethod
            where TRequest : IRequest<TResponse>
        {
            private readonly IServiceScopeFactory _serviceScopeFactory;
            private readonly IRequestDescriptor<IHandlerDescriptor> _descriptors;

            public RpcRequestMethod(
                IServiceScopeFactory serviceScopeFactory,
                IRequestDescriptor<IHandlerDescriptor> descriptors
            )
            {
                _serviceScopeFactory = serviceScopeFactory;
                _descriptors = descriptors;
            }

            public async Task<TResponse> Request(TRequest request, CancellationToken cancellationToken)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                context.Descriptor = _descriptors.Default;
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                cancellationToken.ThrowIfCancellationRequested();

                return await mediator.Send(request, cancellationToken).ConfigureAwait(false);
            }

            public MethodInfo Method { get; } = typeof(RpcRequestMethod<TRequest, TResponse>).GetMethod(nameof(Request), BindingFlags.Instance | BindingFlags.Public);
        }

        class RpcRequestAggregateMethod<TRequest, TResponse> : IRpcMethod
            where TRequest : IRequest<TResponse>
            where TResponse : IEnumerable
        {
            private readonly IServiceScopeFactory _serviceScopeFactory;
            private readonly IRequestDescriptor<IHandlerDescriptor> _descriptors;

            public RpcRequestAggregateMethod(
                IServiceScopeFactory serviceScopeFactory,
                IRequestDescriptor<IHandlerDescriptor> descriptors
            )
            {
                _serviceScopeFactory = serviceScopeFactory;
                _descriptors = descriptors;
            }

            public async Task<AggregateResponse<TResponse>> Request(TRequest request, CancellationToken cancellationToken)
            {
                // TODO: Do we want to support more handlers as "aggregate"?
//                typeof(IEnumerable).IsAssignableFrom(_descriptors.Default!.Response)
//                 && typeof(string) != _descriptors.Default!.Response
//                    && !typeof(JToken).IsAssignableFrom(_descriptors.Default!.Response)
                var responses = await Task.WhenAll(_descriptors.Select(descriptor => InnerRoute(_serviceScopeFactory, descriptor, request, cancellationToken)))
                                          .ConfigureAwait(false);

                return new AggregateResponse<TResponse>(responses);
            }

            static async Task<TResponse> InnerRoute(
                IServiceScopeFactory serviceScopeFactory, IHandlerDescriptor descriptor, TRequest @params, CancellationToken token
            )
            {
                using var scope = serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                context.Descriptor = descriptor;
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                token.ThrowIfCancellationRequested();

                return await mediator.Send(@params, token).ConfigureAwait(false);
            }

            public MethodInfo Method { get; } = typeof(RpcRequestMethod<TRequest, TResponse>).GetMethod(nameof(Request), BindingFlags.Instance | BindingFlags.Public);
        }

        public IDisposable Add(JsonRpcHandlerFactory factory, JsonRpcHandlerOptions? options) => Add(factory(_resolverContext), options);

        public IDisposable Add(string method, JsonRpcHandlerFactory factory, JsonRpcHandlerOptions? options) => Add(method, factory(_resolverContext), options);

        public IDisposable Add(Type handlerType, JsonRpcHandlerOptions? options) => Add(( _resolverContext.Resolve(handlerType) as IJsonRpcHandler )!, options);

        public IDisposable Add(string method, Type handlerType, JsonRpcHandlerOptions? options) =>
            Add(method, ( _resolverContext.Resolve(handlerType) as IJsonRpcHandler )!, options);

        public IDisposable AddLink(string fromMethod, string toMethod)
        {
            var source = _descriptors.FirstOrDefault(z => z.Method == fromMethod);
            if (source == null)
            {
                if (_descriptors.Any(z => z.Method == toMethod))
                {
                    throw new ArgumentException(
                        $"Could not find descriptor for '{fromMethod}', but I did find one for '{toMethod}'.  Did you mean to link '{toMethod}' to '{fromMethod}' instead?",
                        fromMethod
                    );
                }

                throw new ArgumentException(
                    $"Could not find descriptor for '{fromMethod}', has it been registered yet?  Descriptors must be registered before links can be created!", nameof(fromMethod)
                );
            }

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            var descriptor = new LinkedHandler(toMethod, source, () => _descriptors.RemoveAll(z => z.Method == toMethod));
            ImmutableInterlocked.InterlockedExchange(ref _descriptors, _descriptors.Add(descriptor));
            Interlocked.Exchange(ref _lookup, null);
            return descriptor;
        }

        public bool ContainsHandler(Type type) => _descriptors.Any(z => type.IsAssignableFrom(z.HandlerType));

        public bool ContainsHandler(TypeInfo type) => _descriptors.Any(z => type.IsAssignableFrom(z.HandlerType));

        public IEnumerator<IHandlerDescriptor> GetEnumerator() => ( (IEnumerable<IHandlerDescriptor>) _descriptors ).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable) _descriptors ).GetEnumerator();
    }
}
