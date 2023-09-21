//HintName: OutlayHintParams.cs
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
    [Parallel, Method(TextDocumentNames.InlayHint, Direction.ClientToServer)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface IOutlayHintsHandler : IJsonRpcRequestHandler<OutlayHintParams, OutlayHintContainer?>, IRegistration<OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities>
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class OutlayHintsHandlerBase : AbstractHandlers.Request<OutlayHintParams, OutlayHintContainer?, OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities>, IOutlayHintsHandler, IOutlayHintResolveHandler
    {
        protected OutlayHintsHandlerBase(System.Guid id) : base()
        {
            _id = id;
        }

        protected OutlayHintsHandlerBase() : this(Guid.NewGuid())
        {
        }

        private readonly Guid _id;
        Guid ICanBeIdentifiedHandler.Id => _id;
        public abstract Task<OutlayHint> Handle(OutlayHint request, CancellationToken cancellationToken);
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class OutlayHintsHandlerBase<T> : OutlayHintsHandlerBase where T : class?, IHandlerIdentity?
    {
        protected OutlayHintsHandlerBase(Guid id) : base(id)
        {
        }

        protected OutlayHintsHandlerBase() : this(Guid.NewGuid())
        {
        }

        public sealed override async Task<OutlayHintContainer?> Handle(OutlayHintParams request, CancellationToken cancellationToken) => await HandleParams(request, cancellationToken).ConfigureAwait(false);
        public sealed override async Task<OutlayHint> Handle(OutlayHint request, CancellationToken cancellationToken) => await HandleResolve(request, cancellationToken).ConfigureAwait(false);
        protected abstract Task<OutlayHintContainer<T>> HandleParams(OutlayHintParams request, CancellationToken cancellationToken);
        protected abstract Task<OutlayHint<T>> HandleResolve(OutlayHint<T> request, CancellationToken cancellationToken);
    }
}
#nullable restore

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class OutlayHintsExtensions
    {
        public static ILanguageServerRegistry OnOutlayHints(this ILanguageServerRegistry registry, Func<OutlayHintParams, Task<OutlayHintContainer?>> handler, RegistrationOptionsDelegate<OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.InlayHint, new LanguageProtocolDelegatingHandlers.Request<OutlayHintParams, OutlayHintContainer?, OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities>(HandlerAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHintParams, OutlayHintContainer?>(handler), RegistrationAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHintRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnOutlayHints(this ILanguageServerRegistry registry, Func<OutlayHintParams, CancellationToken, Task<OutlayHintContainer?>> handler, RegistrationOptionsDelegate<OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.InlayHint, new LanguageProtocolDelegatingHandlers.Request<OutlayHintParams, OutlayHintContainer?, OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities>(HandlerAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHintParams, OutlayHintContainer?>(handler), RegistrationAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHintRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnOutlayHints(this ILanguageServerRegistry registry, Func<OutlayHintParams, OutlayHintWorkspaceClientCapabilities, CancellationToken, Task<OutlayHintContainer?>> handler, RegistrationOptionsDelegate<OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.InlayHint, new LanguageProtocolDelegatingHandlers.Request<OutlayHintParams, OutlayHintContainer?, OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities>(HandlerAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHintParams, OutlayHintContainer?>(handler), RegistrationAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHintRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnOutlayHints(this ILanguageServerRegistry registry, Func<OutlayHintParams, Task<OutlayHintContainer?>> handler, Func<OutlayHint, Task<OutlayHint>> resolveHandler, RegistrationOptionsDelegate<OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities> registrationOptions)
        {
            var id = Guid.NewGuid();
            return registry.AddHandler(TextDocumentNames.InlayHint, new LanguageProtocolDelegatingHandlers.Request<OutlayHintParams, OutlayHintContainer?, OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities>(id, HandlerAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHintParams, OutlayHintContainer?>(handler), RegistrationAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHintRegistrationOptions>(registrationOptions))).AddHandler(TextDocumentNames.InlayHintResolve, new LanguageProtocolDelegatingHandlers.Request<OutlayHint, OutlayHint, OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities>(id, HandlerAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHint, OutlayHint>(resolveHandler), RegistrationAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHintRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnOutlayHints(this ILanguageServerRegistry registry, Func<OutlayHintParams, CancellationToken, Task<OutlayHintContainer?>> handler, Func<OutlayHint, CancellationToken, Task<OutlayHint>> resolveHandler, RegistrationOptionsDelegate<OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities> registrationOptions)
        {
            var id = Guid.NewGuid();
            return registry.AddHandler(TextDocumentNames.InlayHint, new LanguageProtocolDelegatingHandlers.Request<OutlayHintParams, OutlayHintContainer?, OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities>(id, HandlerAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHintParams, OutlayHintContainer?>(handler), RegistrationAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHintRegistrationOptions>(registrationOptions))).AddHandler(TextDocumentNames.InlayHintResolve, new LanguageProtocolDelegatingHandlers.Request<OutlayHint, OutlayHint, OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities>(id, HandlerAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHint, OutlayHint>(resolveHandler), RegistrationAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHintRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnOutlayHints(this ILanguageServerRegistry registry, Func<OutlayHintParams, OutlayHintWorkspaceClientCapabilities, CancellationToken, Task<OutlayHintContainer?>> handler, Func<OutlayHint, OutlayHintWorkspaceClientCapabilities, CancellationToken, Task<OutlayHint>> resolveHandler, RegistrationOptionsDelegate<OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities> registrationOptions)
        {
            var id = Guid.NewGuid();
            return registry.AddHandler(TextDocumentNames.InlayHint, new LanguageProtocolDelegatingHandlers.Request<OutlayHintParams, OutlayHintContainer?, OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities>(id, HandlerAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHintParams, OutlayHintContainer?>(handler), RegistrationAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHintRegistrationOptions>(registrationOptions))).AddHandler(TextDocumentNames.InlayHintResolve, new LanguageProtocolDelegatingHandlers.Request<OutlayHint, OutlayHint, OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities>(id, HandlerAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHint, OutlayHint>(resolveHandler), RegistrationAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHintRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnOutlayHints<T>(this ILanguageServerRegistry registry, Func<OutlayHintParams, Task<OutlayHintContainer<T>?>> handler, Func<OutlayHint<T>, Task<OutlayHint<T>>> resolveHandler, RegistrationOptionsDelegate<OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities> registrationOptions)
            where T : class?, IHandlerIdentity?
        {
            return registry.AddHandler(new DelegatingOutlayHintsHandler<T>(RegistrationAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHintRegistrationOptions>(registrationOptions), HandlerAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHintParams, OutlayHintContainer<T>?>(handler), HandlerAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHint<T>, OutlayHint<T>>(resolveHandler)));
        }

        public static ILanguageServerRegistry OnOutlayHints<T>(this ILanguageServerRegistry registry, Func<OutlayHintParams, CancellationToken, Task<OutlayHintContainer<T>?>> handler, Func<OutlayHint<T>, CancellationToken, Task<OutlayHint<T>>> resolveHandler, RegistrationOptionsDelegate<OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities> registrationOptions)
            where T : class?, IHandlerIdentity?
        {
            return registry.AddHandler(new DelegatingOutlayHintsHandler<T>(RegistrationAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHintRegistrationOptions>(registrationOptions), HandlerAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHintParams, OutlayHintContainer<T>?>(handler), HandlerAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHint<T>, OutlayHint<T>>(resolveHandler)));
        }

        public static ILanguageServerRegistry OnOutlayHints<T>(this ILanguageServerRegistry registry, Func<OutlayHintParams, OutlayHintWorkspaceClientCapabilities, CancellationToken, Task<OutlayHintContainer<T>?>> handler, Func<OutlayHint<T>, OutlayHintWorkspaceClientCapabilities, CancellationToken, Task<OutlayHint<T>>> resolveHandler, RegistrationOptionsDelegate<OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities> registrationOptions)
            where T : class?, IHandlerIdentity?
        {
            return registry.AddHandler(new DelegatingOutlayHintsHandler<T>(RegistrationAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHintRegistrationOptions>(registrationOptions), HandlerAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHintParams, OutlayHintContainer<T>?>(handler), HandlerAdapter<OutlayHintWorkspaceClientCapabilities>.Adapt<OutlayHint<T>, OutlayHint<T>>(resolveHandler)));
        }

        public static Task<OutlayHintContainer?> RequestOutlayHints(this ITextDocumentLanguageClient mediator, OutlayHintParams request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
        public static Task<OutlayHintContainer?> RequestOutlayHints(this ILanguageClient mediator, OutlayHintParams request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
        private sealed class DelegatingOutlayHintsHandler<T> : OutlayHintsHandlerBase<T> where T : class?, IHandlerIdentity?
        {
            private readonly RegistrationOptionsDelegate<OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities> _registrationOptionsFactory;
            protected internal override OutlayHintRegistrationOptions CreateRegistrationOptions(OutlayHintWorkspaceClientCapabilities capability, ClientCapabilities clientCapabilities) => _registrationOptionsFactory(capability, clientCapabilities);
            private readonly Func<OutlayHintParams, OutlayHintWorkspaceClientCapabilities, CancellationToken, Task<OutlayHintContainer<T>?>> _handleParams;
            private readonly Func<OutlayHint<T>, OutlayHintWorkspaceClientCapabilities, CancellationToken, Task<OutlayHint<T>>> _handleResolve;
            public DelegatingOutlayHintsHandler(RegistrationOptionsDelegate<OutlayHintRegistrationOptions, OutlayHintWorkspaceClientCapabilities> registrationOptionsFactory, Func<OutlayHintParams, OutlayHintWorkspaceClientCapabilities, CancellationToken, Task<OutlayHintContainer<T>?>> handleParams, Func<OutlayHint<T>, OutlayHintWorkspaceClientCapabilities, CancellationToken, Task<OutlayHint<T>>> handleResolve) : base()
            {
                _registrationOptionsFactory = registrationOptionsFactory;
                _handleParams = handleParams;
                _handleResolve = handleResolve;
            }

            protected override Task<OutlayHintContainer<T>?> HandleParams(OutlayHintParams request, CancellationToken cancellationToken) => _handleParams(request, Capability, cancellationToken);
            protected override Task<OutlayHint<T>> HandleResolve(OutlayHint<T> request, CancellationToken cancellationToken) => _handleResolve(request, Capability, cancellationToken);
        }
    }
#nullable restore
}