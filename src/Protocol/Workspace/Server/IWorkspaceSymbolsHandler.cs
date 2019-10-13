using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(WorkspaceNames.WorkspaceSymbol)]
    public interface IWorkspaceSymbolsHandler : IJsonRpcRequestHandler<WorkspaceSymbolParams, Container<SymbolInformation>>, ICapability<WorkspaceSymbolCapability>, IRegistration<WorkspaceSymbolRegistrationOptions> { }

    public abstract class WorkspaceSymbolsHandler : IWorkspaceSymbolsHandler
    {
        protected WorkspaceSymbolCapability Capability { get; private set; }
        private readonly WorkspaceSymbolRegistrationOptions _options;
        protected readonly ProgressManager _progressManager;
        public WorkspaceSymbolsHandler(WorkspaceSymbolRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            _progressManager = progressManager;
        }

        public WorkspaceSymbolRegistrationOptions GetRegistrationOptions() => _options;

        public async Task<Container<SymbolInformation>> Handle(WorkspaceSymbolParams request, CancellationToken cancellationToken)
        {
            using var partialResults = _progressManager.For(request, cancellationToken);
            using var progressReporter = _progressManager.Delegate(request, cancellationToken);
            return await Handle(request, partialResults, progressReporter, cancellationToken).ConfigureAwait(false);
        }

        public abstract Task<Container<SymbolInformation>> Handle(
            WorkspaceSymbolParams request,
            IObserver<Container<SymbolInformation>> partialResults,
            WorkDoneProgressReporter progressReporter,
            CancellationToken cancellationToken
        );

        public virtual void SetCapability(WorkspaceSymbolCapability capability) => Capability = capability;
    }

    public static class WorkspaceSymbolsHandlerExtensions
    {
        public static IDisposable OnWorkspaceSymbols(
            this ILanguageServerRegistry registry,
            Func<WorkspaceSymbolParams, IObserver<Container<SymbolInformation>>, WorkDoneProgressReporter, CancellationToken, Task<Container<SymbolInformation>>> handler,
            Action<WorkspaceSymbolCapability> setCapability = null,
            WorkspaceSymbolRegistrationOptions registrationOptions = null)
        {
            registrationOptions ??= new WorkspaceSymbolRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : WorkspaceSymbolsHandler
        {
            private readonly Func<WorkspaceSymbolParams, IObserver<Container<SymbolInformation>>, WorkDoneProgressReporter, CancellationToken, Task<Container<SymbolInformation>>> _handler;
            private readonly Action<WorkspaceSymbolCapability> _setCapability;

            public DelegatingHandler(
                Func<WorkspaceSymbolParams, IObserver<Container<SymbolInformation>>, WorkDoneProgressReporter, CancellationToken, Task<Container<SymbolInformation>>> handler,
                ProgressManager progressManager,
                Action<WorkspaceSymbolCapability> setCapability,
                WorkspaceSymbolRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<SymbolInformation>> Handle(
                WorkspaceSymbolParams request,
                IObserver<Container<SymbolInformation>> partialResults,
                WorkDoneProgressReporter progressReporter,
                CancellationToken cancellationToken
            ) => _handler.Invoke(request, partialResults, progressReporter, cancellationToken);
            public override void SetCapability(WorkspaceSymbolCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
