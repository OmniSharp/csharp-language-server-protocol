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
    [Serial, Method(WorkspaceNames.ExecuteCommand)]
    public interface IExecuteCommandHandler : IJsonRpcRequestHandler<ExecuteCommandParams>, IRegistration<ExecuteCommandRegistrationOptions>, ICapability<ExecuteCommandCapability> { }

    public abstract class ExecuteCommandHandler : IExecuteCommandHandler
    {
        private readonly ExecuteCommandRegistrationOptions _options;
        public ExecuteCommandHandler(ExecuteCommandRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public ExecuteCommandRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Unit> Handle(ExecuteCommandParams request, CancellationToken cancellationToken);
        public abstract void SetCapability(ExecuteCommandCapability capability);
    }

    public static class ExecuteCommandHandlerExtensions
    {
        public static IDisposable OnExecuteCommand(
            this ILanguageServerRegistry registry,
            Func<ExecuteCommandParams, CancellationToken, Task<Unit>> handler,
            ExecuteCommandRegistrationOptions registrationOptions = null,
            Action<ExecuteCommandCapability> setCapability = null)
        {
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : ExecuteCommandHandler
        {
            private readonly Func<ExecuteCommandParams, CancellationToken, Task<Unit>> _handler;
            private readonly Action<ExecuteCommandCapability> _setCapability;

            public DelegatingHandler(
                Func<ExecuteCommandParams, CancellationToken, Task<Unit>> handler,
                Action<ExecuteCommandCapability> setCapability,
                ExecuteCommandRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Unit> Handle(ExecuteCommandParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(ExecuteCommandCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
