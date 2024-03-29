﻿//HintName: IExitHandler.cs
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
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class ExitExtensions
    {
        public static ILanguageServerRegistry OnExit(this ILanguageServerRegistry registry, Action<ExitParams> handler) => registry.AddHandler(GeneralNames.Exit, NotificationHandler.For(handler));
        public static ILanguageServerRegistry OnExit(this ILanguageServerRegistry registry, Func<ExitParams, Task> handler) => registry.AddHandler(GeneralNames.Exit, NotificationHandler.For(handler));
        public static ILanguageServerRegistry OnExit(this ILanguageServerRegistry registry, Action<ExitParams, CancellationToken> handler) => registry.AddHandler(GeneralNames.Exit, NotificationHandler.For(handler));
        public static ILanguageServerRegistry OnExit(this ILanguageServerRegistry registry, Func<ExitParams, CancellationToken, Task> handler) => registry.AddHandler(GeneralNames.Exit, NotificationHandler.For(handler));
        public static void SendExit(this ILanguageClient mediator, ExitParams request) => mediator.SendNotification(request);
    }
#nullable restore
}