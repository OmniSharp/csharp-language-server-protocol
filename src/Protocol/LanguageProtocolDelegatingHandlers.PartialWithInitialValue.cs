using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Protocol;

public static partial class LanguageProtocolDelegatingHandlers
{
    public sealed class PartialResultWithInitialValue<TParams, TResponse, TItem, TRegistrationOptions, TCapability> :
        AbstractHandlers.Base<TRegistrationOptions, TCapability>,
        IJsonRpcRequestHandler<TParams, TResponse?>,
        ICanBeIdentifiedHandler
        where TParams : IPartialItemWithInitialValueRequest<TResponse?, TItem>
        where TItem : class?
        where TResponse : class?
        where TRegistrationOptions : class, new()
        where TCapability : ICapability
    {
        private readonly Func<TResponse, TItem?, TResponse> _factory;
        private readonly Func<TParams, TCapability, CancellationToken, Task<TResponse>> _initialValue;
        private readonly Action<TParams, IObserver<TItem>, TCapability, CancellationToken> _handler;
        private readonly RegistrationOptionsDelegate<TRegistrationOptions, TCapability> _registrationOptionsFactory;
        private readonly IProgressManager _progressManager;
        private readonly Guid _id;
        Guid ICanBeIdentifiedHandler.Id => _id;

        public PartialResultWithInitialValue(
            Guid id,
            Func<TParams, TCapability, CancellationToken, Task<TResponse>> initialValue,
            Action<TParams, IObserver<TItem>, TCapability, CancellationToken> handler,
            RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory, IProgressManager progressManager,
            Func<TResponse, TItem?, TResponse> factory
        )
        {
            _id = id;
            _handler = handler;
            _registrationOptionsFactory = registrationOptionsFactory;
            _progressManager = progressManager;
            _factory = factory;
            _initialValue = initialValue;
        }

        public PartialResultWithInitialValue(
            Func<TParams, TCapability, CancellationToken, Task<TResponse>> initialValue,
            Action<TParams, IObserver<TItem>, TCapability, CancellationToken> handler,
            RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory, IProgressManager progressManager,
            Func<TResponse, TItem?, TResponse> factory
        ) :
            this(Guid.Empty, initialValue, handler, registrationOptionsFactory, progressManager, factory)
        {
        }

        async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(
            TParams request,
            CancellationToken cancellationToken
        )
        {
            var observer = _progressManager.For(request, cancellationToken);
            if (observer != ProgressObserver<TResponse, TItem>.Noop)
            {
                observer.OnNext(await _initialValue(request, Capability, cancellationToken));
                _handler(request, observer, Capability, cancellationToken);
                await observer;
                return default;
            }

            using var subject = new AsyncSubject<TItem?>();
            var task = subject
                      .Aggregate(await _initialValue(request, Capability, cancellationToken), _factory)
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

    public sealed class PartialResultWithInitialValue<TParams, TResponse, TItem, TRegistrationOptions> :
        AbstractHandlers.Base<TRegistrationOptions>,
        IJsonRpcRequestHandler<TParams, TResponse?>,
        ICanBeIdentifiedHandler
        where TParams : IPartialItemWithInitialValueRequest<TResponse, TItem>
        where TItem : class?
        where TResponse : class?
        where TRegistrationOptions : class, new()
    {
        private readonly Action<TParams, IObserver<TItem>, CancellationToken> _handler;
        private readonly RegistrationOptionsDelegate<TRegistrationOptions> _registrationOptionsFactory;
        private readonly IProgressManager _progressManager;
        private readonly Func<TResponse, TItem?, TResponse> _factory;
        private readonly Func<TParams, CancellationToken, Task<TResponse>> _initialValue;
        private readonly Guid _id;
        Guid ICanBeIdentifiedHandler.Id => _id;

        public PartialResultWithInitialValue(
            Action<TParams, IObserver<TItem>, CancellationToken> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory,
            IProgressManager progressManager,
            Func<TResponse, TItem?, TResponse> factory,
            Func<TParams, CancellationToken, Task<TResponse>> initialValue
        ) :
            this(Guid.Empty, initialValue, handler, registrationOptionsFactory, progressManager, factory)
        {
        }

        public PartialResultWithInitialValue(
            Guid id,
            Func<TParams, CancellationToken, Task<TResponse>> initialValue,
            Action<TParams, IObserver<TItem>, CancellationToken> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory,
            IProgressManager progressManager,
            Func<TResponse, TItem?, TResponse> factory
        )
        {
            _id = id;
            _handler = handler;
            _registrationOptionsFactory = registrationOptionsFactory;
            _progressManager = progressManager;
            _factory = factory;
            _initialValue = initialValue;
        }

        async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(TParams request, CancellationToken cancellationToken)
        {
            var observer = _progressManager.For(request, cancellationToken);
            if (observer != ProgressObserver<TResponse, TItem>.Noop)
            {
                observer.OnNext(await _initialValue(request, cancellationToken));
                _handler(request, observer, cancellationToken);
                await observer;
                return default;
            }

            using var subject = new AsyncSubject<TItem?>();
            var task = subject
                      .Aggregate(await _initialValue(request, cancellationToken), _factory)
                      .ToTask(cancellationToken, _progressManager.Scheduler)
                      .ConfigureAwait(false);
            // in the event nothing is emitted...
            subject.OnNext(default!);
            _handler(request, subject, cancellationToken);
            return await task;
        }

        protected override TRegistrationOptions CreateRegistrationOptions(ClientCapabilities clientCapabilities) =>
            _registrationOptionsFactory(clientCapabilities);
    }

    public sealed class PartialResultWithInitialValueCapability<TParams, TResponse, TItem, TCapability> :
        AbstractHandlers.BaseCapability<TCapability>,
        IJsonRpcRequestHandler<TParams, TResponse?>,
        ICanBeIdentifiedHandler
        where TParams : IPartialItemWithInitialValueRequest<TResponse, TItem>
        where TItem : class?
        where TResponse : class?
        where TCapability : ICapability
    {
        private readonly Action<TParams, TCapability, IObserver<TItem>, CancellationToken> _handler;
        private readonly IProgressManager _progressManager;
        private readonly Func<TResponse, TItem?, TResponse> _factory;
        private readonly Func<TParams, TCapability, CancellationToken, Task<TResponse>> _initialValue;
        private readonly Guid _id;
        Guid ICanBeIdentifiedHandler.Id => _id;

        public PartialResultWithInitialValueCapability(
            Func<TParams, TCapability, CancellationToken, Task<TResponse>> initialValue,
            Action<TParams, TCapability, IObserver<TItem>, CancellationToken> handler,
            IProgressManager progressManager,
            Func<TResponse, TItem?, TResponse> factory
        ) :
            this(Guid.Empty, initialValue, handler, progressManager, factory)
        {
        }

        public PartialResultWithInitialValueCapability(
            Guid id,
            Func<TParams, TCapability, CancellationToken, Task<TResponse>> initialValue,
            Action<TParams, TCapability, IObserver<TItem>, CancellationToken> handler,
            IProgressManager progressManager,
            Func<TResponse, TItem?, TResponse> factory
        )
        {
            _id = id;
            _handler = handler;
            _progressManager = progressManager;
            _factory = factory;
            _initialValue = initialValue;
        }

        async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(TParams request, CancellationToken cancellationToken)
        {
            var observer = _progressManager.For(request, cancellationToken);
            if (observer != ProgressObserver<TResponse, TItem>.Noop)
            {
                observer.OnNext(await _initialValue(request, Capability, cancellationToken));
                _handler(request, Capability, observer, cancellationToken);
                await observer;
                return default;
            }

            using var subject = new AsyncSubject<TItem?>();
            var task = subject
                      .Aggregate(await _initialValue(request, Capability, cancellationToken), _factory)
                      .ToTask(cancellationToken, _progressManager.Scheduler)
                      .ConfigureAwait(false);
            // in the event nothing is emitted...
            subject.OnNext(default!);
            _handler(request, Capability, subject, cancellationToken);
            return await task;
        }
    }

    public sealed class PartialResultWithInitialValue<TParams, TResponse, TItem> :
        IJsonRpcRequestHandler<TParams, TResponse?>,
        ICanBeIdentifiedHandler
        where TParams : IPartialItemWithInitialValueRequest<TResponse, TItem>
        where TItem : class?
        where TResponse : class?
    {
        private readonly Action<TParams, IObserver<TItem>, CancellationToken> _handler;
        private readonly IProgressManager _progressManager;
        private readonly Func<TResponse, TItem?, TResponse> _factory;
        private readonly Func<TParams, CancellationToken, Task<TResponse>> _initialValue;
        private readonly Guid _id;
        Guid ICanBeIdentifiedHandler.Id => _id;

        public PartialResultWithInitialValue(
            Func<TParams, CancellationToken, Task<TResponse>> initialValue,
            Action<TParams, IObserver<TItem>, CancellationToken> handler,
            IProgressManager progressManager,
            Func<TResponse, TItem?, TResponse> factory
        ) :
            this(Guid.Empty, initialValue, handler, progressManager, factory)
        {
        }

        public PartialResultWithInitialValue(
            Guid id,
            Func<TParams, CancellationToken, Task<TResponse>> initialValue,
            Action<TParams, IObserver<TItem>, CancellationToken> handler,
            IProgressManager progressManager,
            Func<TResponse, TItem?, TResponse> factory
        )
        {
            _id = id;
            _handler = handler;
            _progressManager = progressManager;
            _factory = factory;
            _initialValue = initialValue;
        }

        async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(TParams request, CancellationToken cancellationToken)
        {
            var observer = _progressManager.For(request, cancellationToken);
            if (observer != ProgressObserver<TResponse, TItem>.Noop)
            {
                observer.OnNext(await _initialValue(request, cancellationToken));
                _handler(request, observer, cancellationToken);
                await observer;
                return default;
            }

            using var subject = new AsyncSubject<TItem?>();
            var task = subject
                      .Aggregate(await _initialValue(request, cancellationToken), _factory)
                      .ToTask(cancellationToken, _progressManager.Scheduler)
                      .ConfigureAwait(false);
            // in the event nothing is emitted...
            subject.OnNext(default!);
            _handler(request, subject, cancellationToken);
            return await task;
        }
    }

    public sealed class PartialResultsWithInitialValue<TParams, TResponse, TItem, TRegistrationOptions, TCapability> :
        AbstractHandlers.Base<TRegistrationOptions, TCapability>,
        IJsonRpcRequestHandler<TParams, TResponse?>,
        ICanBeIdentifiedHandler
        where TParams : IPartialItemsWithInitialValueRequest<TResponse, TItem>
        where TItem : class?
        where TResponse : IEnumerable<TItem>?
        where TRegistrationOptions : class, new()
        where TCapability : ICapability
    {
        private readonly Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> _handler;
        private readonly RegistrationOptionsDelegate<TRegistrationOptions, TCapability> _registrationOptionsFactory;
        private readonly IProgressManager _progressManager;
        private readonly Func<TResponse, IEnumerable<TItem>, TResponse> _factory;
        private readonly Func<TParams, TCapability, CancellationToken, Task<TResponse>> _initialValue;
        private readonly Guid _id;
        Guid ICanBeIdentifiedHandler.Id => _id;

        public PartialResultsWithInitialValue(
            Func<TParams, TCapability, CancellationToken, Task<TResponse>> initialValue,
            Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> handler,
            RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory, IProgressManager progressManager,
            Func<TResponse, IEnumerable<TItem>, TResponse> factory
        ) :
            this(Guid.Empty, initialValue, handler, registrationOptionsFactory, progressManager, factory)
        {
        }

        public PartialResultsWithInitialValue(
            Guid id,
            Func<TParams, TCapability, CancellationToken, Task<TResponse>> initialValue,
            Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> handler,
            RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory,
            IProgressManager progressManager, Func<TResponse, IEnumerable<TItem>, TResponse> factory
            
        )
        {
            _id = id;
            _handler = handler;
            _registrationOptionsFactory = registrationOptionsFactory;
            _progressManager = progressManager;
            _factory = factory;
            _initialValue = initialValue;
        }

        async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(TParams request, CancellationToken cancellationToken)
        {
            var observer = _progressManager.For(request, cancellationToken);
            if (observer != ProgressObserver<TResponse, IEnumerable<TItem>>.Noop)
            {
                observer.OnNext(await _initialValue(request, Capability, cancellationToken));
                _handler(request, observer, Capability, cancellationToken);
                await observer;
                return default;
            }

            using var subject = new Subject<IEnumerable<TItem>>();
            var task = subject
                      .Aggregate(await _initialValue(request, Capability, cancellationToken), _factory)
                      .ToTask(cancellationToken, _progressManager.Scheduler)
                      .ConfigureAwait(false);
            _handler(request, subject, Capability, cancellationToken);
            return await task;
        }

        protected internal override TRegistrationOptions CreateRegistrationOptions(TCapability capability, ClientCapabilities clientCapabilities) =>
            _registrationOptionsFactory(capability, clientCapabilities);
    }

    public sealed class PartialResultsWithInitialValue<TParams, TResponse, TItem, TRegistrationOptions> :
        AbstractHandlers.Base<TRegistrationOptions>,
        IJsonRpcRequestHandler<TParams, TResponse?>,
        ICanBeIdentifiedHandler
        where TParams : IPartialItemsWithInitialValueRequest<TResponse, TItem>
        where TItem : class?
        where TResponse : IEnumerable<TItem>?
        where TRegistrationOptions : class, new()
    {
        private readonly Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> _handler;
        private readonly RegistrationOptionsDelegate<TRegistrationOptions> _registrationOptionsFactory;
        private readonly IProgressManager _progressManager;
        private readonly Func<TResponse, IEnumerable<TItem>, TResponse> _factory;
        private readonly Func<TParams, CancellationToken, Task<TResponse>> _initialValue;
        private readonly Guid _id;
        Guid ICanBeIdentifiedHandler.Id => _id;

        public PartialResultsWithInitialValue(
            Func<TParams, CancellationToken, Task<TResponse>> initialValue,
            Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler,
            RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory,
            IProgressManager progressManager,
            Func<TResponse, IEnumerable<TItem>, TResponse> factory
            
        ) :
            this(Guid.Empty, initialValue, handler, registrationOptionsFactory, progressManager, factory)
        {
        }

        public PartialResultsWithInitialValue(
            Guid id,
            Func<TParams, CancellationToken, Task<TResponse>> initialValue,
            Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler,
            RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory,
            IProgressManager progressManager,
            Func<TResponse, IEnumerable<TItem>, TResponse> factory
        )
        {
            _id = id;
            _handler = handler;
            _registrationOptionsFactory = registrationOptionsFactory;
            _progressManager = progressManager;
            _factory = factory;
            _initialValue = initialValue;
        }

        async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(TParams request, CancellationToken cancellationToken)
        {
            var observer = _progressManager.For(request, cancellationToken);
            if (observer != ProgressObserver<TResponse, IEnumerable<TItem>>.Noop)
            {
                observer.OnNext(await _initialValue(request, cancellationToken));
                _handler(request, observer, cancellationToken);
                await observer;
                return default;
            }

            using var subject = new Subject<IEnumerable<TItem>>();
            var task = subject
                      .Aggregate(await _initialValue(request, cancellationToken), _factory)
                      .ToTask(cancellationToken, _progressManager.Scheduler)
                      .ConfigureAwait(false);
            _handler(request, subject, cancellationToken);
            return await task;
        }

        protected override TRegistrationOptions CreateRegistrationOptions(ClientCapabilities clientCapabilities) =>
            _registrationOptionsFactory(clientCapabilities);
    }

    public sealed class PartialResultsWithInitialValueCapability<TParams, TResponse, TItem, TCapability> :
        AbstractHandlers.BaseCapability<TCapability>,
        IJsonRpcRequestHandler<TParams, TResponse?>,
        ICanBeIdentifiedHandler
        where TParams : IPartialItemsWithInitialValueRequest<TResponse, TItem>
        where TItem : class?
        where TResponse : IEnumerable<TItem>?
        where TCapability : ICapability
    {
        private readonly Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> _handler;
        private readonly IProgressManager _progressManager;
        private readonly Func<TResponse, IEnumerable<TItem>, TResponse> _factory;
        private readonly Func<TParams, TCapability, CancellationToken, Task<TResponse>> _initialValue;
        private readonly Guid _id;
        Guid ICanBeIdentifiedHandler.Id => _id;

        public PartialResultsWithInitialValueCapability(
            Func<TParams, TCapability, CancellationToken, Task<TResponse>> initialValue,
            Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> handler, IProgressManager progressManager,
            Func<TResponse, IEnumerable<TItem>, TResponse> factory
        ) :
            this(Guid.Empty, initialValue, handler, progressManager, factory)
        {
        }

        public PartialResultsWithInitialValueCapability(
            Guid id,
            Func<TParams, TCapability, CancellationToken, Task<TResponse>> initialValue,
            Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> handler, IProgressManager progressManager,
            Func<TResponse, IEnumerable<TItem>, TResponse> factory
        )
        {
            _id = id;
            _handler = handler;
            _progressManager = progressManager;
            _factory = factory;
            _initialValue = initialValue;
        }

        async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(TParams request, CancellationToken cancellationToken)
        {
            var observer = _progressManager.For(request, cancellationToken);
            if (observer != ProgressObserver<TResponse, IEnumerable<TItem>>.Noop)
            {
                observer.OnNext(await _initialValue(request, Capability, cancellationToken));
                _handler(request, observer, Capability, cancellationToken);
                await observer;
                return default;
            }

            using var subject = new Subject<IEnumerable<TItem>>();
            var task = subject
                      .Aggregate(await _initialValue(request, Capability, cancellationToken), _factory)
                      .ToTask(cancellationToken, _progressManager.Scheduler)
                      .ConfigureAwait(false);
            _handler(request, subject, Capability, cancellationToken);
            return await task;
        }
    }

    public sealed class PartialResultsWithInitialValue<TParams, TResponse, TItem> :
        IJsonRpcRequestHandler<TParams, TResponse?>,
        ICanBeIdentifiedHandler
        where TParams : IPartialItemsWithInitialValueRequest<TResponse, TItem>
        where TItem : class?
        where TResponse : IEnumerable<TItem>?
    {
        private readonly Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> _handler;
        private readonly IProgressManager _progressManager;
        private readonly Func<TResponse, IEnumerable<TItem>, TResponse> _factory;
        private readonly Func<TParams, CancellationToken, Task<TResponse>> _initialValue;
        private readonly Guid _id;
        Guid ICanBeIdentifiedHandler.Id => _id;

        public PartialResultsWithInitialValue(
            Func<TParams, CancellationToken, Task<TResponse>> initialValue,
            Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler, IProgressManager progressManager,
            Func<TResponse, IEnumerable<TItem>, TResponse> factory
            
        ) :
            this(Guid.Empty, initialValue, handler, progressManager, factory)
        {
            _handler = handler;
            _progressManager = progressManager;
            _factory = factory;
        }

        public PartialResultsWithInitialValue(
            Guid id,
            Func<TParams, CancellationToken, Task<TResponse>> initialValue,
            Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler, IProgressManager progressManager,
            Func<TResponse, IEnumerable<TItem>, TResponse> factory
        )
        {
            _id = id;
            _handler = handler;
            _progressManager = progressManager;
            _factory = factory;
            _initialValue = initialValue;
        }

        async Task<TResponse?> IRequestHandler<TParams, TResponse?>.Handle(TParams request, CancellationToken cancellationToken)
        {
            var observer = _progressManager.For(request, cancellationToken);
            if (observer != ProgressObserver<TResponse, IEnumerable<TItem>>.Noop)
            {
                observer.OnNext(await _initialValue(request, cancellationToken));
                _handler(request, observer, cancellationToken);
                await observer;
                return default;
            }

            using var subject = new Subject<IEnumerable<TItem>>();
            var task = subject
                      .Aggregate(await _initialValue(request, cancellationToken), _factory)
                      .ToTask(cancellationToken, _progressManager.Scheduler)
                      .ConfigureAwait(false);
            _handler(request, subject, cancellationToken);
            return await task;
        }
    }
}
