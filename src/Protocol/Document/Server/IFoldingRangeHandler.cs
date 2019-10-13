using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable once CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(DocumentNames.FoldingRange)]
    public interface IFoldingRangeHandler : IJsonRpcRequestHandler<FoldingRangeParam, Container<FoldingRange>>, IRegistration<FoldingRangeRegistrationOptions>, ICapability<FoldingRangeClientCapabilities> { }

    public abstract class FoldingRangeHandler : IFoldingRangeHandler
    {
        private readonly FoldingRangeRegistrationOptions _options;
        private readonly ProgressManager _progressManager;
        public FoldingRangeHandler(FoldingRangeRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            _progressManager = progressManager;
        }

        public FoldingRangeRegistrationOptions GetRegistrationOptions() => _options;

        public Task<Container<FoldingRange>> Handle(FoldingRangeParam request, CancellationToken cancellationToken)
        {
            var partialResults = _progressManager.For(request, cancellationToken);
            var createReporter = _progressManager.Delegate(request, cancellationToken);
            return Handle(request, partialResults, createReporter, cancellationToken);
        }

        public abstract Task<Container<FoldingRange>> Handle(
            FoldingRangeParam request,
            IObserver<Container<FoldingRange>> partialResults,
            Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
            CancellationToken cancellationToken
        );

        public virtual void SetCapability(FoldingRangeClientCapabilities capability) => Capability = capability;
        protected FoldingRangeClientCapabilities Capability { get; private set; }
    }

    public static class FoldingRangeHandlerExtensions
    {
        public static IDisposable OnFoldingRange(
            this ILanguageServerRegistry registry,
            Func<FoldingRangeParam, IObserver<Container<FoldingRange>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<FoldingRange>>> handler,
            FoldingRangeRegistrationOptions registrationOptions = null,
            Action<FoldingRangeClientCapabilities> setCapability = null)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : FoldingRangeHandler
        {
            private readonly Func<FoldingRangeParam, IObserver<Container<FoldingRange>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<FoldingRange>>> _handler;
            private readonly Action<FoldingRangeClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<FoldingRangeParam, IObserver<Container<FoldingRange>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<FoldingRange>>> handler,
                ProgressManager progressManager,
                Action<FoldingRangeClientCapabilities> setCapability,
                FoldingRangeRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<FoldingRange>> Handle(
                FoldingRangeParam request,
                IObserver<Container<FoldingRange>> partialResults,
                Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
                CancellationToken cancellationToken
            ) => _handler.Invoke(request, partialResults, createReporter, cancellationToken);
            public override void SetCapability(FoldingRangeClientCapabilities capability) => _setCapability?.Invoke(capability);

        }
    }
}
