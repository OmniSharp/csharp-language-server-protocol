using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
    [Serial, Method(WorkspaceNames.ExecuteCommand, Direction.ClientToServer)]
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
        public virtual void SetCapability(ExecuteCommandCapability capability) => Capability = capability;
        protected ExecuteCommandCapability Capability { get; private set; }
    }

    public static class ExecuteCommandExtensions
    {
public static ILanguageServerRegistry OnExecuteCommand(this ILanguageServerRegistry registry,
            Func<ExecuteCommandParams, ExecuteCommandCapability, CancellationToken, Task>
                handler,
            ExecuteCommandRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ExecuteCommandRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.ExecuteCommand,
                new LanguageProtocolDelegatingHandlers.Request<ExecuteCommandParams, ExecuteCommandCapability,
                    ExecuteCommandRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnExecuteCommand(this ILanguageServerRegistry registry,
            Func<ExecuteCommandParams, ExecuteCommandCapability, Task> handler,
            ExecuteCommandRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ExecuteCommandRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.ExecuteCommand,
                new LanguageProtocolDelegatingHandlers.Request<ExecuteCommandParams, ExecuteCommandCapability,
                    ExecuteCommandRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnExecuteCommand(this ILanguageServerRegistry registry,
            Func<ExecuteCommandParams, CancellationToken, Task> handler,
            ExecuteCommandRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ExecuteCommandRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.ExecuteCommand,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<ExecuteCommandParams,
                    ExecuteCommandRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnExecuteCommand(this ILanguageServerRegistry registry,
            Func<ExecuteCommandParams, Task> handler,
            ExecuteCommandRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ExecuteCommandRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.ExecuteCommand,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<ExecuteCommandParams,
                    ExecuteCommandRegistrationOptions>(handler, registrationOptions));
        }

        public static Task RequestExecuteCommand(this IWorkspaceLanguageClient router, ExecuteCommandParams @params, CancellationToken cancellationToken = default)
        {
            return router.SendRequest(@params, cancellationToken);
        }
    }
}
