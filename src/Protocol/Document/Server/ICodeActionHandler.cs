using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(DocumentNames.CodeAction)]
    public interface ICodeActionHandler : IJsonRpcRequestHandler<CodeActionParams, CommandOrCodeActionContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<CodeActionCapability> { }

    public abstract class CodeActionHandler : ICodeActionHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public CodeActionHandler(TextDocumentRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<CommandOrCodeActionContainer> Handle(CodeActionParams request, CancellationToken cancellationToken);
        public abstract void SetCapability(CodeActionCapability capability);
    }

    public static class CodeActionHandlerExtensions
    {
        public static IDisposable OnCodeAction(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, CancellationToken, Task<CommandOrCodeActionContainer>> handler,
            TextDocumentRegistrationOptions registrationOptions = null,
            Action<CodeActionCapability> setCapability = null)
        {
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        internal class DelegatingHandler : CodeActionHandler
        {
            private readonly Func<CodeActionParams, CancellationToken, Task<CommandOrCodeActionContainer>> _handler;
            private readonly Action<CodeActionCapability> _setCapability;

            public DelegatingHandler(
                Func<CodeActionParams, CancellationToken, Task<CommandOrCodeActionContainer>> handler,
                Action<CodeActionCapability> setCapability,
                TextDocumentRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<CommandOrCodeActionContainer> Handle(CodeActionParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(CodeActionCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
