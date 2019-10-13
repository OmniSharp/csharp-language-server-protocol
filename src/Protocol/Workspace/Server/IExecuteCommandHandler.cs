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
    [Serial, Method(WorkspaceNames.ExecuteCommand)]
    public interface IExecuteCommandHandler : IJsonRpcRequestHandler<ExecuteCommandParams>, IRegistration<ExecuteCommandRegistrationOptions>, ICapability<ExecuteCommandClientCapabilities> { }

    public abstract class ExecuteCommandHandler : IExecuteCommandHandler
    {
        private readonly ExecuteCommandRegistrationOptions _options;
        private readonly ProgressManager _progressManager;

        public ExecuteCommandHandler(ExecuteCommandRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            _progressManager = progressManager;
        }

        public ExecuteCommandRegistrationOptions GetRegistrationOptions() => _options;

        public Task<Unit> Handle(
            ExecuteCommandParams request,
            CancellationToken cancellationToken)
        {
            var createReporter = _progressManager.Delegate(request, cancellationToken);
            return Handle(request, createReporter, cancellationToken);
        }

        public abstract Task<Unit> Handle(ExecuteCommandParams request, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter, CancellationToken cancellationToken);

        public virtual void SetCapability(ExecuteCommandClientCapabilities capability) => Capability = capability;
        protected ExecuteCommandClientCapabilities Capability { get; private set; }
    }

    public static class ExecuteCommandHandlerExtensions
    {
        public static IDisposable OnExecuteCommand(
            this ILanguageServerRegistry registry,
            Func<ExecuteCommandParams, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Unit>> handler,
            ExecuteCommandRegistrationOptions registrationOptions = null,
            Action<ExecuteCommandClientCapabilities> setCapability = null)
        {
            registrationOptions ??= new ExecuteCommandRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : ExecuteCommandHandler
        {
            private readonly Func<ExecuteCommandParams, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Unit>> _handler;
            private readonly Action<ExecuteCommandClientCapabilities> _setCapability;

            public DelegatingHandler(Func<ExecuteCommandParams, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Unit>> handler,
                ProgressManager progressManager,
                Action<ExecuteCommandClientCapabilities> setCapability,
                ExecuteCommandRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Unit> Handle(ExecuteCommandParams request, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter, CancellationToken cancellationToken) => _handler.Invoke(request, createReporter, cancellationToken);
            public override void SetCapability(ExecuteCommandClientCapabilities capability) => _setCapability?.Invoke(capability);

        }
    }
}
