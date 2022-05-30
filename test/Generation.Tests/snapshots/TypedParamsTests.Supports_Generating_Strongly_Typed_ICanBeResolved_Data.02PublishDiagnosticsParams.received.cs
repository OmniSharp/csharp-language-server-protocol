//HintName: PublishDiagnosticsParams.cs
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Test.Document;
using Test.Models;

#nullable enable
namespace Test.Document
{
    [Parallel, Method(TextDocumentNames.PublishDiagnostics, Direction.ServerToClient)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface IPublishDiagnosticsHandler : IJsonRpcNotificationHandler<PublishDiagnosticsParams>, ICapability<PublishDiagnosticsCapability>
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class PublishDiagnosticsHandlerBase : AbstractHandlers.NotificationCapability<PublishDiagnosticsParams, PublishDiagnosticsCapability>, IPublishDiagnosticsHandler
    {
    }
}
#nullable restore

namespace Test.Document
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class PublishDiagnosticsExtensions
    {
        public static void PublishDiagnostics(this ITextDocumentLanguageServer mediator, PublishDiagnosticsParams request) => mediator.SendNotification(request);
        public static void PublishDiagnostics(this ILanguageServer mediator, PublishDiagnosticsParams request) => mediator.SendNotification(request);
    }
#nullable restore
}