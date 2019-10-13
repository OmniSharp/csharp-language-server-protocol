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
        protected ProgressManager ProgressManager { get; }
        public SelectionRangeHandler(SelectionRangeRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public SelectionRangeRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Container<SelectionRange>> Handle(SelectionRangeParam request, CancellationToken cancellationToken);
        public virtual void SetCapability(SelectionRangeCapability capability) => Capability = capability;
        protected SelectionRangeCapability Capability { get; private set; }
    }

    public static class SelectionRangeHandlerExtensions
    {
        public static IDisposable OnSelectionRange(
            this ILanguageServerRegistry registry,
            Func<SelectionRangeParam, CancellationToken, Task<Container<SelectionRange>>> handler,
            SelectionRangeRegistrationOptions registrationOptions = null,
            Action<SelectionRangeCapability> setCapability = null)
        {
            registrationOptions ??= new SelectionRangeRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : SelectionRangeHandler
        {
            private readonly Func<SelectionRangeParam, CancellationToken, Task<Container<SelectionRange>>> _handler;
            private readonly Action<SelectionRangeCapability> _setCapability;

            public DelegatingHandler(
                Func<SelectionRangeParam, CancellationToken, Task<Container<SelectionRange>>> handler,
                ProgressManager progressManager,
                Action<SelectionRangeCapability> setCapability,
                SelectionRangeRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<SelectionRange>> Handle(SelectionRangeParam request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(SelectionRangeCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
