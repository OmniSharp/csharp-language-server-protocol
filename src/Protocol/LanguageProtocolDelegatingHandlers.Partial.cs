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
    public static partial class LanguageProtocolDelegatingHandlers
    {
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
                Guid id, Action<TParams, IObserver<TItem>, TCapability, CancellationToken> handler,
                RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory, IProgressManager progressManager,
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
                Action<TParams, IObserver<TItem>, TCapability, CancellationToken> handler,
                RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory, IProgressManager progressManager,
                Func<TItem?, TResponse?> factory
            ) :
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
                var task = subject
                          .Select(_factory)
                          .ToTask(cancellationToken, _progressManager.Scheduler)
                          .ConfigureAwait(false);
                // in the event nothing is emitted...
                subject.OnNext(default!);
                _handler(request, subject, Capability, cancellationToken);
                return await task;
            }

            protected internal override TRegistrationOptions CreateRegistrationOptions(TCapability capability, ClientCapabilities clientCapabilities) =>
                _registrationOptionsFactory(capability, clientCapabilities);
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
                Action<TParams, IObserver<TItem>, CancellationToken> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory,
                IProgressManager progressManager,
                Func<TItem?, TResponse?> factory
            ) :
                this(Guid.Empty, handler, registrationOptionsFactory, progressManager, factory)
            {
            }

            public PartialResult(
                Guid id, Action<TParams, IObserver<TItem>, CancellationToken> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory,
                IProgressManager progressManager,
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
                var task = subject
                          .Select(_factory)
                          .ToTask(cancellationToken, _progressManager.Scheduler)
                          .ConfigureAwait(false);
                // in the event nothing is emitted...
                subject.OnNext(default!);
                _handler(request, subject, cancellationToken);
                return await task;
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
                var task = subject
                          .Select(_factory)
                          .ToTask(cancellationToken, _progressManager.Scheduler)
                          .ConfigureAwait(false);
                // in the event nothing is emitted...
                subject.OnNext(default!);
                _handler(request, Capability, subject, cancellationToken);
                return await task;
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
                var task = subject
                          .Select(_factory)
                          .ToTask(cancellationToken, _progressManager.Scheduler)
                          .ConfigureAwait(false);
                // in the event nothing is emitted...
                subject.OnNext(default!);
                _handler(request, subject, cancellationToken);
                return await task;
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
                Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> handler,
                RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory, IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse?> factory
            ) :
                this(Guid.Empty, handler, registrationOptionsFactory, progressManager, factory)
            {
            }

            public PartialResults(
                Guid id, Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> handler,
                RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory,
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
                var task = subject
                          .Aggregate(
                               new List<TItem>(), (acc, items) => {
                                   acc.AddRange(items);
                                   return acc;
                               }
                           )
                          .Select(_factory)
                          .ToTask(cancellationToken, _progressManager.Scheduler)
                          .ConfigureAwait(false);
                _handler(request, subject, Capability, cancellationToken);
                return await task;
            }

            protected internal override TRegistrationOptions CreateRegistrationOptions(TCapability capability, ClientCapabilities clientCapabilities) =>
                _registrationOptionsFactory(capability, clientCapabilities);
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
                Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory,
                IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse?> factory
            ) :
                this(Guid.Empty, handler, registrationOptionsFactory, progressManager, factory)
            {
            }

            public PartialResults(
                Guid id, Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory,
                IProgressManager progressManager,
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
                var task = subject
                          .Aggregate(
                               new List<TItem>(), (acc, items) => {
                                   acc.AddRange(items);
                                   return acc;
                               }
                           )
                          .Select(_factory)
                          .ToTask(cancellationToken, _progressManager.Scheduler)
                          .ConfigureAwait(false);
                _handler(request, subject, cancellationToken);
                return await task;
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
                var task = subject
                          .Aggregate(
                               new List<TItem>(), (acc, items) => {
                                   acc.AddRange(items);
                                   return acc;
                               }
                           )
                          .Select(_factory)
                          .ToTask(cancellationToken, _progressManager.Scheduler)
                          .ConfigureAwait(false);
                _handler(request, subject, Capability, cancellationToken);
                return await task;
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
                var task = subject
                          .Aggregate(
                               new List<TItem>(), (acc, items) => {
                                   acc.AddRange(items);
                                   return acc;
                               }
                           )
                          .Select(_factory)
                          .ToTask(cancellationToken, _progressManager.Scheduler)
                          .ConfigureAwait(false);
                _handler(request, subject, cancellationToken);
                return await task;
            }
        }
    }
}
