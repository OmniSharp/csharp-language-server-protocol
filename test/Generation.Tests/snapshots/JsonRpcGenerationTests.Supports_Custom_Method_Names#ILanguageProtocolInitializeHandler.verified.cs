﻿//HintName: ILanguageProtocolInitializeHandler.cs
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
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Test;

namespace Test
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class LanguageProtocolInitializeExtensions
    {
        public static ILanguageServerRegistry OnLanguageProtocolInitialize(this ILanguageServerRegistry registry, Func<InitializeParams, Task<InitializeResult>> handler) => registry.AddHandler(GeneralNames.Initialize, RequestHandler.For(handler));
        public static ILanguageServerRegistry OnLanguageProtocolInitialize(this ILanguageServerRegistry registry, Func<InitializeParams, CancellationToken, Task<InitializeResult>> handler) => registry.AddHandler(GeneralNames.Initialize, RequestHandler.For(handler));
        public static Task<InitializeResult> RequestLanguageProtocolInitialize(this ITextDocumentLanguageClient mediator, InitializeParams request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
    }
#nullable restore
}