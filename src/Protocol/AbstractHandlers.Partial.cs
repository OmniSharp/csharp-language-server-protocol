﻿using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static partial class AbstractHandlers
    {
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
                    return default;
                }

                using var subject = new AsyncSubject<TItem?>();
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
                    return default;
                }

                using var subject = new AsyncSubject<TItem?>();
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
                    return default;
                }

                using var subject = new AsyncSubject<TItem>();
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
                    return default;
                }

                using var subject = new Subject<IEnumerable<TItem>>();
                var task = subject
                          .Aggregate(
                               new List<TItem>(), (acc, items) =>
                               {
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
                    return default;
                }

                using var subject = new Subject<IEnumerable<TItem>>();
                var task = subject
                          .Aggregate(
                               new List<TItem>(), (acc, items) =>
                               {
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
                    return default;
                }

                using var subject = new Subject<IEnumerable<TItem>>();
                var task = subject
                          .Aggregate(
                               new List<TItem>(), (acc, items) =>
                               {
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

    }
}
