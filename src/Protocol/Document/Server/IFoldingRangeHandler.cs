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
    public interface IFoldingRangeHandler : IJsonRpcRequestHandler<FoldingRangeParam, Container<FoldingRange>>, IRegistration<FoldingRangeRegistrationOptions>, ICapability<FoldingRangeCapability> { }

    public abstract class FoldingRangeHandler : IFoldingRangeHandler
    {
        private readonly FoldingRangeRegistrationOptions _options;
        protected ProgressManager ProgressManager { get; }
        public FoldingRangeHandler(FoldingRangeRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public FoldingRangeRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Container<FoldingRange>> Handle(FoldingRangeParam request, CancellationToken cancellationToken);
        public virtual void SetCapability(FoldingRangeCapability capability) => Capability = capability;
        protected FoldingRangeCapability Capability { get; private set; }
    }

    public static class FoldingRangeHandlerExtensions
    {
        public static IDisposable OnFoldingRange(
            this ILanguageServerRegistry registry,
            Func<FoldingRangeParam, CancellationToken, Task<Container<FoldingRange>>> handler,
            FoldingRangeRegistrationOptions registrationOptions = null,
            Action<FoldingRangeCapability> setCapability = null)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : FoldingRangeHandler
        {
            private readonly Func<FoldingRangeParam, CancellationToken, Task<Container<FoldingRange>>> _handler;
            private readonly Action<FoldingRangeCapability> _setCapability;

            public DelegatingHandler(
                Func<FoldingRangeParam, CancellationToken, Task<Container<FoldingRange>>> handler,
                ProgressManager progressManager,
                Action<FoldingRangeCapability> setCapability,
                FoldingRangeRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<FoldingRange>> Handle(FoldingRangeParam request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(FoldingRangeCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
