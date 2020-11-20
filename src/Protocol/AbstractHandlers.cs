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
    public static class AbstractHandlers
    {
        public abstract class Base<TRegistrationOptions, TCapability> :
            IRegistration<TRegistrationOptions, TCapability>
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            protected TRegistrationOptions RegistrationOptions { get; private set; } = default!;
            protected TCapability Capability { get; private set; } = default!;
            protected abstract TRegistrationOptions CreateRegistrationOptions(TCapability capability);

            TRegistrationOptions IRegistration<TRegistrationOptions, TCapability>.GetRegistrationOptions(TCapability capability)
            {
                Capability = capability;
                return RegistrationOptions = CreateRegistrationOptions(capability);
            }
        }

        public abstract class BaseCapability<TCapability> :
            ICapability<TCapability>
            where TCapability : ICapability
        {
            protected TCapability Capability { get; private set; } = default!;

            void ICapability<TCapability>.SetCapability(TCapability capability)
            {
                Capability = capability;
            }
        }

        public abstract class Base<TRegistrationOptions> :
            IRegistration<TRegistrationOptions>
            where TRegistrationOptions : class, new()
        {
            protected TRegistrationOptions RegistrationOptions { get; private set; } = default!;
            protected abstract TRegistrationOptions CreateRegistrationOptions();

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions()
            {
                return RegistrationOptions = CreateRegistrationOptions();
            }
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
            IJsonRpcRequestHandler<TParams, TResponse>
            where TParams : IPartialItemRequest<TResponse, TItem>
            where TResponse : class, new()
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly IProgressManager _progressManager;
            private readonly Func<TItem, TResponse> _factory;

            protected PartialResult(IProgressManager progressManager, Func<TItem, TResponse> factory)
            {
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TParams, TResponse>.Handle(
                TParams request,
                CancellationToken cancellationToken
            )
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer == ProgressObserver<TItem>.Noop)
                {
                    Handle(request, observer, cancellationToken);
                    await observer;
                    return new TResponse();
                }

                var subject = new AsyncSubject<TItem>();
                // in the event nothing is emitted...
                subject.OnNext(default!);
                Handle(request, subject, cancellationToken);
                return _factory(await subject);
            }

            protected abstract void Handle(TParams request, IObserver<TItem> results, CancellationToken cancellationToken);
        }

        public abstract class PartialResult<TParams, TResponse, TItem, TRegistrationOptions> :
            Base<TRegistrationOptions>,
            IJsonRpcRequestHandler<TParams, TResponse>
            where TParams : IPartialItemRequest<TResponse, TItem>
            where TResponse : class, new()
            where TRegistrationOptions : class, new()
        {
            private readonly IProgressManager _progressManager;
            private readonly Func<TItem, TResponse> _factory;

            protected PartialResult(IProgressManager progressManager, Func<TItem, TResponse> factory)
            {
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TParams, TResponse>.Handle(
                TParams request,
                CancellationToken cancellationToken
            )
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer == ProgressObserver<TItem>.Noop)
                {
                    Handle(request, observer, cancellationToken);
                    await observer;
                    return new TResponse();
                }

                var subject = new AsyncSubject<TItem>();
                // in the event nothing is emitted...
                subject.OnNext(default!);
                Handle(request, subject, cancellationToken);
                return _factory(await subject);
            }

            protected abstract void Handle(TParams request, IObserver<TItem> results, CancellationToken cancellationToken);
        }

        public abstract class PartialResultCapability<TParams, TResponse, TItem, TCapability> :
            BaseCapability<TCapability>,
            IJsonRpcRequestHandler<TParams, TResponse>
            where TParams : IPartialItemRequest<TResponse, TItem>
            where TResponse : class, new()
            where TCapability : ICapability
        {
            private readonly IProgressManager _progressManager;
            private readonly Func<TItem, TResponse> _factory;

            protected PartialResultCapability(IProgressManager progressManager, Func<TItem, TResponse> factory)
            {
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TParams, TResponse>.Handle(
                TParams request,
                CancellationToken cancellationToken
            )
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer == ProgressObserver<TItem>.Noop)
                {
                    Handle(request, observer, cancellationToken);
                    await observer;
                    return new TResponse();
                }

                var subject = new AsyncSubject<TItem>();
                // in the event nothing is emitted...
                subject.OnNext(default!);
                Handle(request, subject, cancellationToken);
                return _factory(await subject);
            }

            protected abstract void Handle(TParams request, IObserver<TItem> results, CancellationToken cancellationToken);
        }

        public abstract class PartialResults<TParams, TResponse, TItem, TRegistrationOptions, TCapability> :
            Base<TRegistrationOptions, TCapability>,
            IJsonRpcRequestHandler<TParams, TResponse>
            where TParams : IPartialItemsRequest<TResponse, TItem>
            where TResponse : IEnumerable<TItem>?, new()
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly IProgressManager _progressManager;
            private readonly Func<IEnumerable<TItem>, TResponse> _factory;

            protected PartialResults(IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory)
            {
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TParams, TResponse>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TItem>.Noop)
                {
                    Handle(request, observer, cancellationToken);
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
                Handle(request, subject, cancellationToken);
                return _factory(await task.ConfigureAwait(false));
            }

            protected abstract void Handle(TParams request, IObserver<IEnumerable<TItem>> results, CancellationToken cancellationToken);
        }

        public abstract class PartialResults<TParams, TResponse, TItem, TRegistrationOptions> :
            Base<TRegistrationOptions>,
            IJsonRpcRequestHandler<TParams, TResponse>
            where TParams : IPartialItemsRequest<TResponse, TItem>
            where TResponse : IEnumerable<TItem>?, new()
            where TRegistrationOptions : class, new()
        {
            private readonly IProgressManager _progressManager;
            private readonly Func<IEnumerable<TItem>, TResponse> _factory;

            protected PartialResults(IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory)
            {
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TParams, TResponse>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TItem>.Noop)
                {
                    Handle(request, observer, cancellationToken);
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
                Handle(request, subject, cancellationToken);
                return _factory(await task.ConfigureAwait(false));
            }

            protected abstract void Handle(TParams request, IObserver<IEnumerable<TItem>> results, CancellationToken cancellationToken);
        }
        public abstract class PartialResultsCapability<TParams, TResponse, TItem, TCapability> :
            BaseCapability<TCapability>,
            IJsonRpcRequestHandler<TParams, TResponse>
            where TParams : IPartialItemsRequest<TResponse, TItem>
            where TResponse : IEnumerable<TItem>?, new()
            where TCapability : ICapability
        {
            private readonly IProgressManager _progressManager;
            private readonly Func<IEnumerable<TItem>, TResponse> _factory;

            protected PartialResultsCapability(IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory)
            {
                _progressManager = progressManager;
                _factory = factory;
            }

            async Task<TResponse> IRequestHandler<TParams, TResponse>.Handle(TParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<TItem>.Noop)
                {
                    Handle(request, observer, cancellationToken);
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
                Handle(request, subject, cancellationToken);
                return _factory(await task.ConfigureAwait(false));
            }

            protected abstract void Handle(TParams request, IObserver<IEnumerable<TItem>> results, CancellationToken cancellationToken);
        }

        public abstract class Notification<TParams, TRegistrationOptions, TCapability> :
            Base<TRegistrationOptions, TCapability>,
            IJsonRpcRequestHandler<TParams>
            where TParams : IRequest
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            public Task<Unit> Handle(TParams request, CancellationToken cancellationToken)
            {
                Handle(request);
                return Unit.Task;
            }

            protected abstract void Handle(TParams request);
        }

        public abstract class Notification<TParams, TRegistrationOptions> :
            Base<TRegistrationOptions>,
            IJsonRpcRequestHandler<TParams>
            where TParams : IRequest
            where TRegistrationOptions : class, new()
        {
            public Task<Unit> Handle(TParams request, CancellationToken cancellationToken)
            {
                Handle(request);
                return Unit.Task;
            }

            protected abstract void Handle(TParams request);
        }

        public abstract class NotificationCapability<TParams, TCapability> :
            BaseCapability<TCapability>,
            IJsonRpcRequestHandler<TParams>
            where TParams : IRequest
            where TCapability : ICapability
        {
            public Task<Unit> Handle(TParams request, CancellationToken cancellationToken)
            {
                Handle(request);
                return Unit.Task;
            }

            protected abstract void Handle(TParams request);
        }
    }
}
