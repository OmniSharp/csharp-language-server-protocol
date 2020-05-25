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
            IRegistration<TRegistrationOptions>, ICapability<TCapability>
            where TParams : IRequest<TResult>
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task<TResult>> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private TCapability _capability;

            public Request(
                Func<TParams, TCapability, Task<TResult>> handler,
                TRegistrationOptions registrationOptions) : this((a, c, ct) => handler(a, c), registrationOptions)
            {
            }

            public Request(
                Func<TParams, TCapability, CancellationToken, Task<TResult>> handler,
                TRegistrationOptions registrationOptions)
            {
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
            ICanBeResolvedHandler<TItem>
            where TItem : ICanBeResolved, IRequest<TItem>
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Func<TItem, bool> _canResolve;
            private readonly Func<TItem, TCapability, CancellationToken, Task<TItem>> _resolveHandler;
            private readonly TRegistrationOptions _registrationOptions;
            private TCapability _capability;

            public CanBeResolved(
                Func<TItem, TCapability, Task<TItem>> resolveHandler,
                Func<TItem, bool> canResolve,
                TRegistrationOptions registrationOptions) : this(
                (a, c, ct) => resolveHandler(a, c) ,
                canResolve,
                registrationOptions)
            {
            }

            public CanBeResolved(
                Func<TItem, TCapability, CancellationToken, Task<TItem>> resolveHandler,
                Func<TItem, bool> canResolve,
                TRegistrationOptions registrationOptions)
            {
                _canResolve = canResolve;
                _resolveHandler = resolveHandler;
                _registrationOptions = registrationOptions;
            }

            Task<TItem> IRequestHandler<TItem, TItem>.
                Handle(TItem request, CancellationToken cancellationToken) =>
                _resolveHandler(request, _capability, cancellationToken);

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
            bool ICanBeResolvedHandler<TItem>.CanResolve(TItem value) => _canResolve(value);
        }

        public sealed class CanBeResolved<TItem, TRegistrationOptions> :
            IRegistration<TRegistrationOptions>,
            ICanBeResolvedHandler<TItem>
            where TItem : ICanBeResolved, IRequest<TItem>
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TItem, bool> _canResolve;
            private readonly Func<TItem, CancellationToken, Task<TItem>> _resolveHandler;
            private readonly TRegistrationOptions _registrationOptions;

            public CanBeResolved(
                Func<TItem, Task<TItem>> resolveHandler,
                Func<TItem, bool> canResolve,
                TRegistrationOptions registrationOptions) : this(
                (a, c) => resolveHandler(a) ,
                canResolve,
                registrationOptions)
            {
            }

            public CanBeResolved(
                Func<TItem, CancellationToken, Task<TItem>> resolveHandler,
                Func<TItem, bool> canResolve,
                TRegistrationOptions registrationOptions)
            {
                _canResolve = canResolve;
                _resolveHandler = resolveHandler;
                _registrationOptions = registrationOptions;
            }

            Task<TItem> IRequestHandler<TItem, TItem>.
                Handle(TItem request, CancellationToken cancellationToken) =>
                _resolveHandler(request, cancellationToken);

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            bool ICanBeResolvedHandler<TItem>.CanResolve(TItem value) => _canResolve(value);
        }

        public sealed class Request<TParams, TCapability, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams>,
            IRegistration<TRegistrationOptions>, ICapability<TCapability>
            where TParams : IRequest
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private TCapability _capability;

            public Request(
                Func<TParams, TCapability, Task> handler,
                TRegistrationOptions registrationOptions) : this((a, c, ct) => handler(a, c), registrationOptions)
            {
            }

            public Request(
                Func<TParams, TCapability, CancellationToken, Task> handler,
                TRegistrationOptions registrationOptions)
            {
                _handler = handler;
                _registrationOptions = registrationOptions;
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.
                Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, _capability, cancellationToken);
                return Unit.Value;
                ;
            }

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }

        public sealed class RequestRegistration<TParams, TResult, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams, TResult>,
            IRegistration<TRegistrationOptions>
            where TParams : IRequest<TResult>
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TParams, CancellationToken, Task<TResult>> _handler;
            private readonly TRegistrationOptions _registrationOptions;

            public RequestRegistration(
                Func<TParams, Task<TResult>> handler,
                TRegistrationOptions registrationOptions) : this((a, ct) => handler(a), registrationOptions)
            {
            }

            public RequestRegistration(
                Func<TParams, CancellationToken, Task<TResult>> handler,
                TRegistrationOptions registrationOptions)
            {
                _handler = handler;
                _registrationOptions = registrationOptions;
            }

            Task<TResult> IRequestHandler<TParams, TResult>.
                Handle(TParams request, CancellationToken cancellationToken) =>
                _handler(request, cancellationToken);

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
        }

        public sealed class RequestRegistration<TParams, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams>,
            IRegistration<TRegistrationOptions>
            where TParams : IRequest
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TParams, CancellationToken, Task> _handler;
            private readonly TRegistrationOptions _registrationOptions;

            public RequestRegistration(
                Func<TParams, Task> handler,
                TRegistrationOptions registrationOptions) : this((a, ct) => handler(a), registrationOptions)
            {
            }

            public RequestRegistration(
                Func<TParams, CancellationToken, Task> handler,
                TRegistrationOptions registrationOptions)
            {
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
            ICapability<TCapability>
            where TParams : IRequest<TResult>
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task<TResult>> _handler;
            private TCapability _capability;

            public RequestCapability(
                Func<TParams, TCapability, Task<TResult>> handler) : this((a, c, ct) => handler(a, c))
            {
            }

            public RequestCapability(
                Func<TParams, TCapability, CancellationToken, Task<TResult>> handler)
            {
                _handler = handler;
            }

            Task<TResult> IRequestHandler<TParams, TResult>.
                Handle(TParams request, CancellationToken cancellationToken) =>
                _handler(request, _capability, cancellationToken);

            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }

        public sealed class RequestCapability<TParams, TCapability> :
            IJsonRpcRequestHandler<TParams>,
            ICapability<TCapability>
            where TParams : IRequest
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task> _handler;
            private TCapability _capability;

            public RequestCapability(
                Func<TParams, TCapability, Task> handler) : this((a, c, ct) => handler(a, c))
            {
            }

            public RequestCapability(
                Func<TParams, TCapability, CancellationToken, Task> handler)
            {
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
            IRegistration<TRegistrationOptions>, ICapability<TCapability>
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

            public PartialResult(
                Action<TItem, IObserver<TItem>, TCapability> handler,
                TRegistrationOptions registrationOptions,
                IProgressManager progressManager,
                Func<TItem, TResponse> factory) : this((p, o, c, ct) => handler(p, o, c), registrationOptions,
                progressManager, factory)
            {
            }

            public PartialResult(
                Action<TItem, IObserver<TItem>, TCapability, CancellationToken> handler,
                TRegistrationOptions registrationOptions,
                IProgressManager progressManager,
                Func<TItem, TResponse> factory)
            {
                _handler = handler;
                _registrationOptions = registrationOptions;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TItem, TResponse>.Handle(TItem request,
                CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != null)
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
            IRegistration<TRegistrationOptions>
            where TItem : IPartialItemRequest<TResponse, TItem>
            where TResponse : class, new()
            where TRegistrationOptions : class, new()
        {
            private readonly Action<TItem, IObserver<TItem>, CancellationToken> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private readonly IProgressManager _progressManager;
            private readonly Func<TItem, TResponse> _factory;

            public PartialResult(
                Action<TItem, IObserver<TItem>> handler,
                TRegistrationOptions registrationOptions,
                IProgressManager progressManager,
                Func<TItem, TResponse> factory) : this((p, o, ct) => handler(p, o), registrationOptions,
                progressManager, factory)
            {
            }

            public PartialResult(
                Action<TItem, IObserver<TItem>, CancellationToken> handler,
                TRegistrationOptions registrationOptions,
                IProgressManager progressManager,
                Func<TItem, TResponse> factory)
            {
                _handler = handler;
                _registrationOptions = registrationOptions;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TItem, TResponse>.Handle(TItem request,
                CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != null)
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
            ICapability<TCapability>
            where TItem : IPartialItemRequest<TResponse, TItem>
            where TResponse : class, new()
            where TCapability : ICapability
        {
            private readonly Action<TItem, TCapability, IObserver<TItem>, CancellationToken> _handler;
            private readonly IProgressManager _progressManager;
            private readonly Func<TItem, TResponse> _factory;
            private TCapability _capability;

            public PartialResultCapability(
                Action<TItem, TCapability, IObserver<TItem>> handler,
                IProgressManager progressManager,
                Func<TItem, TResponse> factory) : this((p, c, o, ct) => handler(p, c, o),
                progressManager, factory)
            {
            }

            public PartialResultCapability(
                Action<TItem, TCapability, IObserver<TItem>, CancellationToken> handler,
                IProgressManager progressManager,
                Func<TItem, TResponse> factory)
            {
                _handler = handler;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TItem, TResponse>.Handle(TItem request,
                CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != null)
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
            IJsonRpcRequestHandler<TItem, TResponse>
            where TItem : IPartialItemRequest<TResponse, TItem>
            where TResponse : class, new()
        {
            private readonly Action<TItem, IObserver<TItem>, CancellationToken> _handler;
            private readonly IProgressManager _progressManager;
            private readonly Func<TItem, TResponse> _factory;

            public PartialResult(
                Action<TItem, IObserver<TItem>> handler,
                IProgressManager progressManager,
                Func<TItem, TResponse> factory) : this((p, o, ct) => handler(p, o),
                progressManager, factory)
            {
            }

            public PartialResult(
                Action<TItem, IObserver<TItem>, CancellationToken> handler,
                IProgressManager progressManager,
                Func<TItem, TResponse> factory)
            {
                _handler = handler;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TItem, TResponse>.Handle(TItem request,
                CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != null)
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
            IRegistration<TRegistrationOptions>, ICapability<TCapability>
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

            public PartialResults(
                Action<TParams, IObserver<IEnumerable<TItem>>, TCapability> handler,
                TRegistrationOptions registrationOptions,
                IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse> factory) : this((p, o, c, ct) => handler(p, o, c), registrationOptions,
                progressManager, factory)
            {
            }

            public PartialResults(
                Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> handler,
                TRegistrationOptions registrationOptions,
                IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse> factory)
            {
                _handler = handler;
                _registrationOptions = registrationOptions;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TParams, TResponse>.Handle(TParams request,
                CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != null)
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
            IRegistration<TRegistrationOptions>
            where TParams : IPartialItemsRequest<TResponse, TItem>
            where TResponse : IEnumerable<TItem>, new()
            where TRegistrationOptions : class, new()
        {
            private readonly Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private readonly IProgressManager _progressManager;
            private readonly Func<IEnumerable<TItem>, TResponse> _factory;

            public PartialResults(
                Action<TParams, IObserver<IEnumerable<TItem>>> handler,
                TRegistrationOptions registrationOptions,
                IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse> factory) : this((p, o, ct) => handler(p, o), registrationOptions,
                progressManager, factory)
            {
            }

            public PartialResults(
                Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler,
                TRegistrationOptions registrationOptions,
                IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse> factory)
            {
                _handler = handler;
                _registrationOptions = registrationOptions;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TParams, TResponse>.Handle(TParams request,
                CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != null)
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
            IJsonRpcRequestHandler<TParams, TResponse>, ICapability<TCapability>
            where TParams : IPartialItemsRequest<TResponse, TItem>
            where TResponse : IEnumerable<TItem>, new()
            where TCapability : ICapability
        {
            private readonly Action<TParams, TCapability, IObserver<IEnumerable<TItem>>, CancellationToken> _handler;
            private readonly IProgressManager _progressManager;
            private readonly Func<IEnumerable<TItem>, TResponse> _factory;
            private TCapability _capability;

            public PartialResultsCapability(
                Action<TParams, TCapability, IObserver<IEnumerable<TItem>>> handler,
                IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse> factory) : this((p, c, o, ct) => handler(p, c, o),
                progressManager, factory)
            {
            }

            public PartialResultsCapability(
                Action<TParams, TCapability, IObserver<IEnumerable<TItem>>, CancellationToken> handler,
                IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse> factory)
            {
                _handler = handler;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TParams, TResponse>.Handle(TParams request,
                CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != null)
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
            IJsonRpcRequestHandler<TParams, TResponse>
            where TParams : IPartialItemsRequest<TResponse, TItem>
            where TResponse : IEnumerable<TItem>, new()
        {
            private readonly Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> _handler;
            private readonly IProgressManager _progressManager;
            private readonly Func<IEnumerable<TItem>, TResponse> _factory;

            public PartialResults(
                Action<TParams, IObserver<IEnumerable<TItem>>> handler,
                IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse> factory) : this((p, o, ct) => handler(p, o),
                progressManager, factory)
            {
            }

            public PartialResults(
                Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler,
                IProgressManager progressManager,
                Func<IEnumerable<TItem>, TResponse> factory)
            {
                _handler = handler;
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TParams, TResponse>.Handle(TParams request,
                CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != null)
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
            IJsonRpcRequestHandler<TParams>,
            IRegistration<TRegistrationOptions>, ICapability<TCapability>
            where TParams : IRequest
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private TCapability _capability;

            public Notification(
                Action<TParams, TCapability> handler,
                TRegistrationOptions registrationOptions) : this((request, capability, ct) => {
                handler(request, capability);
                return Task.CompletedTask;
            }, registrationOptions)
            {
            }

            public Notification(
                Action<TParams, TCapability, CancellationToken> handler,
                TRegistrationOptions registrationOptions) : this((request, c, ct) => {
                handler(request, c, ct);
                return Task.CompletedTask;
            }, registrationOptions)
            {
            }

            public Notification(
                Func<TParams, TCapability, Task> handler,
                TRegistrationOptions registrationOptions) : this((request, capability, ct) => handler(request, capability), registrationOptions)
            {
            }

            public Notification(
                Func<TParams, TCapability, CancellationToken, Task> handler,
                TRegistrationOptions registrationOptions)
            {
                _handler = handler;
                _registrationOptions = registrationOptions;
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.Handle(TParams request, CancellationToken cancellationToken)
            {
                await                _handler(request, _capability, cancellationToken);
                return Unit.Value;
            }

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }

        public sealed class Notification<TParams, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams>,
            IRegistration<TRegistrationOptions>
            where TParams : IRequest
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TParams, CancellationToken, Task> _handler;
            private readonly TRegistrationOptions _registrationOptions;

            public Notification(
                Action<TParams> handler,
                TRegistrationOptions registrationOptions) : this((request, ct) => {
                handler(request);
                return Task.CompletedTask;
            }, registrationOptions)
            {
            }

            public Notification(
                Action<TParams, CancellationToken> handler,
                TRegistrationOptions registrationOptions) : this((request, ct) => {
                handler(request, ct);
                return Task.CompletedTask;
            }, registrationOptions)
            {
            }

            public Notification(
                Func<TParams, Task> handler,
                TRegistrationOptions registrationOptions) : this((request, ct) => handler(request), registrationOptions)
            {
            }

            public Notification(
                Func<TParams, CancellationToken, Task> handler,
                TRegistrationOptions registrationOptions)
            {
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
            IJsonRpcRequestHandler<TParams>, ICapability<TCapability>
            where TParams : IRequest
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task> _handler;
            private TCapability _capability;

            public NotificationCapability(
                Action<TParams, TCapability> handler) : this((request, capability, ct) => {
                handler(request, capability);
                return Task.CompletedTask;
            })
            {
            }

            public NotificationCapability(
                Func<TParams, TCapability, Task> handler) : this((request, capability, ct) => handler(request, capability))
            {
            }

            public NotificationCapability(
                Func<TParams, TCapability, CancellationToken, Task> handler)
            {
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
