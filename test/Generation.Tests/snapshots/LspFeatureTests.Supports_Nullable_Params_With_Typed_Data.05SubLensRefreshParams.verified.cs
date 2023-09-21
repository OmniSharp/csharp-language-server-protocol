//HintName: SubLensRefreshParams.cs
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
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Test.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace.Test;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace.Test
{
    [Parallel, Method(WorkspaceNames.CodeLensRefresh, Direction.ServerToClient)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface ISubLensRefreshHandler : IJsonRpcNotificationHandler<SubLensRefreshParams>, ICapability<SubLensWorkspaceClientCapabilities>
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class SubLensRefreshHandlerBase : AbstractHandlers.NotificationCapability<SubLensRefreshParams, SubLensWorkspaceClientCapabilities>, ISubLensRefreshHandler
    {
    }
}
#nullable restore

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace.Test
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class SubLensRefreshExtensions
    {
        public static ILanguageClientRegistry OnSubLensRefresh(this ILanguageClientRegistry registry, Action<SubLensRefreshParams> handler) => registry.AddHandler(WorkspaceNames.CodeLensRefresh, NotificationHandler.For(handler));
        public static ILanguageClientRegistry OnSubLensRefresh(this ILanguageClientRegistry registry, Func<SubLensRefreshParams, Task> handler) => registry.AddHandler(WorkspaceNames.CodeLensRefresh, NotificationHandler.For(handler));
        public static ILanguageClientRegistry OnSubLensRefresh(this ILanguageClientRegistry registry, Action<SubLensRefreshParams, CancellationToken> handler) => registry.AddHandler(WorkspaceNames.CodeLensRefresh, NotificationHandler.For(handler));
        public static ILanguageClientRegistry OnSubLensRefresh(this ILanguageClientRegistry registry, Func<SubLensRefreshParams, CancellationToken, Task> handler) => registry.AddHandler(WorkspaceNames.CodeLensRefresh, NotificationHandler.For(handler));
        public static ILanguageClientRegistry OnSubLensRefresh(this ILanguageClientRegistry registry, Action<SubLensRefreshParams, SubLensWorkspaceClientCapabilities, CancellationToken> handler) => registry.AddHandler(WorkspaceNames.CodeLensRefresh, new LanguageProtocolDelegatingHandlers.NotificationCapability<SubLensRefreshParams, SubLensWorkspaceClientCapabilities>(HandlerAdapter<SubLensWorkspaceClientCapabilities>.Adapt<SubLensRefreshParams>(handler)));
        public static ILanguageClientRegistry OnSubLensRefresh(this ILanguageClientRegistry registry, Func<SubLensRefreshParams, SubLensWorkspaceClientCapabilities, CancellationToken, Task> handler) => registry.AddHandler(WorkspaceNames.CodeLensRefresh, new LanguageProtocolDelegatingHandlers.NotificationCapability<SubLensRefreshParams, SubLensWorkspaceClientCapabilities>(HandlerAdapter<SubLensWorkspaceClientCapabilities>.Adapt<SubLensRefreshParams>(handler)));
        public static void SendSubLensRefresh(this IWorkspaceLanguageServer mediator, SubLensRefreshParams request) => mediator.SendNotification(request);
        public static void SendSubLensRefresh(this ILanguageServer mediator, SubLensRefreshParams request) => mediator.SendNotification(request);
    }
#nullable restore
}