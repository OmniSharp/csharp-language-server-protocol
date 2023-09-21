//HintName: WorkspaceSymbol.cs
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
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
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
    [Parallel, Method(WorkspaceNames.WorkspaceSymbolResolve, Direction.ClientToServer)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface IWorkspaceSymbolResolveHandler : IJsonRpcRequestHandler<WorkspaceSymbol, WorkspaceSymbol>, ICapability<WorkspaceSymbolCapability>
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class WorkspaceSymbolResolveHandlerBase : AbstractHandlers.RequestCapability<WorkspaceSymbol, WorkspaceSymbol, WorkspaceSymbolCapability>, IWorkspaceSymbolResolveHandler
    {
    }
}
#nullable restore

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class WorkspaceSymbolResolveExtensions
    {
        public static ILanguageServerRegistry OnWorkspaceSymbolResolve(this ILanguageServerRegistry registry, Func<WorkspaceSymbol, Task<WorkspaceSymbol>> handler) => registry.AddHandler(WorkspaceNames.WorkspaceSymbolResolve, RequestHandler.For(handler));
        public static ILanguageServerRegistry OnWorkspaceSymbolResolve(this ILanguageServerRegistry registry, Func<WorkspaceSymbol, CancellationToken, Task<WorkspaceSymbol>> handler) => registry.AddHandler(WorkspaceNames.WorkspaceSymbolResolve, RequestHandler.For(handler));
        public static ILanguageServerRegistry OnWorkspaceSymbolResolve(this ILanguageServerRegistry registry, Func<WorkspaceSymbol, WorkspaceSymbolCapability, CancellationToken, Task<WorkspaceSymbol>> handler) => registry.AddHandler(WorkspaceNames.WorkspaceSymbolResolve, new LanguageProtocolDelegatingHandlers.RequestCapability<WorkspaceSymbol, WorkspaceSymbol, WorkspaceSymbolCapability>(HandlerAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbol, WorkspaceSymbol>(handler)));
        public static Task<WorkspaceSymbol> ResolveWorkspaceSymbol(this IWorkspaceLanguageClient mediator, WorkspaceSymbol request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
        public static Task<WorkspaceSymbol> ResolveWorkspaceSymbol(this ILanguageClient mediator, WorkspaceSymbol request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
    }
#nullable restore
}