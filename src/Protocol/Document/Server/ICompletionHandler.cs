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
    public interface ICompletionHandler : IJsonRpcRequestHandler<CompletionParams, CompletionList>, IRegistration<CompletionRegistrationOptions>, ICapability<CompletionCapability> { }

    [Serial, Method(DocumentNames.CompletionResolve)]
    public interface ICompletionResolveHandler : ICanBeResolvedHandler<CompletionItem> { }

    public abstract class CompletionHandler : ICompletionHandler, ICompletionResolveHandler
    {
        protected readonly CompletionRegistrationOptions _options;

        public CompletionHandler(CompletionRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public CompletionRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken);
        public abstract Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken);
        public abstract bool CanResolve(CompletionItem value);
        public virtual void SetCapability(CompletionCapability capability) => Capability = capability;
        protected CompletionCapability Capability { get; private set; }
    }

    public static class CompletionHandlerExtensions
    {
        public static IDisposable OnCompletion(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, CancellationToken, Task<CompletionList>> handler,
            Func<CompletionItem, CancellationToken, Task<CompletionItem>> resolveHandler = null,
            Func<CompletionItem, bool> canResolve = null,
            CompletionRegistrationOptions registrationOptions = null,
            Action<CompletionCapability> setCapability = null)
        {
            registrationOptions = registrationOptions ?? new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            return registry.AddHandlers(new DelegatingHandler(handler, resolveHandler, canResolve, setCapability, registrationOptions));
        }

        class DelegatingHandler : CompletionHandler
        {
            private readonly Func<CompletionParams, CancellationToken, Task<CompletionList>> _handler;
            private readonly Func<CompletionItem, CancellationToken, Task<CompletionItem>> _resolveHandler;
            private readonly Func<CompletionItem, bool> _canResolve;
            private readonly Action<CompletionCapability> _setCapability;

            public DelegatingHandler(
                Func<CompletionParams, CancellationToken, Task<CompletionList>> handler,
                Func<CompletionItem, CancellationToken, Task<CompletionItem>> resolveHandler,
                Func<CompletionItem, bool> canResolve,
                Action<CompletionCapability> setCapability,
                CompletionRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _resolveHandler = resolveHandler;
                _canResolve = canResolve;
                _setCapability = setCapability;
            }

            public override Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken) => _resolveHandler.Invoke(request, cancellationToken);
            public override bool CanResolve(CompletionItem value) => _canResolve.Invoke(value);
            public override void SetCapability(CompletionCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
