using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
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
        private readonly ProgressManager _progressManager;

        public CodeActionHandler(CodeActionRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            _progressManager = progressManager;
        }

        public CodeActionRegistrationOptions GetRegistrationOptions() => _options;
        public Task<CommandOrCodeActionContainer> Handle(CodeActionParams request, CancellationToken cancellationToken)
        {
            var partialResults = _progressManager.For(request, cancellationToken);
            var progressReporter = _progressManager.Delegate(request, cancellationToken);
            return Handle(request, partialResults, progressReporter, cancellationToken);
        }

        public abstract Task<CommandOrCodeActionContainer> Handle(
            CodeActionParams request,
            IObserver<Container<CodeActionOrCommand>> partialResults,
            WorkDoneProgressReporter progressReporter,
            CancellationToken cancellationToken
        );

        public virtual void SetCapability(CodeActionCapability capability) => Capability = capability;
        protected CodeActionCapability Capability { get; private set; }
    }

    public static class CodeActionHandlerExtensions
    {
        public static IDisposable OnCodeAction(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, IObserver<Container<CodeActionOrCommand>>, WorkDoneProgressReporter, CancellationToken, Task<CommandOrCodeActionContainer>> handler,
            CodeActionRegistrationOptions registrationOptions = null,
            Action<CodeActionCapability> setCapability = null)
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        internal class DelegatingHandler : CodeActionHandler
        {
            private readonly Func<CodeActionParams, IObserver<Container<CodeActionOrCommand>>, WorkDoneProgressReporter, CancellationToken, Task<CommandOrCodeActionContainer>> _handler;
            private readonly Action<CodeActionCapability> _setCapability;

            public DelegatingHandler(
                Func<CodeActionParams, IObserver<Container<CodeActionOrCommand>>, WorkDoneProgressReporter, CancellationToken, Task<CommandOrCodeActionContainer>> handler,
                ProgressManager progressManager,
                Action<CodeActionCapability> setCapability,
                CodeActionRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<CommandOrCodeActionContainer> Handle(
                CodeActionParams request,
                IObserver<Container<CodeActionOrCommand>> partialResults,
                WorkDoneProgressReporter progressReporter,
                CancellationToken cancellationToken) => _handler.Invoke(request, partialResults, progressReporter, cancellationToken);
            public override void SetCapability(CodeActionCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
