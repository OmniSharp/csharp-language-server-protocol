//HintName: ExecuteCommandParams1.cs
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Test;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
    [Serial, Method(WorkspaceNames.ExecuteCommand, Direction.ClientToServer)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface IExecuteCommandHandler<T> : IJsonRpcRequestHandler<ExecuteCommandParams<T>, T>, IRegistration<ExecuteCommandRegistrationOptions, ExecuteCommandCapability>
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class ExecuteCommandHandlerBase<T> : AbstractHandlers.Request<ExecuteCommandParams<T>, T, ExecuteCommandRegistrationOptions, ExecuteCommandCapability>, IExecuteCommandHandler<T>
    {
    }
}
#nullable restore

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class ExecuteCommandExtensions1
    {
        public static ILanguageServerRegistry OnExecuteCommand<T>(this ILanguageServerRegistry registry, Func<ExecuteCommandParams<T>, Task<T>> handler, RegistrationOptionsDelegate<ExecuteCommandRegistrationOptions, ExecuteCommandCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.ExecuteCommand, new LanguageProtocolDelegatingHandlers.Request<ExecuteCommandParams<T>, T, ExecuteCommandRegistrationOptions, ExecuteCommandCapability>(HandlerAdapter<ExecuteCommandCapability>.Adapt<ExecuteCommandParams<T>, T>(handler), RegistrationAdapter<ExecuteCommandCapability>.Adapt<ExecuteCommandRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnExecuteCommand<T>(this ILanguageServerRegistry registry, Func<ExecuteCommandParams<T>, CancellationToken, Task<T>> handler, RegistrationOptionsDelegate<ExecuteCommandRegistrationOptions, ExecuteCommandCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.ExecuteCommand, new LanguageProtocolDelegatingHandlers.Request<ExecuteCommandParams<T>, T, ExecuteCommandRegistrationOptions, ExecuteCommandCapability>(HandlerAdapter<ExecuteCommandCapability>.Adapt<ExecuteCommandParams<T>, T>(handler), RegistrationAdapter<ExecuteCommandCapability>.Adapt<ExecuteCommandRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnExecuteCommand<T>(this ILanguageServerRegistry registry, Func<ExecuteCommandParams<T>, ExecuteCommandCapability, CancellationToken, Task<T>> handler, RegistrationOptionsDelegate<ExecuteCommandRegistrationOptions, ExecuteCommandCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.ExecuteCommand, new LanguageProtocolDelegatingHandlers.Request<ExecuteCommandParams<T>, T, ExecuteCommandRegistrationOptions, ExecuteCommandCapability>(HandlerAdapter<ExecuteCommandCapability>.Adapt<ExecuteCommandParams<T>, T>(handler), RegistrationAdapter<ExecuteCommandCapability>.Adapt<ExecuteCommandRegistrationOptions>(registrationOptions)));
        }

        public static Task<T> ExecuteCommand<T>(this IWorkspaceLanguageClient mediator, ExecuteCommandParams<T> request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
        public static Task<T> ExecuteCommand<T>(this ILanguageClient mediator, ExecuteCommandParams<T> request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
    }
#nullable restore
}