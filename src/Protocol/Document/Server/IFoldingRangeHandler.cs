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
    public interface IFoldingRangeHandler : IJsonRpcRequestHandler<FoldingRangeRequestParam, Container<FoldingRange>>, IRegistration<TextDocumentRegistrationOptions>, ICapability<FoldingRangeCapability> { }

    public abstract class FoldingRangeHandler : IFoldingRangeHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public FoldingRangeHandler(TextDocumentRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Container<FoldingRange>> Handle(FoldingRangeRequestParam request, CancellationToken cancellationToken);
        public virtual void SetCapability(FoldingRangeCapability capability) => Capability = capability;
        protected FoldingRangeCapability Capability { get; private set; }
    }

    public static class FoldingRangeHandlerExtensions
    {
        public static IDisposable OnFoldingRange(
            this ILanguageServerRegistry registry,
            Func<FoldingRangeRequestParam, CancellationToken, Task<Container<FoldingRange>>> handler,
            TextDocumentRegistrationOptions registrationOptions = null,
            Action<FoldingRangeCapability> setCapability = null)
        {
            registrationOptions = registrationOptions ?? new TextDocumentRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : FoldingRangeHandler
        {
            private readonly Func<FoldingRangeRequestParam, CancellationToken, Task<Container<FoldingRange>>> _handler;
            private readonly Action<FoldingRangeCapability> _setCapability;

            public DelegatingHandler(
                Func<FoldingRangeRequestParam, CancellationToken, Task<Container<FoldingRange>>> handler,
                Action<FoldingRangeCapability> setCapability,
                TextDocumentRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<FoldingRange>> Handle(FoldingRangeRequestParam request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(FoldingRangeCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
