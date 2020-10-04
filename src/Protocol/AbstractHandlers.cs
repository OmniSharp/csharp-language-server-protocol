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
        public abstract class Request<TParams, TResult, TCapability, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams, TResult>,
            IRegistration<TRegistrationOptions>, ICapability<TCapability>
            where TParams : IRequest<TResult>
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly TRegistrationOptions _registrationOptions;
            protected TCapability Capability { get; private set; } = default!;

            protected Request(TRegistrationOptions registrationOptions) => _registrationOptions = registrationOptions;

            public abstract Task<TResult> Handle(TParams request, CancellationToken cancellationToken);

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => Capability = capability;
        }

        public abstract class PartialResult<TParams, TResponse, TItem, TCapability, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams, TResponse>,
            IRegistration<TRegistrationOptions>, ICapability<TCapability>
            where TParams : IPartialItemRequest<TResponse, TItem>
            where TResponse : class, new()
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly TRegistrationOptions _registrationOptions;
            private readonly IProgressManager _progressManager;
            private readonly Func<TItem, TResponse> _factory;
            protected TCapability Capability { get; private set; } = default!;

            protected PartialResult(
                TRegistrationOptions registrationOptions,
                IProgressManager progressManager,
                Func<TItem, TResponse> factory
            )
            {
                _registrationOptions = registrationOptions;
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

            protected abstract void Handle(
                TParams request, IObserver<TItem> results,
                CancellationToken cancellationToken
            );

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => Capability = capability;
        }

        public abstract class PartialResults<TParams, TResponse, TItem, TCapability, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams, TResponse>,
            IRegistration<TRegistrationOptions>, ICapability<TCapability>
            where TParams : IPartialItemsRequest<TResponse, TItem>
            where TResponse : IEnumerable<TItem>, new()
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly TRegistrationOptions _registrationOptions;
            private readonly IProgressManager _progressManager;
            private readonly Func<IEnumerable<TItem>, TResponse> _factory;
            protected TCapability Capability { get; private set; } = default!;

            protected PartialResults(TRegistrationOptions registrationOptions, IProgressManager progressManager, Func<IEnumerable<TItem>, TResponse> factory)
            {
                _registrationOptions = registrationOptions;
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

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => Capability = capability;
        }

        public abstract class Notification<TParams, TCapability, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams>,
            IRegistration<TRegistrationOptions>, ICapability<TCapability>
            where TParams : IRequest
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly TRegistrationOptions _registrationOptions;
            protected TCapability Capability { get; private set; } = default!;

            protected Notification(TRegistrationOptions registrationOptions) => _registrationOptions = registrationOptions;

            public Task<Unit> Handle(TParams request, CancellationToken cancellationToken)
            {
                Handle(request);
                return Unit.Task;
            }

            protected abstract void Handle(TParams request);

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => Capability = capability;
        }
    }
}
