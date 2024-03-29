﻿//HintName: AttachRequestArguments.cs
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
    [Method("attach", Direction.ClientToServer)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface IAttachRequestHandler<in T> : IJsonRpcRequestHandler<T, AttachResponse> where T : AttachRequestArguments
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class AttachRequestHandlerBase<T> : AbstractHandlers.Request<T, AttachResponse>, IAttachRequestHandler<T> where T : AttachRequestArguments
    {
    }

    public partial interface IAttachRequestHandler : IAttachRequestHandler<AttachRequestArguments>
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class AttachRequestHandlerBase : AttachRequestHandlerBase<AttachRequestArguments>, IAttachRequestHandler
    {
    }
}
#nullable restore

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Bogus
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class AttachRequestExtensions
    {
        public static IDebugAdapterServerRegistry OnAttachRequest(this IDebugAdapterServerRegistry registry, Func<AttachRequestArguments, Task<AttachResponse>> handler) => registry.AddHandler("attach", RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnAttachRequest(this IDebugAdapterServerRegistry registry, Func<AttachRequestArguments, CancellationToken, Task<AttachResponse>> handler) => registry.AddHandler("attach", RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnAttachRequest<T>(this IDebugAdapterServerRegistry registry, Func<T, Task<AttachResponse>> handler)
            where T : AttachRequestArguments => registry.AddHandler("attach", RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnAttachRequest<T>(this IDebugAdapterServerRegistry registry, Func<T, CancellationToken, Task<AttachResponse>> handler)
            where T : AttachRequestArguments => registry.AddHandler("attach", RequestHandler.For(handler));
        public static Task<AttachResponse> AttachRequest(this IDebugAdapterClient mediator, AttachRequestArguments request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
    }
#nullable restore
}