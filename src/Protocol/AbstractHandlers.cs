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
    public static class AbstractHandlers
    {
        public abstract class Base<TRegistrationOptions, TCapability> :
            IRegistration<TRegistrationOptions, TCapability>,
            ICapability<TCapability>
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            protected TRegistrationOptions RegistrationOptions { get; private set; } = default!;
            protected TCapability Capability { get; private set; } = default!;
            protected ClientCapabilities ClientCapabilities { get; private set; } = default!;
            protected internal abstract TRegistrationOptions CreateRegistrationOptions(TCapability capability, ClientCapabilities clientCapabilities);

            TRegistrationOptions IRegistration<TRegistrationOptions, TCapability>.GetRegistrationOptions(TCapability capability, ClientCapabilities clientCapabilities)
            {
                // ReSharper disable twice ConditionIsAlwaysTrueOrFalse
                if (RegistrationOptions is not null && Capability is not null) return RegistrationOptions;
                Capability = capability;
                ClientCapabilities = clientCapabilities;
                return RegistrationOptions = CreateRegistrationOptions(capability, clientCapabilities);
            }

            void ICapability<TCapability>.SetCapability(TCapability capability, ClientCapabilities clientCapabilities)
            {
                ClientCapabilities = clientCapabilities;
                Capability = capability;
            }
        }

        public abstract class BaseCapability<TCapability> :
            ICapability<TCapability>
            where TCapability : ICapability
        {
            protected TCapability Capability { get; private set; } = default!;
            protected ClientCapabilities ClientCapabilities { get; private set; } = default!;

            void ICapability<TCapability>.SetCapability(TCapability capability, ClientCapabilities clientCapabilities)
            {
                ClientCapabilities = clientCapabilities;
                Capability = capability;
            }
        }

        public abstract class Base<TRegistrationOptions> :
            IRegistration<TRegistrationOptions>
            where TRegistrationOptions : class, new()
        {
            protected TRegistrationOptions RegistrationOptions { get; private set; } = default!;
            protected ClientCapabilities ClientCapabilities { get; private set; } = default!;
            protected abstract TRegistrationOptions CreateRegistrationOptions(ClientCapabilities clientCapabilities);

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions(ClientCapabilities clientCapabilities)
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (RegistrationOptions is not null) return RegistrationOptions;
                ClientCapabilities = clientCapabilities;
                return RegistrationOptions = CreateRegistrationOptions(clientCapabilities);
            }
        }

        public abstract class Request<TParams, TResult> :
            IJsonRpcRequestHandler<TParams, TResult>
            where TParams : IRequest<TResult>
        {
            public abstract Task<TResult> Handle(TParams request, CancellationToken cancellationToken);
        }

        public abstract class Request<TParams, TResult, TRegistrationOptions> :
            Base<TRegistrationOptions>,
            IJsonRpcRequestHandler<TParams, TResult>
            where TParams : IRequest<TResult>
            where TRegistrationOptions : class, new()
        {
            public abstract Task<TResult> Handle(TParams request, CancellationToken cancellationToken);
        }

        public abstract class Request<TParams, TResult, TRegistrationOptions, TCapability> :
            Base<TRegistrationOptions, TCapability>,
            IJsonRpcRequestHandler<TParams, TResult>
            where TParams : IRequest<TResult>
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            public abstract Task<TResult> Handle(TParams request, CancellationToken cancellationToken);
        }

        public abstract class RequestCapability<TParams, TResult, TCapability> :
            BaseCapability<TCapability>,
            IJsonRpcRequestHandler<TParams, TResult>
            where TParams : IRequest<TResult>
            where TCapability : ICapability
        {
            public abstract Task<TResult> Handle(TParams request, CancellationToken cancellationToken);
        }

        public abstract class PartialResult<TParams, TResponse, TItem, TRegistrationOptions, TCapability> :
            Base<TRegistrationOptions, TCapability>,
            IJsonRpcRequestHandler<TParams, TResponse?>
            where TItem : class?
            where TParams : IPartialItemRequest<TResponse?, TItem>
            where TResponse : class?
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly IProgressManager _progressManager;
            private readonly Func<TItem?, TResponse?> _factory;

            protected PartialResult(IProgressManager progressManager, Func<TItem?, TResponse?> factory)
            {
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(
                TParams request,
                CancellationToken cancellationToken
            )
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TItem>.Noop)
                {
                    Handle(request, observer, cancellationToken);
                    await observer;
                    return _factory(default(TItem));
                }

                var subject = new AsyncSubject<TItem?>();
                var task = subject
                          .Select(_factory)
                          .ToTask(cancellationToken, _progressManager.Scheduler)
                          .ConfigureAwait(false);
                // in the event nothing is emitted...
                subject.OnNext(default!);
                Handle(request, subject, cancellationToken);
                return await task;
            }

            protected abstract void Handle(TParams request, IObserver<TItem> results, CancellationToken cancellationToken);
        }

        public abstract class PartialResult<TParams, TResponse, TItem, TRegistrationOptions> :
            Base<TRegistrationOptions>,
            IJsonRpcRequestHandler<TParams, TResponse?>
            where TItem : class?
            where TParams : IPartialItemRequest<TResponse?, TItem>
            where TResponse : class?
            where TRegistrationOptions : class, new()
        {
            private readonly IProgressManager _progressManager;
            private readonly Func<TItem?, TResponse?> _factory;

            protected PartialResult(IProgressManager progressManager, Func<TItem?, TResponse?> factory)
            {
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(
                TParams request,
                CancellationToken cancellationToken
            )
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TItem>.Noop)
                {
                    Handle(request, observer, cancellationToken);
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
                Handle(request, subject, cancellationToken);
                return await task;
            }

            protected abstract void Handle(TParams request, IObserver<TItem> results, CancellationToken cancellationToken);
        }

        public abstract class PartialResultCapability<TParams, TResponse, TItem, TCapability> :
            BaseCapability<TCapability>,
            IJsonRpcRequestHandler<TParams, TResponse?>
            where TItem : class?
            where TParams : IPartialItemRequest<TResponse?, TItem>
            where TResponse : class?
            where TCapability : ICapability
        {
            private readonly IProgressManager _progressManager;
            private readonly Func<TItem?, TResponse?> _factory;

            protected PartialResultCapability(IProgressManager progressManager, Func<TItem?, TResponse?> factory)
            {
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(
                TParams request,
                CancellationToken cancellationToken
            )
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TItem>.Noop)
                {
                    Handle(request, observer, cancellationToken);
                    await observer;
                    return _factory(default);
                }

                var subject = new AsyncSubject<TItem>();
                var task = subject
                          .Select(_factory)
                          .ToTask(cancellationToken, _progressManager.Scheduler)
                          .ConfigureAwait(false);
                // in the event nothing is emitted...
                subject.OnNext(default!);
                Handle(request, subject, cancellationToken);
                return await task;
            }

            protected abstract void Handle(TParams request, IObserver<TItem> results, CancellationToken cancellationToken);
        }

        public abstract class PartialResults<TParams, TResponse, TItem, TRegistrationOptions, TCapability> :
            Base<TRegistrationOptions, TCapability>,
            IJsonRpcRequestHandler<TParams, TResponse?>
            where TParams : IPartialItemsRequest<TResponse?, TItem>
            where TResponse : IEnumerable<TItem>?
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly IProgressManager _progressManager;
            private readonly Func<IEnumerable<TItem>, TResponse?> _factory;

            protected PartialResults(IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse?> factory)
            {
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TItem>.Noop)
                {
                    Handle(request, observer, cancellationToken);
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
                Handle(request, subject, cancellationToken);
                return await task;
            }

            protected abstract void Handle(TParams request, IObserver<IEnumerable<TItem>> results, CancellationToken cancellationToken);
        }

        public abstract class PartialResults<TParams, TResponse, TItem, TRegistrationOptions> :
            Base<TRegistrationOptions>,
            IJsonRpcRequestHandler<TParams, TResponse?>
            where TParams : IPartialItemsRequest<TResponse?, TItem>
            where TResponse : IEnumerable<TItem>?
            where TRegistrationOptions : class, new()
        {
            private readonly IProgressManager _progressManager;
            private readonly Func<IEnumerable<TItem>, TResponse?> _factory;

            protected PartialResults(IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse?> factory)
            {
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TItem>.Noop)
                {
                    Handle(request, observer, cancellationToken);
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
                Handle(request, subject, cancellationToken);
                return await task;
            }

            protected abstract void Handle(TParams request, IObserver<IEnumerable<TItem>> results, CancellationToken cancellationToken);
        }

        public abstract class PartialResultsCapability<TParams, TResponse, TItem, TCapability> :
            BaseCapability<TCapability>,
            IJsonRpcRequestHandler<TParams, TResponse?>
            where TParams : IPartialItemsRequest<TResponse?, TItem>
            where TResponse : IEnumerable<TItem>?
            where TCapability : ICapability
        {
            private readonly IProgressManager _progressManager;
            private readonly Func<IEnumerable<TItem>, TResponse?> _factory;

            protected PartialResultsCapability(IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse?> factory)
            {
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TItem>.Noop)
                {
                    Handle(request, observer, cancellationToken);
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
                Handle(request, subject, cancellationToken);
                return await task;
            }

            protected abstract void Handle(TParams request, IObserver<IEnumerable<TItem>> results, CancellationToken cancellationToken);
        }

        public abstract class Notification<TParams> : IJsonRpcRequestHandler<TParams>
            where TParams : IRequest
        {
            public abstract Task<Unit> Handle(TParams request, CancellationToken cancellationToken);
        }

        public abstract class Notification<TParams, TRegistrationOptions, TCapability> :
            Base<TRegistrationOptions, TCapability>,
            IJsonRpcRequestHandler<TParams>
            where TParams : IRequest
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            public abstract Task<Unit> Handle(TParams request, CancellationToken cancellationToken);
        }

        public abstract class Notification<TParams, TRegistrationOptions> :
            Base<TRegistrationOptions>,
            IJsonRpcRequestHandler<TParams>
            where TParams : IRequest
            where TRegistrationOptions : class, new()
        {
            public abstract Task<Unit> Handle(TParams request, CancellationToken cancellationToken);
        }

        public abstract class NotificationCapability<TParams, TCapability> :
            BaseCapability<TCapability>,
            IJsonRpcRequestHandler<TParams>
            where TParams : IRequest
            where TCapability : ICapability
        {
            public abstract Task<Unit> Handle(TParams request, CancellationToken cancellationToken);
        }
    }
}
