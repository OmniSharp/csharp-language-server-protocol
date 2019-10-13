using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable once CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(DocumentNames.SelectionRange)]
    public interface ISelectionRangeHandler : IJsonRpcRequestHandler<SelectionRangeParam, Container<SelectionRange>>, IRegistration<SelectionRangeRegistrationOptions>, ICapability<SelectionRangeClientCapabilities> { }

    public abstract class SelectionRangeHandler : ISelectionRangeHandler
    {
        private readonly SelectionRangeRegistrationOptions _options;
        private readonly ProgressManager _progressManager;
        public SelectionRangeHandler(SelectionRangeRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            _progressManager = progressManager;
        }

        public SelectionRangeRegistrationOptions GetRegistrationOptions() => _options;

        public Task<Container<SelectionRange>> Handle(SelectionRangeParam request, CancellationToken cancellationToken)
        {
            var partialResults = _progressManager.For(request, cancellationToken);
            var createReporter = _progressManager.Delegate(request, cancellationToken);
            return Handle(request, partialResults, createReporter, cancellationToken);
        }

        public abstract Task<Container<SelectionRange>> Handle(
            SelectionRangeParam request,
            IObserver<Container<SelectionRange>> partialResults,
            Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
            CancellationToken cancellationToken
        );

        public virtual void SetCapability(SelectionRangeClientCapabilities capability) => Capability = capability;
        protected SelectionRangeClientCapabilities Capability { get; private set; }
    }

    public static class SelectionRangeHandlerExtensions
    {
        public static IDisposable OnSelectionRange(
            this ILanguageServerRegistry registry,
            Func<SelectionRangeParam, IObserver<Container<SelectionRange>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<SelectionRange>>> handler,
            SelectionRangeRegistrationOptions registrationOptions = null,
            Action<SelectionRangeClientCapabilities> setCapability = null)
        {
            registrationOptions = registrationOptions ?? new SelectionRangeRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : SelectionRangeHandler
        {
            private readonly Func<SelectionRangeParam, IObserver<Container<SelectionRange>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<SelectionRange>>> _handler;
            private readonly Action<SelectionRangeClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<SelectionRangeParam, IObserver<Container<SelectionRange>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<SelectionRange>>> handler,
                ProgressManager progressManager,
                Action<SelectionRangeClientCapabilities> setCapability,
                SelectionRangeRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<SelectionRange>> Handle(
                SelectionRangeParam request,
                IObserver<Container<SelectionRange>> partialResults,
                Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
                CancellationToken cancellationToken
            ) => _handler.Invoke(request, partialResults, createReporter, cancellationToken);
            public override void SetCapability(SelectionRangeClientCapabilities capability) => _setCapability?.Invoke(capability);

        }
    }
}
