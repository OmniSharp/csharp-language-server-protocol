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
    public interface ISelectionRangeHandler : IJsonRpcRequestHandler<SelectionRangeParam, Container<SelectionRange>>, IRegistration<SelectionRangeRegistrationOptions>, ICapability<SelectionRangeCapability> { }

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

        public async Task<Container<SelectionRange>> Handle(SelectionRangeParam request, CancellationToken cancellationToken)
        {
            using var partialResults = _progressManager.For(request, cancellationToken);
            using var progressReporter = _progressManager.Delegate(request, cancellationToken);
            return await Handle(request, partialResults, progressReporter, cancellationToken).ConfigureAwait(false);
        }

        public abstract Task<Container<SelectionRange>> Handle(
            SelectionRangeParam request,
            IObserver<Container<SelectionRange>> partialResults,
            WorkDoneProgressReporter progressReporter,
            CancellationToken cancellationToken
        );

        public virtual void SetCapability(SelectionRangeCapability capability) => Capability = capability;
        protected SelectionRangeCapability Capability { get; private set; }
    }

    public static class SelectionRangeHandlerExtensions
    {
        public static IDisposable OnSelectionRange(
            this ILanguageServerRegistry registry,
            Func<SelectionRangeParam, IObserver<Container<SelectionRange>>, WorkDoneProgressReporter, CancellationToken, Task<Container<SelectionRange>>> handler,
            SelectionRangeRegistrationOptions registrationOptions = null,
            Action<SelectionRangeCapability> setCapability = null)
        {
            registrationOptions ??= new SelectionRangeRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : SelectionRangeHandler
        {
            private readonly Func<SelectionRangeParam, IObserver<Container<SelectionRange>>, WorkDoneProgressReporter, CancellationToken, Task<Container<SelectionRange>>> _handler;
            private readonly Action<SelectionRangeCapability> _setCapability;

            public DelegatingHandler(
                Func<SelectionRangeParam, IObserver<Container<SelectionRange>>, WorkDoneProgressReporter, CancellationToken, Task<Container<SelectionRange>>> handler,
                ProgressManager progressManager,
                Action<SelectionRangeCapability> setCapability,
                SelectionRangeRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<SelectionRange>> Handle(
                SelectionRangeParam request,
                IObserver<Container<SelectionRange>> partialResults,
                WorkDoneProgressReporter progressReporter,
                CancellationToken cancellationToken
            ) => _handler.Invoke(request, partialResults, progressReporter, cancellationToken);
            public override void SetCapability(SelectionRangeCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
