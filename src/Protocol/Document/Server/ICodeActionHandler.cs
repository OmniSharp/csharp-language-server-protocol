using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(DocumentNames.CodeAction)]
    public interface ICodeActionHandler : IJsonRpcRequestHandler<CodeActionParams, CommandOrCodeActionContainer>, IRegistration<CodeActionRegistrationOptions>, ICapability<CodeActionCapability> { }

    public abstract class CodeActionHandler : ICodeActionHandler
    {
        private readonly CodeActionRegistrationOptions _options;
        protected ProgressManager ProgressManager { get; }

        public CodeActionHandler(CodeActionRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public CodeActionRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<CommandOrCodeActionContainer> Handle(CodeActionParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(CodeActionCapability capability) => Capability = capability;
        protected CodeActionCapability Capability { get; private set; }
    }

    public static class CodeActionHandlerExtensions
    {
        public static IDisposable OnCodeAction(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, CancellationToken, Task<CommandOrCodeActionContainer>> handler,
            CodeActionRegistrationOptions registrationOptions = null,
            Action<CodeActionCapability> setCapability = null)
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        internal class DelegatingHandler : CodeActionHandler
        {
            private readonly Func<CodeActionParams, CancellationToken, Task<CommandOrCodeActionContainer>> _handler;
            private readonly Action<CodeActionCapability> _setCapability;

            public DelegatingHandler(
                Func<CodeActionParams, CancellationToken, Task<CommandOrCodeActionContainer>> handler,
                ProgressManager progressManager,
                Action<CodeActionCapability> setCapability,
                CodeActionRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<CommandOrCodeActionContainer> Handle(CodeActionParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(CodeActionCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
