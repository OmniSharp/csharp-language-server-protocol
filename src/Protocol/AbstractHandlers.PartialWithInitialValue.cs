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
    public static partial class AbstractHandlers
    {
        public abstract class PartialResultWithInitialValue<TParams, TResponse, TItem, TRegistrationOptions, TCapability> :
            Base<TRegistrationOptions, TCapability>,
            IJsonRpcRequestHandler<TParams, TResponse?>
            where TItem : class?
            where TParams : IPartialItemWithInitialValueRequest<TResponse?, TItem?>
            where TResponse : TItem?
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly IProgressManager _progressManager;
            private readonly Func<TResponse, TItem?, TResponse> _factory;

            protected PartialResultWithInitialValue(IProgressManager progressManager, 
                                                    Func<TResponse, TItem?, TResponse> factory)
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
                if (observer != ProgressObserver<TResponse, TItem>.Noop)
                {
                    observer.OnNext(await HandleInitialValue(request, cancellationToken));
                    Handle(request, observer, cancellationToken);
                    await observer;
                    return default;
                }

                using var subject = new AsyncSubject<TItem?>();
                var task = subject
                          .Aggregate(await HandleInitialValue(request, cancellationToken), _factory)
                          .ToTask(cancellationToken, _progressManager.Scheduler)
                          .ConfigureAwait(false);
                // in the event nothing is emitted...
                subject.OnNext(default!);
                Handle(request, subject, cancellationToken);
                return await task;
            }

            protected abstract Task<TResponse> HandleInitialValue(TParams request, CancellationToken cancellationToken);
            protected abstract void Handle(TParams request, IObserver<TItem> results, CancellationToken cancellationToken);
        }

        public abstract class PartialResultWithInitialValue<TParams, TResponse, TItem, TRegistrationOptions> :
            Base<TRegistrationOptions>,
            IJsonRpcRequestHandler<TParams, TResponse?>
            where TItem : class?
            where TParams : IPartialItemWithInitialValueRequest<TResponse?, TItem?>
            where TResponse : TItem?
            where TRegistrationOptions : class, new()
        {
            private readonly IProgressManager _progressManager;
            private readonly Func<TResponse, TItem?, TResponse> _factory;

            protected PartialResultWithInitialValue(IProgressManager progressManager, 
                                                    Func<TResponse, TItem?, TResponse> factory)
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
                if (observer != ProgressObserver<TResponse, TItem>.Noop)
                {
                    observer.OnNext(await HandleInitialValue(request, cancellationToken));
                    Handle(request, observer, cancellationToken);
                    await observer;
                    return default;
                }

                using var subject = new AsyncSubject<TItem?>();
                var task = subject
                          .Aggregate(await HandleInitialValue(request, cancellationToken), _factory)
                          .ToTask(cancellationToken, _progressManager.Scheduler)
                          .ConfigureAwait(false);
                // in the event nothing is emitted...
                subject.OnNext(default!);
                Handle(request, subject, cancellationToken);
                return await task;
            }

            protected abstract Task<TResponse> HandleInitialValue(TParams request, CancellationToken cancellationToken);
            protected abstract void Handle(TParams request, IObserver<TItem> results, CancellationToken cancellationToken);
        }

        public abstract class PartialResultWithInitialValueCapability<TParams, TResponse, TItem, TCapability> :
            BaseCapability<TCapability>,
            IJsonRpcRequestHandler<TParams, TResponse?>
            where TItem : class?
            where TParams : IPartialItemWithInitialValueRequest<TResponse?, TItem?>
            where TResponse : TItem?
            where TCapability : ICapability
        {
            private readonly IProgressManager _progressManager;
            private readonly Func<TResponse, TItem?, TResponse> _factory;

            protected PartialResultWithInitialValueCapability(IProgressManager progressManager, 
                                                              Func<TResponse, TItem?, TResponse> factory)
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
                if (observer != ProgressObserver<TResponse, TItem>.Noop)
                {
                    observer.OnNext(await HandleInitialValue(request, cancellationToken));
                    Handle(request, observer, cancellationToken);
                    await observer;
                    return default;
                }

                using var subject = new AsyncSubject<TItem>();
                var task = subject
                          .Aggregate(await HandleInitialValue(request, cancellationToken), _factory)
                          .ToTask(cancellationToken, _progressManager.Scheduler)
                          .ConfigureAwait(false);
                // in the event nothing is emitted...
                subject.OnNext(default!);
                Handle(request, subject, cancellationToken);
                return await task;
            }

            protected abstract Task<TResponse> HandleInitialValue(TParams request, CancellationToken cancellationToken);
            protected abstract void Handle(TParams request, IObserver<TItem> results, CancellationToken cancellationToken);
        }

        public abstract class PartialResultsWithInitialValue<TParams, TResponse, TItem, TRegistrationOptions, TCapability> :
            Base<TRegistrationOptions, TCapability>,
            IJsonRpcRequestHandler<TParams, TResponse?>
            where TParams : IPartialItemsWithInitialValueRequest<TResponse?, TItem>
            where TResponse : IEnumerable<TItem>?
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly IProgressManager _progressManager;
            private readonly Func<TResponse, IEnumerable<TItem>, TResponse> _factory;

            protected PartialResultsWithInitialValue(
                IProgressManager progressManager,
                Func<TResponse, IEnumerable<TItem>, TResponse> factory
            )
            {
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TResponse?, TItem>.Noop)
                {
                    observer.OnNext(await HandleInitialValue(request, cancellationToken));
                    Handle(request, observer, cancellationToken);
                    await observer;
                    return default;
                }

                {
                    using var subject = new Subject<IEnumerable<TItem>>();
                    var task = subject
                              .Aggregate(await HandleInitialValue(request, cancellationToken), _factory)
                              .ToTask(cancellationToken, _progressManager.Scheduler)
                              .ConfigureAwait(false);
                    Handle(request, subject, cancellationToken);
                    return await task;
                }
            }

            protected abstract Task<TResponse> HandleInitialValue(TParams request, CancellationToken cancellationToken);
            protected abstract void Handle(TParams request, IObserver<IEnumerable<TItem>> results, CancellationToken cancellationToken);
        }

        public abstract class PartialResultsWithInitialValue<TParams, TResponse, TItem, TRegistrationOptions> :
            Base<TRegistrationOptions>,
            IJsonRpcRequestHandler<TParams, TResponse?>
            where TParams : IPartialItemsWithInitialValueRequest<TResponse?, TItem>
            where TResponse : IEnumerable<TItem>?
            where TRegistrationOptions : class, new()
        {
            private readonly IProgressManager _progressManager;
            private readonly Func<TResponse, IEnumerable<TItem>, TResponse> _factory;

            protected PartialResultsWithInitialValue(
                IProgressManager progressManager, Func<TResponse, IEnumerable<TItem>, TResponse> factory
            )
            {
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TResponse?, TItem>.Noop)
                {
                    observer.OnNext(await HandleInitialValue(request, cancellationToken));
                    Handle(request, observer, cancellationToken);
                    await observer;
                    return default;
                }

                {
                    using var subject = new Subject<IEnumerable<TItem>>();
                    var task = subject
                              .Aggregate(await HandleInitialValue(request, cancellationToken), _factory)
                              .ToTask(cancellationToken, _progressManager.Scheduler)
                              .ConfigureAwait(false);
                    Handle(request, subject, cancellationToken);
                    return await task;
                }
            }

            protected abstract Task<TResponse> HandleInitialValue(TParams request, CancellationToken cancellationToken);
            protected abstract void Handle(TParams request, IObserver<IEnumerable<TItem>> results, CancellationToken cancellationToken);
        }

        public abstract class PartialResultsWithInitialValueCapability<TParams, TResponse, TItem, TCapability> :
            BaseCapability<TCapability>,
            IJsonRpcRequestHandler<TParams, TResponse?>
            where TParams : IPartialItemsWithInitialValueRequest<TResponse?, TItem>
            where TResponse : IEnumerable<TItem>?
            where TCapability : ICapability
        {
            private readonly IProgressManager _progressManager;
            private readonly Func<TResponse, IEnumerable<TItem>, TResponse> _factory;

            protected PartialResultsWithInitialValueCapability(
                IProgressManager progressManager, Func<TResponse, IEnumerable<TItem>, TResponse?> factory
            )
            {
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TResponse?, TItem>.Noop)
                {
                    observer.OnNext(await HandleInitialValue(request, cancellationToken));
                    Handle(request, observer, cancellationToken);
                    await observer;
                    return default;
                }

                {
                    using var subject = new Subject<IEnumerable<TItem>>();
                    var task = subject
                              .Aggregate(await HandleInitialValue(request, cancellationToken), _factory)
                              .ToTask(cancellationToken, _progressManager.Scheduler)
                              .ConfigureAwait(false);
                    Handle(request, subject, cancellationToken);
                    return await task;
                }
            }

            protected abstract Task<TResponse> HandleInitialValue(TParams request, CancellationToken cancellationToken);
            protected abstract void Handle(TParams request, IObserver<IEnumerable<TItem>> results, CancellationToken cancellationToken);
        }
    }
}
