﻿//HintName: LaunchRequestArguments.cs
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Bogus;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace OmniSharp.Extensions.DebugAdapter.Protocol.Bogus
{
    [Method("launch", Direction.ClientToServer)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface ILaunchRequestHandler<in T> : IJsonRpcRequestHandler<T, LaunchResponse?> where T : LaunchRequestArguments
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class LaunchRequestHandlerBase<T> : AbstractHandlers.Request<T, LaunchResponse?>, ILaunchRequestHandler<T> where T : LaunchRequestArguments
    {
    }

    public partial interface ILaunchRequestHandler : ILaunchRequestHandler<LaunchRequestArguments>
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class LaunchRequestHandlerBase : LaunchRequestHandlerBase<LaunchRequestArguments>, ILaunchRequestHandler
    {
    }
}
#nullable restore

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Bogus
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class LaunchRequestExtensions
    {
        public static IDebugAdapterServerRegistry OnLaunchRequest(this IDebugAdapterServerRegistry registry, Func<LaunchRequestArguments, Task<LaunchResponse?>> handler) => registry.AddHandler("launch", RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnLaunchRequest(this IDebugAdapterServerRegistry registry, Func<LaunchRequestArguments, CancellationToken, Task<LaunchResponse?>> handler) => registry.AddHandler("launch", RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnLaunchRequest<T>(this IDebugAdapterServerRegistry registry, Func<T, Task<LaunchResponse?>> handler)
            where T : LaunchRequestArguments => registry.AddHandler("launch", RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnLaunchRequest<T>(this IDebugAdapterServerRegistry registry, Func<T, CancellationToken, Task<LaunchResponse?>> handler)
            where T : LaunchRequestArguments => registry.AddHandler("launch", RequestHandler.For(handler));
        public static Task<LaunchResponse?> LaunchRequest(this IDebugAdapterClient mediator, LaunchRequestArguments request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
    }
#nullable restore
}