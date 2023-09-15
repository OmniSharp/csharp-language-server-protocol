//HintName: SubLens.cs
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
using OmniSharp.Extensions.LanguageServer.Protocol.Document.Test;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Test.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Document.Test
{
    [Parallel, Method(TextDocumentNames.CodeLensResolve, Direction.ClientToServer)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface ISubLensResolveHandler : IJsonRpcRequestHandler<SubLens, SubLens>, IDoesNotParticipateInRegistration, ICapability<SubLensCapability>, ICanBeIdentifiedHandler
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class SubLensResolveHandlerBase : AbstractHandlers.RequestCapability<SubLens, SubLens, SubLensCapability>, ISubLensResolveHandler
    {
        protected SubLensResolveHandlerBase(System.Guid id) : base()
        {
            _id = id;
        }

        protected SubLensResolveHandlerBase() : this(Guid.NewGuid())
        {
        }

        private readonly Guid _id;
        Guid ICanBeIdentifiedHandler.Id => _id;
    }
}
#nullable restore

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document.Test
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class SubLensResolveExtensions
    {
        public static ILanguageServerRegistry OnSubLensResolve(this ILanguageServerRegistry registry, Func<SubLens, Task<SubLens>> handler) => registry.AddHandler(TextDocumentNames.CodeLensResolve, RequestHandler.For(handler));
        public static ILanguageServerRegistry OnSubLensResolve(this ILanguageServerRegistry registry, Func<SubLens, CancellationToken, Task<SubLens>> handler) => registry.AddHandler(TextDocumentNames.CodeLensResolve, RequestHandler.For(handler));
        public static ILanguageServerRegistry OnSubLensResolve(this ILanguageServerRegistry registry, Func<SubLens, SubLensCapability, CancellationToken, Task<SubLens>> handler) => registry.AddHandler(TextDocumentNames.CodeLensResolve, new LanguageProtocolDelegatingHandlers.RequestCapability<SubLens, SubLens, SubLensCapability>(HandlerAdapter<SubLensCapability>.Adapt<SubLens, SubLens>(handler)));
        public static Task<SubLens> ResolveSubLens(this ITextDocumentLanguageClient mediator, SubLens request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
        public static Task<SubLens> ResolveSubLens(this ILanguageClient mediator, SubLens request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
    }
#nullable restore
}