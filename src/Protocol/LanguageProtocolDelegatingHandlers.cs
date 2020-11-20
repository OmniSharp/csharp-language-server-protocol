using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static class LanguageProtocolDelegatingHandlers
    {
        public sealed class Request<TParams, TResult, TRegistrationOptions, TCapability> :
            AbstractHandlers.Base<TRegistrationOptions, TCapability>,
            IJsonRpcRequestHandler<TParams, TResult>,
            ICanBeIdentifiedHandler
            where TParams : IRequest<TResult>
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task<TResult>> _handler;
            private readonly Func<TCapability, TRegistrationOptions> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public Request(Func<TParams, TCapability, Task<TResult>> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory) :
                this(Guid.Empty, (a, c, _) => handler(a, c), registrationOptionsFactory)
            {
            }

            public Request(Func<TParams, TCapability, CancellationToken, Task<TResult>> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory) :
                this(Guid.Empty, handler, registrationOptionsFactory)
            {
            }

            public Request(Guid id, Func<TParams, TCapability, Task<TResult>> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory) :
                this(id, (a, c, _) => handler(a, c), registrationOptionsFactory)
            {
            }

            public Request(Guid id, Func<TParams, TCapability, CancellationToken, Task<TResult>> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory)
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            Task<TResult> IRequestHandler<TParams, TResult>.
                Handle(TParams request, CancellationToken cancellationToken) =>
                _handler(request, Capability, cancellationToken);

            protected override TRegistrationOptions CreateRegistrationOptions(TCapability capability) => _registrationOptionsFactory(capability);
        }

        public sealed class CanBeResolved<TItem, TRegistrationOptions, TCapability> :
            AbstractHandlers.Base<TRegistrationOptions, TCapability>,
            ICanBeResolvedHandler<TItem>,
            ICanBeIdentifiedHandler
            where TItem : ICanBeResolved, IRequest<TItem>
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Func<TItem, TCapability, CancellationToken, Task<TItem>> _resolveHandler;
                        private readonly Func<TCapability, TRegistrationOptions> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public CanBeResolved(Guid id, Func<TItem, TCapability, Task<TItem>> resolveHandler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory) :
                this(id, (a, c, _) => resolveHandler(a, c), registrationOptionsFactory)
            {
            }

            public CanBeResolved(Guid id, Func<TItem, TCapability, CancellationToken, Task<TItem>> resolveHandler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory)
            {
                _resolveHandler = resolveHandler;
                _registrationOptionsFactory = registrationOptionsFactory;
                _id = id;
            }

            Task<TItem> IRequestHandler<TItem, TItem>.Handle(TItem request, CancellationToken cancellationToken) => _resolveHandler(request, Capability, cancellationToken);

            protected override TRegistrationOptions CreateRegistrationOptions(TCapability capability) => _registrationOptionsFactory(capability);
        }

        public sealed class CanBeResolved<TItem, TRegistrationOptions> :
            AbstractHandlers.Base<TRegistrationOptions>,
            ICanBeResolvedHandler<TItem>,
            ICanBeIdentifiedHandler
            where TItem : ICanBeResolved, IRequest<TItem>
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TItem, CancellationToken, Task<TItem>> _resolveHandler;
            private readonly Func<TRegistrationOptions> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public CanBeResolved(Guid id, Func<TItem, Task<TItem>> resolveHandler, Func<TRegistrationOptions> registrationOptionsFactory) :
                this(id, (a, _) => resolveHandler(a), registrationOptionsFactory)
            {
            }

            public CanBeResolved(Guid id, Func<TItem, CancellationToken, Task<TItem>> resolveHandler, Func<TRegistrationOptions> registrationOptionsFactory)
            {
                _id = id;
                _resolveHandler = resolveHandler;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            Task<TItem> IRequestHandler<TItem, TItem>.Handle(TItem request, CancellationToken cancellationToken) => _resolveHandler(request, cancellationToken);

            protected override TRegistrationOptions CreateRegistrationOptions() => _registrationOptionsFactory();
        }

        public sealed class Request<TParams, TRegistrationOptions, TCapability> :
            AbstractHandlers.Base<TRegistrationOptions, TCapability>,
            IJsonRpcRequestHandler<TParams>,
            ICanBeIdentifiedHandler
            where TParams : IRequest
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task> _handler;
            private readonly Func<TCapability, TRegistrationOptions> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public Request(Func<TParams, TCapability, Task> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory) :
                this(Guid.Empty, (a, c, _) => handler(a, c), registrationOptionsFactory)
            {
            }

            public Request(Func<TParams, TCapability, CancellationToken, Task> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory) :
                this(Guid.Empty, handler, registrationOptionsFactory)
            {
            }


            public Request(Guid id, Func<TParams, TCapability, Task> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory) :
                this(id, (a, c, _) => handler(a, c), registrationOptionsFactory)
            {
            }

            public Request(Guid id, Func<TParams, TCapability, CancellationToken, Task> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory)
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, Capability, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }

            protected override TRegistrationOptions CreateRegistrationOptions(TCapability capability) => _registrationOptionsFactory(capability);
        }

        public sealed class RequestRegistration<TParams, TResult, TRegistrationOptions> :
            AbstractHandlers.Base<TRegistrationOptions>,
            IJsonRpcRequestHandler<TParams, TResult>,
            ICanBeIdentifiedHandler
            where TParams : IRequest<TResult>
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TParams, CancellationToken, Task<TResult>> _handler;
            private readonly Func<TRegistrationOptions> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public RequestRegistration(Func<TParams, Task<TResult>> handler, Func<TRegistrationOptions> registrationOptionsFactory) :
                this(Guid.Empty, (a, _) => handler(a), registrationOptionsFactory)
            {
            }

            public RequestRegistration(Func<TParams, CancellationToken, Task<TResult>> handler, Func<TRegistrationOptions> registrationOptionsFactory) :
                this(Guid.Empty, handler, registrationOptionsFactory)
            {
            }

            public RequestRegistration(Guid id, Func<TParams, Task<TResult>> handler, Func<TRegistrationOptions> registrationOptionsFactory) :
                this(id, (a, _) => handler(a), registrationOptionsFactory)
            {
            }

            public RequestRegistration(Guid id, Func<TParams, CancellationToken, Task<TResult>> handler, Func<TRegistrationOptions> registrationOptionsFactory)
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            Task<TResult> IRequestHandler<TParams, TResult>.Handle(TParams request, CancellationToken cancellationToken) => _handler(request, cancellationToken);


            protected override TRegistrationOptions CreateRegistrationOptions() => _registrationOptionsFactory();
        }

        public sealed class RequestRegistration<TParams, TRegistrationOptions> :
            AbstractHandlers.Base<TRegistrationOptions>,
            IJsonRpcRequestHandler<TParams>,
            ICanBeIdentifiedHandler
            where TParams : IRequest
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TParams, CancellationToken, Task> _handler;
            private readonly Func<TRegistrationOptions> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public RequestRegistration(Func<TParams, Task> handler, Func<TRegistrationOptions> registrationOptionsFactory) :
                this(Guid.Empty, (a, _) => handler(a), registrationOptionsFactory)
            {
            }

            public RequestRegistration(Func<TParams, CancellationToken, Task> handler, Func<TRegistrationOptions> registrationOptionsFactory) :
                this(Guid.Empty, handler, registrationOptionsFactory)
            {
            }

            public RequestRegistration(Guid id, Func<TParams, Task> handler, Func<TRegistrationOptions> registrationOptionsFactory) :
                this(id, (a, _) => handler(a), registrationOptionsFactory)
            {
            }

            public RequestRegistration(Guid id, Func<TParams, CancellationToken, Task> handler, Func<TRegistrationOptions> registrationOptionsFactory)
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.
                Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }

            protected override TRegistrationOptions CreateRegistrationOptions() => _registrationOptionsFactory();
        }

        public sealed class RequestCapability<TParams, TResult, TCapability> :
            AbstractHandlers.BaseCapability<TCapability>,
            IJsonRpcRequestHandler<TParams, TResult>,
            ICanBeIdentifiedHandler
            where TParams : IRequest<TResult>
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task<TResult>> _handler;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public RequestCapability(Func<TParams, TCapability, Task<TResult>> handler) :
                this(Guid.Empty, (a, c, _) => handler(a, c))
            {
            }

            public RequestCapability(Func<TParams, TCapability, CancellationToken, Task<TResult>> handler) :
                this(Guid.Empty, handler)
            {
            }

            public RequestCapability(Guid id, Func<TParams, TCapability, Task<TResult>> handler) :
                this(id, (a, c, _) => handler(a, c))
            {
            }

            public RequestCapability(Guid id, Func<TParams, TCapability, CancellationToken, Task<TResult>> handler)
            {
                _id = id;
                _handler = handler;
            }

            Task<TResult> IRequestHandler<TParams, TResult>.
                Handle(TParams request, CancellationToken cancellationToken) =>
                _handler(request, Capability, cancellationToken);
        }

        public sealed class RequestCapability<TParams, TCapability> :
            AbstractHandlers.BaseCapability<TCapability>,
            IJsonRpcRequestHandler<TParams>,
            ICanBeIdentifiedHandler
            where TParams : IRequest
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task> _handler;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public RequestCapability(Func<TParams, TCapability, Task> handler) :
                this(Guid.Empty, handler)
            {
            }

            public RequestCapability(Func<TParams, TCapability, CancellationToken, Task> handler) :
                this(Guid.Empty, handler)
            {
            }

            public RequestCapability(Guid id, Func<TParams, TCapability, Task> handler) :
                this(id, (a, c, _) => handler(a, c))
            {
            }

            public RequestCapability(Guid id, Func<TParams, TCapability, CancellationToken, Task> handler)
            {
                _id = id;
                _handler = handler;
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.
                Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, Capability, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }
        }

        public sealed class PartialResult<TParams, TResponse, TItem, TRegistrationOptions, TCapability> :
            AbstractHandlers.Base<TRegistrationOptions, TCapability>,
            IJsonRpcRequestHandler<TParams, TResponse>,
            ICanBeIdentifiedHandler
            where TParams : IPartialItemRequest<TResponse, TItem>
            where TResponse : new()
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Func<TItem, TResponse> _factory;
            private readonly Action<TParams, IObserver<TItem>, TCapability, CancellationToken> _handler;
            private readonly Func<TCapability, TRegistrationOptions> _registrationOptionsFactory;
            private readonly IProgressManager _progressManager;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResult(
                Action<TParams, IObserver<TItem>, TCapability> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory, IProgressManager progressManager, Func<TItem, TResponse> factory
            ) :
                this(Guid.Empty, (p, o, c, _) => handler(p, o, c), registrationOptionsFactory, progressManager, factory)
            {
            }

            public PartialResult(
                Action<TParams, IObserver<TItem>, TCapability, CancellationToken> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory, IProgressManager progressManager,
                Func<TItem, TResponse> factory
            ) :
                this(Guid.Empty, handler, registrationOptionsFactory, progressManager, factory)
            {
            }

            public PartialResult(
                Guid id, Action<TParams, IObserver<TItem>, TCapability> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory, IProgressManager progressManager,
                Func<TItem, TResponse> factory
            ) :
                this(id, (p, o, c, _) => handler(p, o, c), registrationOptionsFactory, progressManager, factory)
            {
            }

            public PartialResult(
                Guid id, Action<TParams, IObserver<TItem>, TCapability, CancellationToken> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory, IProgressManager progressManager,
                Func<TItem, TResponse> factory
            )
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TParams, TResponse>.Handle(
                TParams request,
                CancellationToken cancellationToken
            )
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TItem>.Noop)
                {
                    _handler(request, observer, Capability, cancellationToken);
                    await observer;
                    return new TResponse();
                }

                var subject = new AsyncSubject<TItem>();
                // in the event nothing is emitted...
                subject.OnNext(default!);
                _handler(request, subject, Capability, cancellationToken);
                return await subject.Select(_factory).ToTask(cancellationToken).ConfigureAwait(false);
            }

            protected override TRegistrationOptions CreateRegistrationOptions(TCapability capability) => _registrationOptionsFactory(capability);
        }

        public sealed class PartialResult<TParams, TResponse, TItem, TRegistrationOptions> :
            AbstractHandlers.Base<TRegistrationOptions>,
            IJsonRpcRequestHandler<TParams, TResponse>,
            ICanBeIdentifiedHandler
            where TParams : IPartialItemRequest<TResponse, TItem>
            where TResponse : new()
            where TRegistrationOptions : class, new()
        {
            private readonly Action<TParams, IObserver<TItem>, CancellationToken> _handler;
            private readonly Func<TRegistrationOptions> _registrationOptionsFactory;
            private readonly IProgressManager _progressManager;
            private readonly Func<TItem, TResponse> _factory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResult(
                Action<TParams, IObserver<TItem>> handler, Func< TRegistrationOptions> registrationOptionsFactory, IProgressManager progressManager, Func<TItem, TResponse> factory
            ) :
                this(Guid.Empty, (p, o, _) => handler(p, o), registrationOptionsFactory, progressManager, factory)
            {
            }

            public PartialResult(
                Action<TParams, IObserver<TItem>, CancellationToken> handler, Func< TRegistrationOptions> registrationOptionsFactory, IProgressManager progressManager,
                Func<TItem, TResponse> factory
            ) :
                this(Guid.Empty, handler, registrationOptionsFactory, progressManager, factory)
            {
            }

            public PartialResult(
                Guid id, Action<TParams, IObserver<TItem>> handler, Func< TRegistrationOptions> registrationOptionsFactory, IProgressManager progressManager, Func<TItem, TResponse> factory
            ) :
                this(id, (p, o, _) => handler(p, o), registrationOptionsFactory, progressManager, factory)
            {
            }

            public PartialResult(
                Guid id, Action<TParams, IObserver<TItem>, CancellationToken> handler, Func<TRegistrationOptions> registrationOptionsFactory, IProgressManager progressManager,
                Func<TItem, TResponse> factory
            )
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TParams, TResponse>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TItem>.Noop)
                {
                    _handler(request, observer, cancellationToken);
                    await observer;
                    return new TResponse();
                }

                var subject = new AsyncSubject<TItem>();
                // in the event nothing is emitted...
                subject.OnNext(default!);
                _handler(request, subject, cancellationToken);
                return await subject.Select(_factory).ToTask(cancellationToken).ConfigureAwait(false);
            }

            protected override TRegistrationOptions CreateRegistrationOptions() => _registrationOptionsFactory();
        }

        public sealed class PartialResultCapability<TParams, TResponse, TItem, TCapability> :
            AbstractHandlers.BaseCapability<TCapability>,
            IJsonRpcRequestHandler<TParams, TResponse>,
            ICanBeIdentifiedHandler
            where TParams : IPartialItemRequest<TResponse, TItem>
            where TResponse : new()
            where TCapability : ICapability
        {
            private readonly Action<TParams, TCapability, IObserver<TItem>, CancellationToken> _handler;
            private readonly IProgressManager _progressManager;
            private readonly Func<TItem, TResponse> _factory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResultCapability(Action<TParams, TCapability, IObserver<TItem>> handler, IProgressManager progressManager, Func<TItem, TResponse> factory) :
                this((p, c, o, _) => handler(p, c, o), progressManager, factory)
            {
            }

            public PartialResultCapability(
                Action<TParams, TCapability, IObserver<TItem>, CancellationToken> handler, IProgressManager progressManager, Func<TItem, TResponse> factory
            ) :
                this(Guid.Empty, handler, progressManager, factory)
            {
            }

            public PartialResultCapability(Guid id, Action<TParams, TCapability, IObserver<TItem>> handler, IProgressManager progressManager, Func<TItem, TResponse> factory) :
                this(id, (p, c, o, _) => handler(p, c, o), progressManager, factory)
            {
            }

            public PartialResultCapability(
                Guid id, Action<TParams, TCapability, IObserver<TItem>, CancellationToken> handler, IProgressManager progressManager, Func<TItem, TResponse> factory
            )
            {
                _id = id;
                _handler = handler;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TParams, TResponse>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TItem>.Noop)
                {
                    _handler(request, Capability, observer, cancellationToken);
                    await observer;
                    return new TResponse();
                }

                var subject = new AsyncSubject<TItem>();
                // in the event nothing is emitted...
                subject.OnNext(default!);
                _handler(request, Capability, subject, cancellationToken);
                return await subject.Select(_factory).ToTask(cancellationToken).ConfigureAwait(false);
            }
        }

        public sealed class PartialResult<TParams, TResponse, TItem> :
            IJsonRpcRequestHandler<TParams, TResponse>,
            ICanBeIdentifiedHandler
            where TParams : IPartialItemRequest<TResponse, TItem>
            where TResponse : new()
        {
            private readonly Action<TParams, IObserver<TItem>, CancellationToken> _handler;
            private readonly IProgressManager _progressManager;
            private readonly Func<TItem, TResponse> _factory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResult(Action<TParams, IObserver<TItem>> handler, IProgressManager progressManager, Func<TItem, TResponse> factory) :
                this(Guid.Empty, (p, o, _) => handler(p, o), progressManager, factory)
            {
            }

            public PartialResult(Action<TParams, IObserver<TItem>, CancellationToken> handler, IProgressManager progressManager, Func<TItem, TResponse> factory) :
                this(Guid.Empty, handler, progressManager, factory)
            {
                _handler = handler;
                _progressManager = progressManager;
                _factory = factory;
            }

            public PartialResult(Guid id, Action<TParams, IObserver<TItem>> handler, IProgressManager progressManager, Func<TItem, TResponse> factory) :
                this(id, (p, o, _) => handler(p, o), progressManager, factory)
            {
            }

            public PartialResult(Guid id, Action<TParams, IObserver<TItem>, CancellationToken> handler, IProgressManager progressManager, Func<TItem, TResponse> factory)
            {
                _id = id;
                _handler = handler;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TParams, TResponse>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TItem>.Noop)
                {
                    _handler(request, observer, cancellationToken);
                    await observer;
                    return new TResponse();
                }

                var subject = new AsyncSubject<TItem>();
                // in the event nothing is emitted...
                subject.OnNext(default!);
                _handler(request, subject, cancellationToken);
                return await subject.Select(_factory).ToTask(cancellationToken).ConfigureAwait(false);
            }
        }

        public sealed class PartialResults<TParams, TResponse, TItem, TRegistrationOptions, TCapability> :
            AbstractHandlers.Base<TRegistrationOptions, TCapability>,
            IJsonRpcRequestHandler<TParams, TResponse>,
            ICanBeIdentifiedHandler
            where TParams : IPartialItemsRequest<TResponse, TItem>
            where TResponse : IEnumerable<TItem>?, new()
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> _handler;
            private readonly Func<TCapability, TRegistrationOptions> _registrationOptionsFactory;
            private readonly IProgressManager _progressManager;
            private readonly Func<IEnumerable<TItem>, TResponse> _factory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResults(
                Action<TParams, IObserver<IEnumerable<TItem>>, TCapability> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory, IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse> factory
            ) :
                this(Guid.Empty, (p, o, c, _) => handler(p, o, c), registrationOptionsFactory, progressManager, factory)
            {
            }

            public PartialResults(
                Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory, IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse> factory
            ) :
                this(Guid.Empty, handler, registrationOptionsFactory, progressManager, factory)
            {
            }

            public PartialResults(
                Guid id, Action<TParams, IObserver<IEnumerable<TItem>>, TCapability> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory, IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse> factory
            ) :
                this(id, (p, o, c, _) => handler(p, o, c), registrationOptionsFactory, progressManager, factory)
            {
            }

            public PartialResults(
                Guid id, Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory,
                IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory
            )
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TParams, TResponse>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<IEnumerable<TItem>>.Noop)
                {
                    _handler(request, observer, Capability, cancellationToken);
                    await observer;
                    return new TResponse();
                }

                var subject = new Subject<IEnumerable<TItem>>();
                var task = subject.Aggregate(
                                       new List<TItem>(), (acc, items) => {
                                           acc.AddRange(items);
                                           return acc;
                                       }
                                   )
                                  .ToTask(cancellationToken);
                _handler(request, subject, Capability, cancellationToken);
                var result = _factory(await task.ConfigureAwait(false));
                return result;
            }

            protected override TRegistrationOptions CreateRegistrationOptions(TCapability capability) => _registrationOptionsFactory(capability);
        }

        public sealed class PartialResults<TParams, TResponse, TItem, TRegistrationOptions> :
            AbstractHandlers.Base<TRegistrationOptions>,
            IJsonRpcRequestHandler<TParams, TResponse>,
            ICanBeIdentifiedHandler
            where TParams : IPartialItemsRequest<TResponse, TItem>
            where TResponse : IEnumerable<TItem>?, new()
            where TRegistrationOptions : class, new()
        {
            private readonly Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> _handler;
            private readonly Func<TRegistrationOptions> _registrationOptionsFactory;
            private readonly IProgressManager _progressManager;
            private readonly Func<IEnumerable<TItem>, TResponse> _factory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResults(
                Action<TParams, IObserver<IEnumerable<TItem>>> handler, Func<TRegistrationOptions> registrationOptionsFactory, IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse> factory
            ) :
                this(Guid.Empty, (p, o, _) => handler(p, o), registrationOptionsFactory, progressManager, factory)
            {
            }

            public PartialResults(
                Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler, Func<TRegistrationOptions> registrationOptionsFactory, IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse> factory
            ) :
                this(Guid.Empty, handler, registrationOptionsFactory, progressManager, factory)
            {
            }

            public PartialResults(
                Guid id, Action<TParams, IObserver<IEnumerable<TItem>>> handler, Func<TRegistrationOptions> registrationOptionsFactory, IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse> factory
            ) :
                this(id, (p, o, _) => handler(p, o), registrationOptionsFactory, progressManager, factory)
            {
            }

            public PartialResults(
                Guid id, Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler, Func<TRegistrationOptions> registrationOptionsFactory, IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse> factory
            )
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TParams, TResponse>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<IEnumerable<TItem>>.Noop)
                {
                    _handler(request, observer, cancellationToken);
                    await observer;
                    return new TResponse();
                }

                var subject = new Subject<IEnumerable<TItem>>();
                var task = subject.Aggregate(
                                       new List<TItem>(), (acc, items) => {
                                           acc.AddRange(items);
                                           return acc;
                                       }
                                   )
                                  .ToTask(cancellationToken);
                _handler(request, subject, cancellationToken);
                var result = _factory(await task.ConfigureAwait(false));
                return result;
            }

            protected override TRegistrationOptions CreateRegistrationOptions() => _registrationOptionsFactory();
        }

        public sealed class PartialResultsCapability<TParams, TResponse, TItem, TCapability> :
            AbstractHandlers.BaseCapability<TCapability>,
            IJsonRpcRequestHandler<TParams, TResponse>,
            ICanBeIdentifiedHandler
            where TParams : IPartialItemsRequest<TResponse, TItem>
            where TResponse : IEnumerable<TItem>, new()
            where TCapability : ICapability
        {
            private readonly Action<TParams, TCapability, IObserver<IEnumerable<TItem>>, CancellationToken> _handler;
            private readonly IProgressManager _progressManager;
            private readonly Func<IEnumerable<TItem>, TResponse> _factory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResultsCapability(
                Action<TParams, TCapability, IObserver<IEnumerable<TItem>>> handler, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory
            ) :
                this(Guid.Empty, (p, c, o, _) => handler(p, c, o), progressManager, factory)
            {
            }

            public PartialResultsCapability(
                Action<TParams, TCapability, IObserver<IEnumerable<TItem>>, CancellationToken> handler, IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse> factory
            ) :
                this(Guid.Empty, handler, progressManager, factory)
            {
            }

            public PartialResultsCapability(
                Guid id, Action<TParams, TCapability, IObserver<IEnumerable<TItem>>> handler, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory
            ) :
                this(id, (p, c, o, _) => handler(p, c, o), progressManager, factory)
            {
            }

            public PartialResultsCapability(
                Guid id, Action<TParams, TCapability, IObserver<IEnumerable<TItem>>, CancellationToken> handler, IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse> factory
            )
            {
                _id = id;
                _handler = handler;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TParams, TResponse>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<IEnumerable<TItem>>.Noop)
                {
                    _handler(request, Capability, observer, cancellationToken);
                    await observer;
                    return new TResponse();
                }

                var subject = new Subject<IEnumerable<TItem>>();
                var task = subject.Aggregate(
                                       new List<TItem>(), (acc, items) => {
                                           acc.AddRange(items);
                                           return acc;
                                       }
                                   )
                                  .ToTask(cancellationToken);
                _handler(request, Capability, subject, cancellationToken);
                var result = _factory(await task.ConfigureAwait(false));
                return result;
            }
        }

        public sealed class PartialResults<TParams, TResponse, TItem> :
            IJsonRpcRequestHandler<TParams, TResponse>,
            ICanBeIdentifiedHandler
            where TParams : IPartialItemsRequest<TResponse, TItem>
            where TResponse : IEnumerable<TItem>, new()
        {
            private readonly Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> _handler;
            private readonly IProgressManager _progressManager;
            private readonly Func<IEnumerable<TItem>, TResponse> _factory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResults(Action<TParams, IObserver<IEnumerable<TItem>>> handler, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory) :
                this(Guid.Empty, (p, o, _) => handler(p, o), progressManager, factory)
            {
            }

            public PartialResults(
                Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory
            ) :
                this(Guid.Empty, handler, progressManager, factory)
            {
                _handler = handler;
                _progressManager = progressManager;
                _factory = factory;
            }

            public PartialResults(Guid id, Action<TParams, IObserver<IEnumerable<TItem>>> handler, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory) :
                this(id, (p, o, _) => handler(p, o), progressManager, factory)
            {
            }

            public PartialResults(
                Guid id, Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory
            )
            {
                _id = id;
                _handler = handler;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TParams, TResponse>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<IEnumerable<TItem>>.Noop)
                {
                    _handler(request, observer, cancellationToken);
                    await observer;
                    return new TResponse();
                }

                var subject = new Subject<IEnumerable<TItem>>();
                var task = subject.Aggregate(
                                       new List<TItem>(), (acc, items) => {
                                           acc.AddRange(items);
                                           return acc;
                                       }
                                   )
                                  .ToTask(cancellationToken);
                _handler(request, subject, cancellationToken);
                var result = _factory(await task.ConfigureAwait(false));
                return result;
            }
        }

        public sealed class Notification<TParams, TRegistrationOptions, TCapability> :
            AbstractHandlers.Base<TRegistrationOptions, TCapability>,
            IJsonRpcNotificationHandler<TParams>,
            ICanBeIdentifiedHandler
            where TParams : IRequest
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task> _handler;
            private readonly Func<TCapability, TRegistrationOptions> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public Notification(Action<TParams, TCapability> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory) :
                this(
                    Guid.Empty, (request, capability, _) => {
                        handler(request, capability);
                        return Task.CompletedTask;
                    }, registrationOptionsFactory
                )
            {
            }

            public Notification(Action<TParams, TCapability, CancellationToken> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory) :
                this(
                    Guid.Empty, (request, c, ct) => {
                        handler(request, c, ct);
                        return Task.CompletedTask;
                    }, registrationOptionsFactory
                )
            {
            }

            public Notification(Func<TParams, TCapability, Task> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory) :
                this(Guid.Empty, (request, capability, _) => handler(request, capability), registrationOptionsFactory)
            {
            }

            public Notification(Func<TParams, TCapability, CancellationToken, Task> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory) :
                this(Guid.Empty, handler, registrationOptionsFactory)
            {
            }

            public Notification(Guid id, Action<TParams, TCapability> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory) :
                this(
                    id, (request, capability, _) => {
                        handler(request, capability);
                        return Task.CompletedTask;
                    }, registrationOptionsFactory
                )
            {
            }

            public Notification(Guid id, Action<TParams, TCapability, CancellationToken> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory) :
                this(
                    id, (request, c, ct) => {
                        handler(request, c, ct);
                        return Task.CompletedTask;
                    }, registrationOptionsFactory
                )
            {
            }

            public Notification(Guid id, Func<TParams, TCapability, Task> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory) :
                this(id, (request, capability, _) => handler(request, capability), registrationOptionsFactory)
            {
            }

            public Notification(Guid id, Func<TParams, TCapability, CancellationToken, Task> handler, Func<TCapability, TRegistrationOptions> registrationOptionsFactory)
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, Capability, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }

            protected override TRegistrationOptions CreateRegistrationOptions(TCapability capability) => _registrationOptionsFactory(capability);
        }

        public sealed class Notification<TParams, TRegistrationOptions> :
            AbstractHandlers.Base<TRegistrationOptions>,
            IJsonRpcNotificationHandler<TParams>,
            ICanBeIdentifiedHandler
            where TParams : IRequest
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TParams, CancellationToken, Task> _handler;
            private readonly Func<TRegistrationOptions> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public Notification(Action<TParams> handler, Func<TRegistrationOptions> registrationOptionsFactory) :
                this(
                    Guid.Empty, (request, _) => {
                        handler(request);
                        return Task.CompletedTask;
                    }, registrationOptionsFactory
                )
            {
            }

            public Notification(Action<TParams, CancellationToken> handler, Func<TRegistrationOptions> registrationOptionsFactory) :
                this(
                    Guid.Empty, (request, ct) => {
                        handler(request, ct);
                        return Task.CompletedTask;
                    }, registrationOptionsFactory
                )
            {
            }

            public Notification(Func<TParams, Task> handler, Func<TRegistrationOptions> registrationOptionsFactory) :
                this(Guid.Empty, (request, _) => handler(request), registrationOptionsFactory)
            {
            }

            public Notification(Func<TParams, CancellationToken, Task> handler, Func<TRegistrationOptions> registrationOptionsFactory) :
                this(Guid.Empty, handler, registrationOptionsFactory)
            {
            }

            public Notification(Guid id, Action<TParams> handler, Func<TRegistrationOptions> registrationOptionsFactory) :
                this(
                    id, (request, _) => {
                        handler(request);
                        return Task.CompletedTask;
                    }, registrationOptionsFactory
                )
            {
            }

            public Notification(Guid id, Action<TParams, CancellationToken> handler, Func<TRegistrationOptions> registrationOptionsFactory) :
                this(
                    id, (request, ct) => {
                        handler(request, ct);
                        return Task.CompletedTask;
                    }, registrationOptionsFactory
                )
            {
            }

            public Notification(Guid id, Func<TParams, Task> handler, Func<TRegistrationOptions> registrationOptionsFactory) :
                this(id, (request, _) => handler(request), registrationOptionsFactory)
            {
            }

            public Notification(Guid id, Func<TParams, CancellationToken, Task> handler, Func<TRegistrationOptions> registrationOptionsFactory)
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }

            protected override TRegistrationOptions CreateRegistrationOptions() => _registrationOptionsFactory();
        }

        public sealed class NotificationCapability<TParams, TCapability> :
            AbstractHandlers.BaseCapability<TCapability>,
            IJsonRpcNotificationHandler<TParams>,
            ICanBeIdentifiedHandler
            where TParams : IRequest
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task> _handler;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public NotificationCapability(Action<TParams, TCapability> handler) :
                this(
                    Guid.Empty, (request, capability, _) => {
                        handler(request, capability);
                        return Task.CompletedTask;
                    }
                )
            {
            }

            public NotificationCapability(Func<TParams, TCapability, Task> handler) :
                this(Guid.Empty, (request, capability, _) => handler(request, capability))
            {
            }

            public NotificationCapability(Action<TParams, TCapability, CancellationToken> handler) :
                this(
                    Guid.Empty, (request, capability, ct) => {
                        handler(request, capability, ct);
                        return Task.CompletedTask;
                    }
                )
            {
            }


            public NotificationCapability(Func<TParams, TCapability, CancellationToken, Task> handler) :
                this(Guid.Empty, handler)
            {
            }

            public NotificationCapability(Guid id, Action<TParams, TCapability> handler) :
                this(
                    id, (request, capability, _) => {
                        handler(request, capability);
                        return Task.CompletedTask;
                    }
                )
            {
            }

            public NotificationCapability(Guid id, Func<TParams, TCapability, Task> handler) :
                this(id, (request, capability, _) => handler(request, capability))
            {
            }

            public NotificationCapability(Guid id, Func<TParams, TCapability, CancellationToken, Task> handler)
            {
                _id = id;
                _handler = handler;
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, Capability, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }
        }
    }
}
