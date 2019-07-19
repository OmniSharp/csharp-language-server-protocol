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
    public interface IWorkspaceSymbolsHandler : IJsonRpcRequestHandler<WorkspaceSymbolParams, SymbolInformationContainer>, ICapability<WorkspaceSymbolCapability>, IRegistration<object> { }

    public abstract class WorkspaceSymbolsHandler : IWorkspaceSymbolsHandler
    {
        protected WorkspaceSymbolCapability Capability { get; private set; }

        public object GetRegistrationOptions() => new object();
        public abstract Task<SymbolInformationContainer> Handle(WorkspaceSymbolParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(WorkspaceSymbolCapability capability) => Capability = capability;
    }

    public static class WorkspaceSymbolsHandlerExtensions
    {
        public static IDisposable OnWorkspaceSymbols(
            this ILanguageServerRegistry registry,
            Func<WorkspaceSymbolParams, CancellationToken, Task<SymbolInformationContainer>> handler,
            Action<WorkspaceSymbolCapability> setCapability = null)
        {
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability));
        }

        class DelegatingHandler : WorkspaceSymbolsHandler
        {
            private readonly Func<WorkspaceSymbolParams, CancellationToken, Task<SymbolInformationContainer>> _handler;
            private readonly Action<WorkspaceSymbolCapability> _setCapability;

            public DelegatingHandler(
                Func<WorkspaceSymbolParams, CancellationToken, Task<SymbolInformationContainer>> handler,
                Action<WorkspaceSymbolCapability> setCapability) : base()
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<SymbolInformationContainer> Handle(WorkspaceSymbolParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(WorkspaceSymbolCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
