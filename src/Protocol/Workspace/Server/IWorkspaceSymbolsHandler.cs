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
    public interface IWorkspaceSymbolsHandler : IJsonRpcRequestHandler<WorkspaceSymbolParams, Container<SymbolInformation>>, ICapability<WorkspaceSymbolClientCapabilities>, IRegistration<WorkspaceSymbolRegistrationOptions> { }

    public abstract class WorkspaceSymbolsHandler : IWorkspaceSymbolsHandler
    {
        protected WorkspaceSymbolClientCapabilities Capability { get; private set; }
        private readonly WorkspaceSymbolRegistrationOptions _options;
        private readonly ProgressManager _progressManager;
        public WorkspaceSymbolsHandler(WorkspaceSymbolRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            _progressManager = progressManager;
        }

        public WorkspaceSymbolRegistrationOptions GetRegistrationOptions() => _options;

        public Task<Container<SymbolInformation>> Handle(WorkspaceSymbolParams request, CancellationToken cancellationToken)
        {
            var partialResults = _progressManager.For(request, cancellationToken);
            var createReporter = _progressManager.Delegate(request, cancellationToken);
            return Handle(request, partialResults, createReporter, cancellationToken);
        }

        public abstract Task<Container<SymbolInformation>> Handle(
            WorkspaceSymbolParams request,
            IObserver<Container<SymbolInformation>> partialResults,
            Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
            CancellationToken cancellationToken
        );

        public virtual void SetCapability(WorkspaceSymbolClientCapabilities capability) => Capability = capability;
    }

    public static class WorkspaceSymbolsHandlerExtensions
    {
        public static IDisposable OnWorkspaceSymbols(
            this ILanguageServerRegistry registry,
            Func<WorkspaceSymbolParams, IObserver<Container<SymbolInformation>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<SymbolInformation>>> handler,
            Action<WorkspaceSymbolClientCapabilities> setCapability = null,
            WorkspaceSymbolRegistrationOptions registrationOptions = null)
        {
            registrationOptions ??= new WorkspaceSymbolRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : WorkspaceSymbolsHandler
        {
            private readonly Func<WorkspaceSymbolParams, IObserver<Container<SymbolInformation>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<SymbolInformation>>> _handler;
            private readonly Action<WorkspaceSymbolClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<WorkspaceSymbolParams, IObserver<Container<SymbolInformation>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<SymbolInformation>>> handler,
                ProgressManager progressManager,
                Action<WorkspaceSymbolClientCapabilities> setCapability,
                WorkspaceSymbolRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<SymbolInformation>> Handle(
                WorkspaceSymbolParams request,
                IObserver<Container<SymbolInformation>> partialResults,
                Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
                CancellationToken cancellationToken
            ) => _handler.Invoke(request, partialResults, createReporter, cancellationToken);
            public override void SetCapability(WorkspaceSymbolClientCapabilities capability) => _setCapability?.Invoke(capability);
        }
    }
}
