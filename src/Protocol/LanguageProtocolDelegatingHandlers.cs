using System;
using System.Collections.Generic;
using System.Linq;
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
            private readonly RegistrationOptionsDelegate<TRegistrationOptions, TCapability> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public Request(Guid id, Func<TParams, TCapability, CancellationToken, Task<TResult>> handler, RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory)
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            public Request(Func<TParams, TCapability, CancellationToken, Task<TResult>> handler, RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory) :
                this(Guid.Empty, handler, registrationOptionsFactory)
            {
            }

            Task<TResult> IRequestHandler<TParams, TResult>.Handle(TParams request, CancellationToken cancellationToken) =>
                _handler(request, Capability, cancellationToken);

            protected internal override TRegistrationOptions CreateRegistrationOptions(TCapability capability, ClientCapabilities clientCapabilities) => _registrationOptionsFactory(capability, clientCapabilities);
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
                        private readonly RegistrationOptionsDelegate<TRegistrationOptions, TCapability> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public CanBeResolved(Guid id, Func<TItem, TCapability, CancellationToken, Task<TItem>> resolveHandler, RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory)
            {
                _resolveHandler = resolveHandler;
                _registrationOptionsFactory = registrationOptionsFactory;
                _id = id;
            }

            Task<TItem> IRequestHandler<TItem, TItem>.Handle(TItem request, CancellationToken cancellationToken) => _resolveHandler(request, Capability, cancellationToken);

            protected internal override TRegistrationOptions CreateRegistrationOptions(TCapability capability, ClientCapabilities clientCapabilities) => _registrationOptionsFactory(capability, clientCapabilities);
        }

        public sealed class CanBeResolved<TItem, TRegistrationOptions> :
            AbstractHandlers.Base<TRegistrationOptions>,
            ICanBeResolvedHandler<TItem>,
            ICanBeIdentifiedHandler
            where TItem : ICanBeResolved, IRequest<TItem>
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TItem, CancellationToken, Task<TItem>> _resolveHandler;
            private readonly RegistrationOptionsDelegate<TRegistrationOptions> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public CanBeResolved(Guid id, Func<TItem, CancellationToken, Task<TItem>> resolveHandler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory)
            {
                _id = id;
                _resolveHandler = resolveHandler;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            Task<TItem> IRequestHandler<TItem, TItem>.Handle(TItem request, CancellationToken cancellationToken) => _resolveHandler(request, cancellationToken);

            protected override TRegistrationOptions CreateRegistrationOptions(ClientCapabilities clientCapabilities) => _registrationOptionsFactory(clientCapabilities);
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
            private readonly RegistrationOptionsDelegate<TRegistrationOptions, TCapability> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public Request(Guid id, Func<TParams, TCapability, CancellationToken, Task> handler, RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory)
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            public Request(Func<TParams, TCapability, CancellationToken, Task> handler, RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory):
                 this(Guid.Empty, handler, registrationOptionsFactory)
            {
            }


            async Task<Unit> IRequestHandler<TParams, Unit>.Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, Capability, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }

            protected internal override TRegistrationOptions CreateRegistrationOptions(TCapability capability, ClientCapabilities clientCapabilities) => _registrationOptionsFactory(capability, clientCapabilities);
        }

        public sealed class RequestRegistration<TParams, TResult, TRegistrationOptions> :
            AbstractHandlers.Base<TRegistrationOptions>,
            IJsonRpcRequestHandler<TParams, TResult>,
            ICanBeIdentifiedHandler
            where TParams : IRequest<TResult>
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TParams, CancellationToken, Task<TResult>> _handler;
            private readonly RegistrationOptionsDelegate<TRegistrationOptions> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public RequestRegistration(Func<TParams, CancellationToken, Task<TResult>> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory) :
                this(Guid.Empty, handler, registrationOptionsFactory)
            {
            }

            public RequestRegistration(Guid id, Func<TParams, CancellationToken, Task<TResult>> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory)
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            Task<TResult> IRequestHandler<TParams, TResult>.Handle(TParams request, CancellationToken cancellationToken) => _handler(request, cancellationToken);


            protected override TRegistrationOptions CreateRegistrationOptions(ClientCapabilities clientCapabilities) => _registrationOptionsFactory(clientCapabilities);
        }

        public sealed class RequestRegistration<TParams, TRegistrationOptions> :
            AbstractHandlers.Base<TRegistrationOptions>,
            IJsonRpcRequestHandler<TParams>,
            ICanBeIdentifiedHandler
            where TParams : IRequest
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TParams, CancellationToken, Task> _handler;
            private readonly RegistrationOptionsDelegate<TRegistrationOptions> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public RequestRegistration(Guid id, Func<TParams, CancellationToken, Task> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory)
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            public RequestRegistration(Func<TParams, CancellationToken, Task> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory):
                this(Guid.Empty, handler, registrationOptionsFactory)
            {
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.
                Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }

            protected override TRegistrationOptions CreateRegistrationOptions(ClientCapabilities clientCapabilities) => _registrationOptionsFactory(clientCapabilities);
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

            public RequestCapability(Func<TParams, TCapability, CancellationToken, Task<TResult>> handler) :
                this(Guid.Empty, handler)
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

            public RequestCapability(Func<TParams, TCapability, CancellationToken, Task> handler) :
                this(Guid.Empty, handler)
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
            IJsonRpcRequestHandler<TParams, TResponse?>,
            ICanBeIdentifiedHandler
            where TParams : IPartialItemRequest<TResponse?, TItem>
            where TItem : class?
            where TResponse : class?
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Func<TItem?, TResponse?> _factory;
            private readonly Action<TParams, IObserver<TItem>, TCapability, CancellationToken> _handler;
            private readonly RegistrationOptionsDelegate<TRegistrationOptions, TCapability> _registrationOptionsFactory;
            private readonly IProgressManager _progressManager;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResult(
                Guid id, Action<TParams, IObserver<TItem>, TCapability, CancellationToken> handler, RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory, IProgressManager progressManager,
                Func<TItem?, TResponse?> factory
            )
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
                _progressManager = progressManager;
                _factory = factory;
            }

            public PartialResult(
                Action<TParams, IObserver<TItem>, TCapability, CancellationToken> handler, RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory, IProgressManager progressManager,
                Func<TItem?, TResponse?> factory
            ):
                this(Guid.Empty, handler, registrationOptionsFactory, progressManager, factory)
            {
            }

            async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(
                TParams request,
                CancellationToken cancellationToken
            )
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TItem>.Noop)
                {
                    _handler(request, observer, Capability, cancellationToken);
                    await observer;
                    return _factory(default);
                }

                var subject = new AsyncSubject<TItem?>();
                // in the event nothing is emitted...
                subject.OnNext(default!);
                _handler(request, subject, Capability, cancellationToken);
                return await subject.Select(_factory).ToTask(cancellationToken, _progressManager.Scheduler).ConfigureAwait(false);
            }

            protected internal override TRegistrationOptions CreateRegistrationOptions(TCapability capability, ClientCapabilities clientCapabilities) => _registrationOptionsFactory(capability, clientCapabilities);
        }

        public sealed class PartialResult<TParams, TResponse, TItem, TRegistrationOptions> :
            AbstractHandlers.Base<TRegistrationOptions>,
            IJsonRpcRequestHandler<TParams, TResponse?>,
            ICanBeIdentifiedHandler
            where TParams : IPartialItemRequest<TResponse, TItem>
            where TItem : class?
            where TResponse : class?
            where TRegistrationOptions : class, new()
        {
            private readonly Action<TParams, IObserver<TItem>, CancellationToken> _handler;
            private readonly RegistrationOptionsDelegate<TRegistrationOptions> _registrationOptionsFactory;
            private readonly IProgressManager _progressManager;
            private readonly Func<TItem?, TResponse?> _factory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResult(
                Action<TParams, IObserver<TItem>, CancellationToken> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory, IProgressManager progressManager,
                Func<TItem?, TResponse?> factory
            ) :
                this(Guid.Empty, handler, registrationOptionsFactory, progressManager, factory)
            {
            }
            public PartialResult(
                Guid id, Action<TParams, IObserver<TItem>, CancellationToken> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory, IProgressManager progressManager,
                Func<TItem?, TResponse?> factory
            )
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TItem>.Noop)
                {
                    _handler(request, observer, cancellationToken);
                    await observer;
                    return _factory(default);
                }

                var subject = new AsyncSubject<TItem?>();
                // in the event nothing is emitted...
                subject.OnNext(default!);
                _handler(request, subject, cancellationToken);
                return await subject.Select(_factory).ToTask(cancellationToken, _progressManager.Scheduler).ConfigureAwait(false);
            }

            protected override TRegistrationOptions CreateRegistrationOptions(ClientCapabilities clientCapabilities) => _registrationOptionsFactory(clientCapabilities);
        }

        public sealed class PartialResultCapability<TParams, TResponse, TItem, TCapability> :
            AbstractHandlers.BaseCapability<TCapability>,
            IJsonRpcRequestHandler<TParams, TResponse?>,
            ICanBeIdentifiedHandler
            where TParams : IPartialItemRequest<TResponse, TItem>
            where TItem : class?
            where TResponse : class?
            where TCapability : ICapability
        {
            private readonly Action<TParams, TCapability, IObserver<TItem>, CancellationToken> _handler;
            private readonly IProgressManager _progressManager;
            private readonly Func<TItem?, TResponse?> _factory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResultCapability(
                Action<TParams, TCapability, IObserver<TItem>, CancellationToken> handler, IProgressManager progressManager, Func<TItem?, TResponse?> factory
            ) :
                this(Guid.Empty, handler, progressManager, factory)
            {
            }

            public PartialResultCapability(
                Guid id, Action<TParams, TCapability, IObserver<TItem>, CancellationToken> handler, IProgressManager progressManager, Func<TItem?, TResponse?> factory
            )
            {
                _id = id;
                _handler = handler;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TItem>.Noop)
                {
                    _handler(request, Capability, observer, cancellationToken);
                    await observer;
                    return _factory(default);
                }

                var subject = new AsyncSubject<TItem?>();
                // in the event nothing is emitted...
                subject.OnNext(default!);
                _handler(request, Capability, subject, cancellationToken);
                return await subject.Select(_factory).ToTask(cancellationToken, _progressManager.Scheduler).ConfigureAwait(false);
            }
        }

        public sealed class PartialResult<TParams, TResponse, TItem> :
            IJsonRpcRequestHandler<TParams, TResponse?>,
            ICanBeIdentifiedHandler
            where TParams : IPartialItemRequest<TResponse, TItem>
            where TItem : class?
            where TResponse : class?
        {
            private readonly Action<TParams, IObserver<TItem>, CancellationToken> _handler;
            private readonly IProgressManager _progressManager;
            private readonly Func<TItem?, TResponse?> _factory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResult(Action<TParams, IObserver<TItem>, CancellationToken> handler, IProgressManager progressManager, Func<TItem?, TResponse?> factory) :
                this(Guid.Empty, handler, progressManager, factory)
            {
            }

            public PartialResult(Guid id, Action<TParams, IObserver<TItem>, CancellationToken> handler, IProgressManager progressManager, Func<TItem?, TResponse?> factory)
            {
                _id = id;
                _handler = handler;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TItem>.Noop)
                {
                    _handler(request, observer, cancellationToken);
                    await observer;
                    return _factory(default);
                }

                var subject = new AsyncSubject<TItem?>();
                // in the event nothing is emitted...
                subject.OnNext(default!);
                _handler(request, subject, cancellationToken);
                return await subject.Select(_factory).ToTask(cancellationToken, _progressManager.Scheduler).ConfigureAwait(false);
            }
        }

        public sealed class PartialResults<TParams, TResponse, TItem, TRegistrationOptions, TCapability> :
            AbstractHandlers.Base<TRegistrationOptions, TCapability>,
            IJsonRpcRequestHandler<TParams, TResponse?>,
            ICanBeIdentifiedHandler
            where TParams : IPartialItemsRequest<TResponse, TItem>
            where TItem : class?
            where TResponse : IEnumerable<TItem>?
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> _handler;
            private readonly RegistrationOptionsDelegate<TRegistrationOptions, TCapability> _registrationOptionsFactory;
            private readonly IProgressManager _progressManager;
            private readonly Func<IEnumerable<TItem>, TResponse?> _factory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResults(
                Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> handler, RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory, IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse?> factory
            ) :
                this(Guid.Empty, handler, registrationOptionsFactory, progressManager, factory)
            {
            }

            public PartialResults(
                Guid id, Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> handler, RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory,
                IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse?> factory
            )
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<IEnumerable<TItem>>.Noop)
                {
                    _handler(request, observer, Capability, cancellationToken);
                    await observer;
                    return _factory(Enumerable.Empty<TItem>());
                }

                var subject = new Subject<IEnumerable<TItem>>();
                var task = subject.Aggregate(
                                       new List<TItem>(), (acc, items) => {
                                           acc.AddRange(items);
                                           return acc;
                                       }
                                   )
                                  .ToTask(cancellationToken, _progressManager.Scheduler);
                _handler(request, subject, Capability, cancellationToken);
                var result = _factory(await task.ConfigureAwait(false));
                return result;
            }

            protected internal override TRegistrationOptions CreateRegistrationOptions(TCapability capability, ClientCapabilities clientCapabilities) => _registrationOptionsFactory(capability, clientCapabilities);
        }

        public sealed class PartialResults<TParams, TResponse, TItem, TRegistrationOptions> :
            AbstractHandlers.Base<TRegistrationOptions>,
            IJsonRpcRequestHandler<TParams, TResponse?>,
            ICanBeIdentifiedHandler
            where TParams : IPartialItemsRequest<TResponse, TItem>
            where TItem : class?
            where TResponse : IEnumerable<TItem>?
            where TRegistrationOptions : class, new()
        {
            private readonly Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> _handler;
            private readonly RegistrationOptionsDelegate<TRegistrationOptions> _registrationOptionsFactory;
            private readonly IProgressManager _progressManager;
            private readonly Func<IEnumerable<TItem>, TResponse?> _factory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResults(
                Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory, IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse?> factory
            ) :
                this(Guid.Empty, handler, registrationOptionsFactory, progressManager, factory)
            {
            }

            public PartialResults(
                Guid id, Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory, IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse?> factory
            )
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<IEnumerable<TItem>>.Noop)
                {
                    _handler(request, observer, cancellationToken);
                    await observer;
                    return _factory(Enumerable.Empty<TItem>());
                }

                var subject = new Subject<IEnumerable<TItem>>();
                var task = subject.Aggregate(
                                       new List<TItem>(), (acc, items) => {
                                           acc.AddRange(items);
                                           return acc;
                                       }
                                   )
                                  .ToTask(cancellationToken, _progressManager.Scheduler);
                _handler(request, subject, cancellationToken);
                var result = _factory(await task.ConfigureAwait(false));
                return result;
            }

            protected override TRegistrationOptions CreateRegistrationOptions(ClientCapabilities clientCapabilities) => _registrationOptionsFactory(clientCapabilities);
        }

        public sealed class PartialResultsCapability<TParams, TResponse, TItem, TCapability> :
            AbstractHandlers.BaseCapability<TCapability>,
            IJsonRpcRequestHandler<TParams, TResponse?>,
            ICanBeIdentifiedHandler
            where TParams : IPartialItemsRequest<TResponse, TItem>
            where TItem : class?
            where TResponse : IEnumerable<TItem>?
            where TCapability : ICapability
        {
            private readonly Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> _handler;
            private readonly IProgressManager _progressManager;
            private readonly Func<IEnumerable<TItem>, TResponse?> _factory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResultsCapability(
                Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> handler, IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse?> factory
            ) :
                this(Guid.Empty, handler, progressManager, factory)
            {
            }

            public PartialResultsCapability(
                Guid id, Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> handler, IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse?> factory
            )
            {
                _id = id;
                _handler = handler;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<IEnumerable<TItem>>.Noop)
                {
                    _handler(request, observer, Capability, cancellationToken);
                    await observer;
                    return _factory(Enumerable.Empty<TItem>());
                }

                var subject = new Subject<IEnumerable<TItem>>();
                var task = subject.Aggregate(
                                       new List<TItem>(), (acc, items) => {
                                           acc.AddRange(items);
                                           return acc;
                                       }
                                   )
                                  .ToTask(cancellationToken, _progressManager.Scheduler);
                _handler(request, subject, Capability, cancellationToken);
                var result = _factory(await task.ConfigureAwait(false));
                return result;
            }
        }

        public sealed class PartialResults<TParams, TResponse, TItem> :
            IJsonRpcRequestHandler<TParams, TResponse?>,
            ICanBeIdentifiedHandler
            where TParams : IPartialItemsRequest<TResponse, TItem>
            where TItem : class?
            where TResponse : IEnumerable<TItem>?
        {
            private readonly Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> _handler;
            private readonly IProgressManager _progressManager;
            private readonly Func<IEnumerable<TItem>, TResponse?> _factory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResults(
                Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse?> factory
            ) :
                this(Guid.Empty, handler, progressManager, factory)
            {
                _handler = handler;
                _progressManager = progressManager;
                _factory = factory;
            }

            public PartialResults(
                Guid id, Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse?> factory
            )
            {
                _id = id;
                _handler = handler;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<IEnumerable<TItem>>.Noop)
                {
                    _handler(request, observer, cancellationToken);
                    await observer;
                    return _factory(Enumerable.Empty<TItem>());
                }

                var subject = new Subject<IEnumerable<TItem>>();
                var task = subject.Aggregate(
                                       new List<TItem>(), (acc, items) => {
                                           acc.AddRange(items);
                                           return acc;
                                       }
                                   )
                                  .ToTask(cancellationToken, _progressManager.Scheduler);
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
            private readonly RegistrationOptionsDelegate<TRegistrationOptions, TCapability> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public Notification(Guid id, Func<TParams, TCapability, CancellationToken, Task> handler, RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory)
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            public Notification(Func<TParams, TCapability, CancellationToken, Task> handler, RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory) :
                this(Guid.Empty, handler, registrationOptionsFactory)
            {
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, Capability, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }

            protected internal override TRegistrationOptions CreateRegistrationOptions(TCapability capability, ClientCapabilities clientCapabilities) => _registrationOptionsFactory(capability, clientCapabilities);
        }

        public sealed class Notification<TParams, TRegistrationOptions> :
            AbstractHandlers.Base<TRegistrationOptions>,
            IJsonRpcNotificationHandler<TParams>,
            ICanBeIdentifiedHandler
            where TParams : IRequest
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TParams, CancellationToken, Task> _handler;
            private readonly RegistrationOptionsDelegate<TRegistrationOptions> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public Notification(Guid id, Func<TParams, CancellationToken, Task> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory)
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            public Notification(Func<TParams, CancellationToken, Task> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory) :
                this(Guid.Empty, handler, registrationOptionsFactory)
            {
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }

            protected override TRegistrationOptions CreateRegistrationOptions(ClientCapabilities clientCapabilities) => _registrationOptionsFactory(clientCapabilities);
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

            public NotificationCapability(Guid id, Func<TParams, TCapability, CancellationToken, Task> handler)
            {
                _id = id;
                _handler = handler;
            }

            public NotificationCapability(Func<TParams, TCapability, CancellationToken, Task> handler) :
                this(Guid.Empty, handler)
            {
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, Capability, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }
        }

        public sealed class TypedPartialObserver<T, TR> : IObserver<IEnumerable<T>>
        {
            private readonly IObserver<IEnumerable<TR>> _results;
            private readonly Func<T, TR> _factory;

            public TypedPartialObserver(IObserver<IEnumerable<TR>> results, Func<T, TR> factory)
            {
                _results = results;
                _factory = factory;
            }
            public void OnCompleted() => _results.OnCompleted();

            public void OnError(Exception error) => _results.OnError(error);

            public void OnNext(IEnumerable<T> value) => _results.OnNext(value.Select(_factory));
        }
    }
}
