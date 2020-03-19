using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Serial, Method(DocumentNames.PrepareRename)]
    public interface IPrepareRenameHandler : IJsonRpcRequestHandler<PrepareRenameParams, RangeOrPlaceholderRange>, IRegistration<object>, ICapability<RenameCapability> { }

    public abstract class PrepareRenameHandler : IPrepareRenameHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public PrepareRenameHandler(TextDocumentRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public object GetRegistrationOptions() => new object();
        public abstract Task<RangeOrPlaceholderRange> Handle(PrepareRenameParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(RenameCapability capability) => Capability = capability;
        protected RenameCapability Capability { get; private set; }
    }

    public static class PrepareRenameHandlerExtensions
    {
        public static IDisposable OnPrepareRename(
            this ILanguageServerRegistry registry,
            Func<PrepareRenameParams, CancellationToken, Task<RangeOrPlaceholderRange>> handler,
            TextDocumentRegistrationOptions registrationOptions = null,
            Action<RenameCapability> setCapability = null)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : PrepareRenameHandler
        {
            private readonly Func<PrepareRenameParams, CancellationToken, Task<RangeOrPlaceholderRange>> _handler;
            private readonly Action<RenameCapability> _setCapability;

            public DelegatingHandler(
                Func<PrepareRenameParams, CancellationToken, Task<RangeOrPlaceholderRange>> handler,
                Action<RenameCapability> setCapability,
                TextDocumentRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<RangeOrPlaceholderRange> Handle(PrepareRenameParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(RenameCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
