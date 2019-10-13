using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(DocumentNames.Completion)]
    public interface ICompletionHandler : IJsonRpcRequestHandler<CompletionParams, CompletionList>, IRegistration<CompletionRegistrationOptions>, ICapability<CompletionClientCapabilities> { }

    [Serial, Method(DocumentNames.CompletionResolve)]
    public interface ICompletionResolveHandler : ICanBeResolvedHandler<CompletionItem> { }

    public abstract class CompletionHandler : ICompletionHandler, ICompletionResolveHandler
    {
        protected readonly CompletionRegistrationOptions _options;
        private readonly ProgressManager _progressManager;
        public CompletionHandler(CompletionRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            _progressManager = progressManager;
        }

        public CompletionRegistrationOptions GetRegistrationOptions() => _options;

        public Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
        {
            var partialResults = _progressManager.For(request, cancellationToken);
            var createReporter = _progressManager.Delegate(request, cancellationToken);
            return Handle(request, partialResults, createReporter, cancellationToken);
        }

        public abstract Task<CompletionList> Handle(
            CompletionParams request,
            IObserver<Container<CompletionItem>> partialResults,
            Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
            CancellationToken cancellationToken
        );

        public abstract Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken);
        public abstract bool CanResolve(CompletionItem value);
        public virtual void SetCapability(CompletionClientCapabilities capability) => Capability = capability;
        protected CompletionClientCapabilities Capability { get; private set; }
    }

    public static class CompletionHandlerExtensions
    {
        public static IDisposable OnCompletion(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, IObserver<Container<CompletionItem>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<CompletionList>> handler,
            Func<CompletionItem, CancellationToken, Task<CompletionItem>> resolveHandler = null,
            Func<CompletionItem, bool> canResolve = null,
            CompletionRegistrationOptions registrationOptions = null,
            Action<CompletionClientCapabilities> setCapability = null)
        {
            registrationOptions = registrationOptions ?? new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            return registry.AddHandlers(new DelegatingHandler(handler, resolveHandler, registry.ProgressManager, canResolve, setCapability, registrationOptions));
        }

        class DelegatingHandler : CompletionHandler
        {
            private readonly Func<CompletionParams, IObserver<Container<CompletionItem>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<CompletionList>> _handler;
            private readonly Func<CompletionItem, CancellationToken, Task<CompletionItem>> _resolveHandler;
            private readonly Func<CompletionItem, bool> _canResolve;
            private readonly Action<CompletionClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<CompletionParams, IObserver<Container<CompletionItem>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<CompletionList>> handler,
                Func<CompletionItem, CancellationToken, Task<CompletionItem>> resolveHandler,
                ProgressManager progressManager,
                Func<CompletionItem, bool> canResolve,
                Action<CompletionClientCapabilities> setCapability,
                CompletionRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _resolveHandler = resolveHandler;
                _canResolve = canResolve;
                _setCapability = setCapability;
            }

            public override Task<CompletionList> Handle(
                CompletionParams request,
                IObserver<Container<CompletionItem>> partialResults,
                Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
                CancellationToken cancellationToken
            ) => _handler.Invoke(request, partialResults, createReporter, cancellationToken);

            public override Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken) => _resolveHandler.Invoke(request, cancellationToken);
            public override bool CanResolve(CompletionItem value) => _canResolve.Invoke(value);
            public override void SetCapability(CompletionClientCapabilities capability) => _setCapability?.Invoke(capability);

        }
    }
}
