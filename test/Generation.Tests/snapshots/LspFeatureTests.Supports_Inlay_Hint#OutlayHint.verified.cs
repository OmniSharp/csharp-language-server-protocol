//HintName: OutlayHint.cs
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Test.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Parallel, Method(TextDocumentNames.InlayHintResolve, Direction.ClientToServer)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface IOutlayHintResolveHandler : IJsonRpcRequestHandler<OutlayHint, OutlayHint>, IDoesNotParticipateInRegistration, ICapability<OutlayHintWorkspaceClientCapabilities>, ICanBeIdentifiedHandler
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class OutlayHintResolveHandlerBase : AbstractHandlers.RequestCapability<OutlayHint, OutlayHint, OutlayHintWorkspaceClientCapabilities>, IOutlayHintResolveHandler
    {
        protected OutlayHintResolveHandlerBase(System.Guid id) : base()
        {
            _id = id;
        }

        protected OutlayHintResolveHandlerBase() : this(Guid.NewGuid())
        {
        }

        private readonly Guid _id;
        Guid ICanBeIdentifiedHandler.Id => _id;
    }
}
#nullable restore

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class OutlayHintResolveExtensions
    {
        public static ILanguageServerRegistry OnOutlayHintResolve(this ILanguageServerRegistry registry, Func<OutlayHint, Task<OutlayHint>> handler) => registry.AddHandler(TextDocumentNames.InlayHintResolve, RequestHandler.For(handler));
        public static ILanguageServerRegistry OnOutlayHintResolve(this ILanguageServerRegistry registry, Func<OutlayHint, CancellationToken, Task<OutlayHint>> handler) => registry.AddHandler(TextDocumentNames.InlayHintResolve, RequestHandler.For(handler));
        public static ILanguageServerRegistry OnOutlayHintResolve(this ILanguageServerRegistry registry, Func<OutlayHint, OutlayHintWorkspaceClientCapabilities, CancellationToken, Task<OutlayHint>> handler) => registry.AddHandler(TextDocumentNames.InlayHintResolve, new LanguageProtocolDelegatingHandlers.RequestCapability<OutlayHint, OutlayHint, OutlayHintWorkspaceClientCapabilities>(HandlerAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHint, OutlayHint>(handler)));
        public static Task<OutlayHint> ResolveOutlayHint(this ITextDocumentLanguageClient mediator, OutlayHint request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
        public static Task<OutlayHint> ResolveOutlayHint(this ILanguageClient mediator, OutlayHint request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
    }
#nullable restore
}