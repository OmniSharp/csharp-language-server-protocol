//HintName: ICapabilitiesHandler.cs
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events.Test;
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

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events.Test
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class CapabilitiesExtensions
    {
        public static IDebugAdapterClientRegistry OnCapabilities(this IDebugAdapterClientRegistry registry, Action<CapabilitiesEvent> handler) => registry.AddHandler(EventNames.Capabilities, NotificationHandler.For(handler));
        public static IDebugAdapterClientRegistry OnCapabilities(this IDebugAdapterClientRegistry registry, Func<CapabilitiesEvent, Task> handler) => registry.AddHandler(EventNames.Capabilities, NotificationHandler.For(handler));
        public static IDebugAdapterClientRegistry OnCapabilities(this IDebugAdapterClientRegistry registry, Action<CapabilitiesEvent, CancellationToken> handler) => registry.AddHandler(EventNames.Capabilities, NotificationHandler.For(handler));
        public static IDebugAdapterClientRegistry OnCapabilities(this IDebugAdapterClientRegistry registry, Func<CapabilitiesEvent, CancellationToken, Task> handler) => registry.AddHandler(EventNames.Capabilities, NotificationHandler.For(handler));
        public static void SendCapabilities(this IDebugAdapterServer mediator, CapabilitiesEvent request) => mediator.SendNotification(request);
    }
#nullable restore
}