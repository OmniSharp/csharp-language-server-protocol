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
        public sealed class Request<TParams, TResult, TCapability, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams, TResult>,
            IRegistration<TRegistrationOptions>, ICapability<TCapability>,
            ICanBeIdentifiedHandler
            where TParams : IRequest<TResult>
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task<TResult>> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private TCapability _capability;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public Request(Func<TParams, TCapability, Task<TResult>> handler, TRegistrationOptions registrationOptions) :
                this(Guid.Empty, (a, c, ct) => handler(a, c), registrationOptions)
            {
            }

            public Request(Func<TParams, TCapability, CancellationToken, Task<TResult>> handler, TRegistrationOptions registrationOptions) :
                this(Guid.Empty, handler, registrationOptions)
            {
            }

            public Request(Guid id, Func<TParams, TCapability, Task<TResult>> handler, TRegistrationOptions registrationOptions) :
                this(id, (a, c, ct) => handler(a, c), registrationOptions)
            {
            }

            public Request(Guid id, Func<TParams, TCapability, CancellationToken, Task<TResult>> handler, TRegistrationOptions registrationOptions)
            {
                _id = id;
                _handler = handler;
                _registrationOptions = registrationOptions;
            }

            Task<TResult> IRequestHandler<TParams, TResult>.
                Handle(TParams request, CancellationToken cancellationToken) =>
                _handler(request, _capability, cancellationToken);

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }

        public sealed class CanBeResolved<TItem, TCapability, TRegistrationOptions> :
            IRegistration<TRegistrationOptions>,
            ICapability<TCapability>,
            ICanBeResolvedHandler<TItem>,
            ICanBeIdentifiedHandler
            where TItem : ICanBeResolved, IRequest<TItem>
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Func<TItem, TCapability, CancellationToken, Task<TItem>> _resolveHandler;
            private readonly TRegistrationOptions _registrationOptions;
            private TCapability _capability;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public CanBeResolved(Guid id, Func<TItem, TCapability, Task<TItem>> resolveHandler, TRegistrationOptions registrationOptions) :
                this(id, (a, c, ct) => resolveHandler(a, c), registrationOptions)
            {
            }

            public CanBeResolved(Guid id, Func<TItem, TCapability, CancellationToken, Task<TItem>> resolveHandler, TRegistrationOptions registrationOptions)
            {
                _resolveHandler = resolveHandler;
                _registrationOptions = registrationOptions;
                _id = id;
            }

            Task<TItem> IRequestHandler<TItem, TItem>.Handle(TItem request, CancellationToken cancellationToken) => _resolveHandler(request, _capability, cancellationToken);

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }

        public sealed class CanBeResolved<TItem, TRegistrationOptions> :
            IRegistration<TRegistrationOptions>,
            ICanBeResolvedHandler<TItem>,
            ICanBeIdentifiedHandler
            where TItem : ICanBeResolved, IRequest<TItem>
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TItem, CancellationToken, Task<TItem>> _resolveHandler;
            private readonly TRegistrationOptions _registrationOptions;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public CanBeResolved(Guid id, Func<TItem, Task<TItem>> resolveHandler, TRegistrationOptions registrationOptions) :
                this(id, (a, c) => resolveHandler(a), registrationOptions)
            {
            }

            public CanBeResolved(Guid id, Func<TItem, CancellationToken, Task<TItem>> resolveHandler, TRegistrationOptions registrationOptions)
            {
                _id = id;
                _resolveHandler = resolveHandler;
                _registrationOptions = registrationOptions;
            }

            Task<TItem> IRequestHandler<TItem, TItem>.Handle(TItem request, CancellationToken cancellationToken) => _resolveHandler(request, cancellationToken);

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
        }

        public sealed class Request<TParams, TCapability, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams>,
            IRegistration<TRegistrationOptions>, ICapability<TCapability>,
            ICanBeIdentifiedHandler
            where TParams : IRequest
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private TCapability _capability;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public Request(Func<TParams, TCapability, Task> handler, TRegistrationOptions registrationOptions) :
                this(Guid.Empty, (a, c, ct) => handler(a, c), registrationOptions)
            {
            }

            public Request(Func<TParams, TCapability, CancellationToken, Task> handler, TRegistrationOptions registrationOptions) :
                this(Guid.Empty, handler, registrationOptions)
            {
            }


            public Request(Guid id, Func<TParams, TCapability, Task> handler, TRegistrationOptions registrationOptions) :
                this(id, (a, c, ct) => handler(a, c), registrationOptions)
            {
            }

            public Request(Guid id, Func<TParams, TCapability, CancellationToken, Task> handler, TRegistrationOptions registrationOptions)
            {
                _id = id;
                _handler = handler;
                _registrationOptions = registrationOptions;
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, _capability, cancellationToken);
                return Unit.Value;
            }

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }

        public sealed class RequestRegistration<TParams, TResult, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams, TResult>,
            IRegistration<TRegistrationOptions>,
            ICanBeIdentifiedHandler
            where TParams : IRequest<TResult>
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TParams, CancellationToken, Task<TResult>> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public RequestRegistration(Func<TParams, Task<TResult>> handler, TRegistrationOptions registrationOptions) :
                this(Guid.Empty, (a, ct) => handler(a), registrationOptions)
            {
            }

            public RequestRegistration(Func<TParams, CancellationToken, Task<TResult>> handler, TRegistrationOptions registrationOptions) :
                this(Guid.Empty, handler, registrationOptions)
            {
            }

            public RequestRegistration(Guid id, Func<TParams, Task<TResult>> handler, TRegistrationOptions registrationOptions) :
                this(id, (a, ct) => handler(a), registrationOptions)
            {
            }

            public RequestRegistration(Guid id, Func<TParams, CancellationToken, Task<TResult>> handler, TRegistrationOptions registrationOptions)
            {
                _id = id;
                _handler = handler;
                _registrationOptions = registrationOptions;
            }

            Task<TResult> IRequestHandler<TParams, TResult>.Handle(TParams request, CancellationToken cancellationToken) => _handler(request, cancellationToken);

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
        }

        public sealed class RequestRegistration<TParams, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams>,
            IRegistration<TRegistrationOptions>,
            ICanBeIdentifiedHandler
            where TParams : IRequest
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TParams, CancellationToken, Task> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public RequestRegistration(Func<TParams, Task> handler, TRegistrationOptions registrationOptions) :
                this(Guid.Empty, (a, ct) => handler(a), registrationOptions)
            {
            }

            public RequestRegistration(Func<TParams, CancellationToken, Task> handler, TRegistrationOptions registrationOptions):
                this(Guid.Empty, handler, registrationOptions)
            {
            }

            public RequestRegistration(Guid id, Func<TParams, Task> handler, TRegistrationOptions registrationOptions) :
                this(id, (a, ct) => handler(a), registrationOptions)
            {
            }

            public RequestRegistration(Guid id, Func<TParams, CancellationToken, Task> handler, TRegistrationOptions registrationOptions)
            {
                _id = id;
                _handler = handler;
                _registrationOptions = registrationOptions;
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.
                Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, cancellationToken);
                return Unit.Value;
            }

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
        }

        public sealed class RequestCapability<TParams, TResult, TCapability> :
            IJsonRpcRequestHandler<TParams, TResult>,
            ICapability<TCapability>,
            ICanBeIdentifiedHandler
            where TParams : IRequest<TResult>
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task<TResult>> _handler;
            private TCapability _capability;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public RequestCapability(Func<TParams, TCapability, Task<TResult>> handler) :
                this(Guid.Empty, (a, c, ct) => handler(a, c))
            {
            }

            public RequestCapability(Func<TParams, TCapability, CancellationToken, Task<TResult>> handler):
                this(Guid.Empty, handler)
            {
            }

            public RequestCapability(Guid id, Func<TParams, TCapability, Task<TResult>> handler) :
                this(id, (a, c, ct) => handler(a, c))
            {
            }

            public RequestCapability(Guid id, Func<TParams, TCapability, CancellationToken, Task<TResult>> handler)
            {
                _id = id;
                _handler = handler;
            }

            Task<TResult> IRequestHandler<TParams, TResult>.
                Handle(TParams request, CancellationToken cancellationToken) =>
                _handler(request, _capability, cancellationToken);

            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }

        public sealed class RequestCapability<TParams, TCapability> :
            IJsonRpcRequestHandler<TParams>,
            ICapability<TCapability>,
            ICanBeIdentifiedHandler
            where TParams : IRequest
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task> _handler;
            private TCapability _capability;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public RequestCapability(Func<TParams, TCapability, Task> handler) :
                this(Guid.Empty, handler)
            {
            }

            public RequestCapability(Func<TParams, TCapability, CancellationToken, Task> handler):
                this(Guid.Empty, handler)
            {
            }

            public RequestCapability(Guid id, Func<TParams, TCapability, Task> handler) :
                this(id, (a, c, ct) => handler(a, c))
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
                await _handler(request, _capability, cancellationToken);
                return Unit.Value;
            }

            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }

        public sealed class PartialResult<TItem, TResponse, TCapability, TRegistrationOptions> :
            IJsonRpcRequestHandler<TItem, TResponse>,
            IRegistration<TRegistrationOptions>, ICapability<TCapability>,
            ICanBeIdentifiedHandler
            where TItem : IPartialItemRequest<TResponse, TItem>
            where TResponse : class, new()
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Func<TItem, TResponse> _factory;
            private readonly Action<TItem, IObserver<TItem>, TCapability, CancellationToken> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private readonly IProgressManager _progressManager;
            private TCapability _capability;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResult(Action<TItem, IObserver<TItem>, TCapability> handler, TRegistrationOptions registrationOptions, IProgressManager progressManager, Func<TItem, TResponse> factory) :
                this(Guid.Empty, (p, o, c, ct) => handler(p, o, c), registrationOptions, progressManager, factory)
            {
            }

            public PartialResult(Action<TItem, IObserver<TItem>, TCapability, CancellationToken> handler, TRegistrationOptions registrationOptions, IProgressManager progressManager, Func<TItem, TResponse> factory):
                this(Guid.Empty, handler, registrationOptions, progressManager, factory)
            {
            }

            public PartialResult(Guid id, Action<TItem, IObserver<TItem>, TCapability> handler, TRegistrationOptions registrationOptions, IProgressManager progressManager, Func<TItem, TResponse> factory) :
                this(id, (p, o, c, ct) => handler(p, o, c), registrationOptions, progressManager, factory)
            {
            }

            public PartialResult(Guid id, Action<TItem, IObserver<TItem>, TCapability, CancellationToken> handler, TRegistrationOptions registrationOptions, IProgressManager progressManager, Func<TItem, TResponse> factory)
            {
                _id = id;
                _handler = handler;
                _registrationOptions = registrationOptions;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TItem, TResponse>.Handle(TItem request,
                CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TItem>.Noop)
                {
                    _handler(request, observer, _capability, cancellationToken);
                    await observer;
                    return new TResponse();
                }

                var subject = new AsyncSubject<TItem>();
                _handler(request, subject, _capability, cancellationToken);
                return await subject.Select(_factory);
            }

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }

        public sealed class PartialResult<TItem, TResponse, TRegistrationOptions> :
            IJsonRpcRequestHandler<TItem, TResponse>,
            IRegistration<TRegistrationOptions>,
            ICanBeIdentifiedHandler
            where TItem : IPartialItemRequest<TResponse, TItem>
            where TResponse : class, new()
            where TRegistrationOptions : class, new()
        {
            private readonly Action<TItem, IObserver<TItem>, CancellationToken> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private readonly IProgressManager _progressManager;
            private readonly Func<TItem, TResponse> _factory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResult(Action<TItem, IObserver<TItem>> handler, TRegistrationOptions registrationOptions, IProgressManager progressManager, Func<TItem, TResponse> factory) :
                this(Guid.Empty, (p, o, ct) => handler(p, o), registrationOptions, progressManager, factory)
            {
            }

            public PartialResult(Action<TItem, IObserver<TItem>, CancellationToken> handler, TRegistrationOptions registrationOptions, IProgressManager progressManager, Func<TItem, TResponse> factory):
                this(Guid.Empty, handler, registrationOptions, progressManager, factory)
            {
            }

            public PartialResult(Guid id, Action<TItem, IObserver<TItem>> handler, TRegistrationOptions registrationOptions, IProgressManager progressManager, Func<TItem, TResponse> factory) :
                this(id, (p, o, ct) => handler(p, o), registrationOptions, progressManager, factory)
            {
            }

            public PartialResult(Guid id, Action<TItem, IObserver<TItem>, CancellationToken> handler, TRegistrationOptions registrationOptions, IProgressManager progressManager, Func<TItem, TResponse> factory)
            {
                _id = id;
                _handler = handler;
                _registrationOptions = registrationOptions;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TItem, TResponse>.Handle(TItem request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TItem>.Noop)
                {
                    _handler(request, observer, cancellationToken);
                    await observer;
                    return new TResponse();
                }

                var subject = new AsyncSubject<TItem>();
                _handler(request, subject, cancellationToken);
                return await subject.Select(_factory);
            }

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
        }

        public sealed class PartialResultCapability<TItem, TResponse, TCapability> :
            IJsonRpcRequestHandler<TItem, TResponse>,
            ICapability<TCapability>,
            ICanBeIdentifiedHandler
            where TItem : IPartialItemRequest<TResponse, TItem>
            where TResponse : class, new()
            where TCapability : ICapability
        {
            private readonly Action<TItem, TCapability, IObserver<TItem>, CancellationToken> _handler;
            private readonly IProgressManager _progressManager;
            private readonly Func<TItem, TResponse> _factory;
            private TCapability _capability;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResultCapability(Action<TItem, TCapability, IObserver<TItem>> handler, IProgressManager progressManager, Func<TItem, TResponse> factory) :
                this((p, c, o, ct) => handler(p, c, o), progressManager, factory)
            {
            }

            public PartialResultCapability(Action<TItem, TCapability, IObserver<TItem>, CancellationToken> handler, IProgressManager progressManager, Func<TItem, TResponse> factory):
                this(Guid.Empty, handler, progressManager, factory)
            {
            }

            public PartialResultCapability(Guid id, Action<TItem, TCapability, IObserver<TItem>> handler, IProgressManager progressManager, Func<TItem, TResponse> factory) :
                this(id, (p, c, o, ct) => handler(p, c, o), progressManager, factory)
            {
            }

            public PartialResultCapability(Guid id, Action<TItem, TCapability, IObserver<TItem>, CancellationToken> handler, IProgressManager progressManager, Func<TItem, TResponse> factory)
            {
                _id = id;
                _handler = handler;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TItem, TResponse>.Handle(TItem request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TItem>.Noop)
                {
                    _handler(request, _capability, observer, cancellationToken);
                    await observer;
                    return new TResponse();
                }

                var subject = new AsyncSubject<TItem>();
                _handler(request, _capability, subject, cancellationToken);
                return await subject.Select(_factory);
            }

            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }

        public sealed class PartialResult<TItem, TResponse> :
            IJsonRpcRequestHandler<TItem, TResponse>,
            ICanBeIdentifiedHandler
            where TItem : IPartialItemRequest<TResponse, TItem>
            where TResponse : class, new()
        {
            private readonly Action<TItem, IObserver<TItem>, CancellationToken> _handler;
            private readonly IProgressManager _progressManager;
            private readonly Func<TItem, TResponse> _factory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResult(Action<TItem, IObserver<TItem>> handler, IProgressManager progressManager, Func<TItem, TResponse> factory) :
                this(Guid.Empty, (p, o, ct) => handler(p, o), progressManager, factory)
            {
            }

            public PartialResult(Action<TItem, IObserver<TItem>, CancellationToken> handler, IProgressManager progressManager, Func<TItem, TResponse> factory):
                this(Guid.Empty, handler, progressManager,factory)
            {
                _handler = handler;
                _progressManager = progressManager;
                _factory = factory;
            }

            public PartialResult(Guid id, Action<TItem, IObserver<TItem>> handler, IProgressManager progressManager, Func<TItem, TResponse> factory) :
                this(id, (p, o, ct) => handler(p, o), progressManager, factory)
            {
            }

            public PartialResult(Guid id, Action<TItem, IObserver<TItem>, CancellationToken> handler, IProgressManager progressManager, Func<TItem, TResponse> factory)
            {
                _id = id;
                _handler = handler;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TItem, TResponse>.Handle(TItem request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TItem>.Noop)
                {
                    _handler(request, observer, cancellationToken);
                    await observer;
                    return new TResponse();
                }

                var subject = new AsyncSubject<TItem>();
                _handler(request, subject, cancellationToken);
                return await subject.Select(_factory);
            }
        }

        public sealed class PartialResults<TParams, TResponse, TItem, TCapability, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams, TResponse>,
            IRegistration<TRegistrationOptions>, ICapability<TCapability>,
            ICanBeIdentifiedHandler
            where TParams : IPartialItemsRequest<TResponse, TItem>
            where TResponse : IEnumerable<TItem>, new()
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private readonly IProgressManager _progressManager;
            private readonly Func<IEnumerable<TItem>, TResponse> _factory;
            private TCapability _capability;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResults(Action<TParams, IObserver<IEnumerable<TItem>>, TCapability> handler, TRegistrationOptions registrationOptions, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory) :
                this(Guid.Empty, (p, o, c, ct) => handler(p, o, c), registrationOptions, progressManager, factory)
            {
            }

            public PartialResults(Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> handler, TRegistrationOptions registrationOptions, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory):
                this(Guid.Empty, handler, registrationOptions, progressManager, factory)
            {
            }

            public PartialResults(Guid id, Action<TParams, IObserver<IEnumerable<TItem>>, TCapability> handler, TRegistrationOptions registrationOptions, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory) :
                this(id, (p, o, c, ct) => handler(p, o, c), registrationOptions, progressManager, factory)
            {
            }

            public PartialResults(Guid id, Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> handler, TRegistrationOptions registrationOptions, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory)
            {
                _id = id;
                _handler = handler;
                _registrationOptions = registrationOptions;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TParams, TResponse>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<IEnumerable<TItem>>.Noop)
                {
                    _handler(request, observer, _capability, cancellationToken);
                    await observer;
                    return new TResponse();
                }

                var subject = new Subject<IEnumerable<TItem>>();
                var task = subject.Aggregate(new List<TItem>(), (acc, items) => {
                        acc.AddRange(items);
                        return acc;
                    })
                    .ToTask(cancellationToken);
                _handler(request, subject, _capability, cancellationToken);
                var result = _factory(await task);
                return result;
            }

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }

        public sealed class PartialResults<TParams, TResponse, TItem, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams, TResponse>,
            IRegistration<TRegistrationOptions>,
            ICanBeIdentifiedHandler
            where TParams : IPartialItemsRequest<TResponse, TItem>
            where TResponse : IEnumerable<TItem>, new()
            where TRegistrationOptions : class, new()
        {
            private readonly Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private readonly IProgressManager _progressManager;
            private readonly Func<IEnumerable<TItem>, TResponse> _factory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResults(Action<TParams, IObserver<IEnumerable<TItem>>> handler, TRegistrationOptions registrationOptions, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory) :
                this(Guid.Empty, (p, o, ct) => handler(p, o), registrationOptions, progressManager, factory)
            {
            }

            public PartialResults(Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler, TRegistrationOptions registrationOptions, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory):
                this(Guid.Empty, handler, registrationOptions, progressManager, factory)
            {
            }

            public PartialResults(Guid id, Action<TParams, IObserver<IEnumerable<TItem>>> handler, TRegistrationOptions registrationOptions, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory) :
                this(id, (p, o, ct) => handler(p, o), registrationOptions, progressManager, factory)
            {
            }

            public PartialResults(Guid id, Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler, TRegistrationOptions registrationOptions, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory)
            {
                _id = id;
                _handler = handler;
                _registrationOptions = registrationOptions;
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
                var task = subject.Aggregate(new List<TItem>(), (acc, items) => {
                        acc.AddRange(items);
                        return acc;
                    })
                    .ToTask(cancellationToken);
                _handler(request, subject, cancellationToken);
                var result = _factory(await task);
                return result;
            }

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
        }

        public sealed class PartialResultsCapability<TParams, TResponse, TItem, TCapability> :
            IJsonRpcRequestHandler<TParams, TResponse>, ICapability<TCapability>,
            ICanBeIdentifiedHandler
            where TParams : IPartialItemsRequest<TResponse, TItem>
            where TResponse : IEnumerable<TItem>, new()
            where TCapability : ICapability
        {
            private readonly Action<TParams, TCapability, IObserver<IEnumerable<TItem>>, CancellationToken> _handler;
            private readonly IProgressManager _progressManager;
            private readonly Func<IEnumerable<TItem>, TResponse> _factory;
            private TCapability _capability;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public PartialResultsCapability(Action<TParams, TCapability, IObserver<IEnumerable<TItem>>> handler, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory) :
                this(Guid.Empty, (p, c, o, ct) => handler(p, c, o), progressManager, factory)
            {
            }

            public PartialResultsCapability(Action<TParams, TCapability, IObserver<IEnumerable<TItem>>, CancellationToken> handler, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory):
                this(Guid.Empty, handler, progressManager, factory)
            {
            }

            public PartialResultsCapability(Guid id, Action<TParams, TCapability, IObserver<IEnumerable<TItem>>> handler, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory) :
                this(id, (p, c, o, ct) => handler(p, c, o), progressManager, factory)
            {
            }

            public PartialResultsCapability(Guid id, Action<TParams, TCapability, IObserver<IEnumerable<TItem>>, CancellationToken> handler, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory)
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
                    _handler(request, _capability, observer, cancellationToken);
                    await observer;
                    return new TResponse();
                }

                var subject = new Subject<IEnumerable<TItem>>();
                var task = subject.Aggregate(new List<TItem>(), (acc, items) => {
                        acc.AddRange(items);
                        return acc;
                    })
                    .ToTask(cancellationToken);
                _handler(request, _capability, subject, cancellationToken);
                var result = _factory(await task);
                return result;
            }

            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
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
                this(Guid.Empty, (p, o, ct) => handler(p, o), progressManager, factory)
            {
            }

            public PartialResults(Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory):
                this(Guid.Empty, handler, progressManager, factory)
            {
                _handler = handler;
                _progressManager = progressManager;
                _factory = factory;
            }

            public PartialResults(Guid id, Action<TParams, IObserver<IEnumerable<TItem>>> handler, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory) :
                this(id, (p, o, ct) => handler(p, o), progressManager, factory)
            {
            }

            public PartialResults(Guid id, Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory)
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
                var task = subject.Aggregate(new List<TItem>(), (acc, items) => {
                        acc.AddRange(items);
                        return acc;
                    })
                    .ToTask(cancellationToken);
                _handler(request, subject, cancellationToken);
                var result = _factory(await task);
                return result;
            }
        }

        public sealed class Notification<TParams, TCapability, TRegistrationOptions> :
            IJsonRpcNotificationHandler<TParams>,
            IRegistration<TRegistrationOptions>, ICapability<TCapability>,
            ICanBeIdentifiedHandler
            where TParams : IRequest
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private TCapability _capability;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public Notification(Action<TParams, TCapability> handler, TRegistrationOptions registrationOptions) :
                this(Guid.Empty, (request, capability, ct) => { handler(request, capability); return Task.CompletedTask; }, registrationOptions)
            {
            }

            public Notification(Action<TParams, TCapability, CancellationToken> handler, TRegistrationOptions registrationOptions) :
                this(Guid.Empty, (request, c, ct) => { handler(request, c, ct); return Task.CompletedTask; }, registrationOptions)
            {
            }

            public Notification(Func<TParams, TCapability, Task> handler, TRegistrationOptions registrationOptions) :
                this(Guid.Empty, (request, capability, ct) => handler(request, capability), registrationOptions)
            {
            }

            public Notification(Func<TParams, TCapability, CancellationToken, Task> handler, TRegistrationOptions registrationOptions):
                this(Guid.Empty, handler, registrationOptions)
            {
            }

            public Notification(Guid id, Action<TParams, TCapability> handler, TRegistrationOptions registrationOptions) :
                this(id, (request, capability, ct) => { handler(request, capability); return Task.CompletedTask; }, registrationOptions)
            {
            }

            public Notification(                Guid id, Action<TParams, TCapability, CancellationToken> handler, TRegistrationOptions registrationOptions) :
                this(id, (request, c, ct) => { handler(request, c, ct); return Task.CompletedTask; }, registrationOptions)
            {
            }

            public Notification(Guid id, Func<TParams, TCapability, Task> handler, TRegistrationOptions registrationOptions) :
                this(id, (request, capability, ct) => handler(request, capability), registrationOptions)
            {
            }

            public Notification(Guid id, Func<TParams, TCapability, CancellationToken, Task> handler, TRegistrationOptions registrationOptions)
            {
                _id = id;
                _handler = handler;
                _registrationOptions = registrationOptions;
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, _capability, cancellationToken);
                return Unit.Value;
            }

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }

        public sealed class Notification<TParams, TRegistrationOptions> :
            IJsonRpcNotificationHandler<TParams>,
            IRegistration<TRegistrationOptions>,
            ICanBeIdentifiedHandler
            where TParams : IRequest
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TParams, CancellationToken, Task> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public Notification(Action<TParams> handler, TRegistrationOptions registrationOptions) :
                this(Guid.Empty, (request, ct) => { handler(request); return Task.CompletedTask; }, registrationOptions)
            {
            }

            public Notification(Action<TParams, CancellationToken> handler, TRegistrationOptions registrationOptions) :
                this(Guid.Empty, (request, ct) => { handler(request, ct); return Task.CompletedTask; }, registrationOptions)
            {
            }

            public Notification(Func<TParams, Task> handler, TRegistrationOptions registrationOptions) :
                this(Guid.Empty, (request, ct) => handler(request), registrationOptions)
            {
            }

            public Notification(Func<TParams, CancellationToken, Task> handler, TRegistrationOptions registrationOptions):
                this(Guid.Empty, handler, registrationOptions)
            {
            }

            public Notification(Guid id, Action<TParams> handler, TRegistrationOptions registrationOptions) :
                this(id,(request, ct) => { handler(request); return Task.CompletedTask; }, registrationOptions)
            {
            }

            public Notification(Guid id, Action<TParams, CancellationToken> handler, TRegistrationOptions registrationOptions) :
                this(id, (request, ct) => { handler(request, ct); return Task.CompletedTask; }, registrationOptions)
            {
            }

            public Notification(Guid id, Func<TParams, Task> handler, TRegistrationOptions registrationOptions) :
                this(id, (request, ct) => handler(request), registrationOptions)
            {
            }

            public Notification(Guid id, Func<TParams, CancellationToken, Task> handler, TRegistrationOptions registrationOptions)
            {
                _id = id;
                _handler = handler;
                _registrationOptions = registrationOptions;
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, cancellationToken);
                return Unit.Value;
            }

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
        }

        public sealed class NotificationCapability<TParams, TCapability> :
            IJsonRpcNotificationHandler<TParams>, ICapability<TCapability>,
            ICanBeIdentifiedHandler
            where TParams : IRequest
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task> _handler;
            private TCapability _capability;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public NotificationCapability(Action<TParams, TCapability> handler) :
                this(Guid.Empty, (request, capability, ct) => { handler(request, capability); return Task.CompletedTask; })
            {
            }

            public NotificationCapability(Func<TParams, TCapability, Task> handler) :
                this(Guid.Empty, (request, capability, ct) => handler(request, capability))
            {
            }

            public NotificationCapability(Func<TParams, TCapability, CancellationToken, Task> handler) :
                this(Guid.Empty, handler)
            {
            }

            public NotificationCapability(Guid id, Action<TParams, TCapability> handler) :
                this(id, (request, capability, ct) => { handler(request, capability); return Task.CompletedTask; })
            {
            }

            public NotificationCapability(Guid id, Func<TParams, TCapability, Task> handler) :
                this(id, (request, capability, ct) => handler(request, capability))
            {
            }

            public NotificationCapability(Guid id, Func<TParams, TCapability, CancellationToken, Task> handler)
            {
                _id = id;
                _handler = handler;
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, _capability, cancellationToken);
                return Unit.Value;
            }

            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }
    }
}
