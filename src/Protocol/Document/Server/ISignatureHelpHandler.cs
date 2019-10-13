using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(DocumentNames.SignatureHelp)]
    public interface ISignatureHelpHandler : IJsonRpcRequestHandler<SignatureHelpParams, SignatureHelp>, IRegistration<SignatureHelpRegistrationOptions>, ICapability<SignatureHelpClientCapabilities> { }

    public abstract class SignatureHelpHandler : ISignatureHelpHandler
    {
        private readonly SignatureHelpRegistrationOptions _options;
        public SignatureHelpHandler(SignatureHelpRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public SignatureHelpRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<SignatureHelp> Handle(SignatureHelpParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(SignatureHelpClientCapabilities capability) => Capability = capability;
        protected SignatureHelpClientCapabilities Capability { get; private set; }
    }

    public static class SignatureHelpHandlerExtensions
    {
        public static IDisposable OnSignatureHelp(
            this ILanguageServerRegistry registry,
            Func<SignatureHelpParams, CancellationToken, Task<SignatureHelp>> handler,
            SignatureHelpRegistrationOptions registrationOptions = null,
            Action<SignatureHelpClientCapabilities> setCapability = null)
        {
            registrationOptions ??= new SignatureHelpRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : SignatureHelpHandler
        {
            private readonly Func<SignatureHelpParams, CancellationToken, Task<SignatureHelp>> _handler;
            private readonly Action<SignatureHelpClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<SignatureHelpParams, CancellationToken, Task<SignatureHelp>> handler,
                Action<SignatureHelpClientCapabilities> setCapability,
                SignatureHelpRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<SignatureHelp> Handle(SignatureHelpParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(SignatureHelpClientCapabilities capability) => _setCapability?.Invoke(capability);

        }
    }
}
