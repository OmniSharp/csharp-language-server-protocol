//HintName: SubLensParams.cs
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
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
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
    [Parallel, Method(TextDocumentNames.CodeLens, Direction.ClientToServer)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface ISubLensHandler : IJsonRpcRequestHandler<SubLensParams, SubLensContainer?>, IRegistration<SubLensRegistrationOptions, SubLensCapability>
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class SubLensHandlerBase : AbstractHandlers.Request<SubLensParams, SubLensContainer?, SubLensRegistrationOptions, SubLensCapability>, ISubLensHandler, ISubLensResolveHandler
    {
        protected SubLensHandlerBase(System.Guid id) : base()
        {
            _id = id;
        }

        protected SubLensHandlerBase() : this(Guid.NewGuid())
        {
        }

        private readonly Guid _id;
        Guid ICanBeIdentifiedHandler.Id => _id;
        public abstract Task<SubLens> Handle(SubLens request, CancellationToken cancellationToken);
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class SubLensHandlerBase<T> : SubLensHandlerBase where T : class?, IHandlerIdentity?
    {
        protected SubLensHandlerBase(Guid id) : base(id)
        {
        }

        protected SubLensHandlerBase() : this(Guid.NewGuid())
        {
        }

        public sealed override async Task<SubLensContainer?> Handle(SubLensParams request, CancellationToken cancellationToken) => await HandleParams(request, cancellationToken).ConfigureAwait(false);
        public sealed override async Task<SubLens> Handle(SubLens request, CancellationToken cancellationToken) => await HandleResolve(request, cancellationToken).ConfigureAwait(false);
        protected abstract Task<SubLensContainer<T>> HandleParams(SubLensParams request, CancellationToken cancellationToken);
        protected abstract Task<SubLens<T>> HandleResolve(SubLens<T> request, CancellationToken cancellationToken);
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class SubLensPartialHandlerBase : AbstractHandlers.PartialResults<SubLensParams, SubLensContainer?, SubLens, SubLensRegistrationOptions, SubLensCapability>, ISubLensHandler, ISubLensResolveHandler
    {
        protected SubLensPartialHandlerBase(System.Guid id, IProgressManager progressManager) : base(progressManager, SubLensContainer.From)
        {
            _id = id;
        }

        protected SubLensPartialHandlerBase(IProgressManager progressManager) : this(Guid.NewGuid(), progressManager)
        {
        }

        private readonly Guid _id;
        Guid ICanBeIdentifiedHandler.Id => _id;
        public abstract Task<SubLens> Handle(SubLens request, CancellationToken cancellationToken);
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class SubLensPartialHandlerBase<T> : SubLensPartialHandlerBase where T : class?, IHandlerIdentity?
    {
        protected SubLensPartialHandlerBase(Guid id, IProgressManager progressManager) : base(id, progressManager)
        {
        }

        protected SubLensPartialHandlerBase(IProgressManager progressManager) : this(Guid.NewGuid(), progressManager)
        {
        }

        protected sealed override void Handle(SubLensParams request, IObserver<IEnumerable<SubLens>> results, CancellationToken cancellationToken) => Handle(request, new LanguageProtocolDelegatingHandlers.TypedPartialObserver<SubLens<T>, SubLens>(results, SubLens.From), cancellationToken);
        public sealed override async Task<SubLens> Handle(SubLens request, CancellationToken cancellationToken)
        {
            var response = await HandleResolve(request, cancellationToken).ConfigureAwait(false);
            return response;
        }

        protected abstract void Handle(SubLensParams request, IObserver<IEnumerable<SubLens<T>>> results, CancellationToken cancellationToken);
        protected abstract Task<SubLens<T>> HandleResolve(SubLens<T> request, CancellationToken cancellationToken);
    }
}
#nullable restore

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document.Test
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class SubLensExtensions
    {
        public static ILanguageServerRegistry OnSubLens(this ILanguageServerRegistry registry, Func<SubLensParams, Task<SubLensContainer?>> handler, RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.CodeLens, new LanguageProtocolDelegatingHandlers.Request<SubLensParams, SubLensContainer?, SubLensRegistrationOptions, SubLensCapability>(HandlerAdapter<SubLensCapability>.Adapt<SubLensParams, SubLensContainer?>(handler), RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnSubLens(this ILanguageServerRegistry registry, Func<SubLensParams, CancellationToken, Task<SubLensContainer?>> handler, RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.CodeLens, new LanguageProtocolDelegatingHandlers.Request<SubLensParams, SubLensContainer?, SubLensRegistrationOptions, SubLensCapability>(HandlerAdapter<SubLensCapability>.Adapt<SubLensParams, SubLensContainer?>(handler), RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnSubLens(this ILanguageServerRegistry registry, Func<SubLensParams, SubLensCapability, CancellationToken, Task<SubLensContainer?>> handler, RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.CodeLens, new LanguageProtocolDelegatingHandlers.Request<SubLensParams, SubLensContainer?, SubLensRegistrationOptions, SubLensCapability>(HandlerAdapter<SubLensCapability>.Adapt<SubLensParams, SubLensContainer?>(handler), RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry ObserveSubLens(this ILanguageServerRegistry registry, Action<SubLensParams, IObserver<IEnumerable<SubLens>>> handler, RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.CodeLens, _ => new LanguageProtocolDelegatingHandlers.PartialResults<SubLensParams, SubLensContainer?, SubLens, SubLensRegistrationOptions, SubLensCapability>(PartialAdapter<SubLensCapability>.Adapt<SubLensParams, SubLens>(handler), RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), SubLensContainer.From));
        }

        public static ILanguageServerRegistry ObserveSubLens(this ILanguageServerRegistry registry, Action<SubLensParams, IObserver<IEnumerable<SubLens>>, CancellationToken> handler, RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.CodeLens, _ => new LanguageProtocolDelegatingHandlers.PartialResults<SubLensParams, SubLensContainer?, SubLens, SubLensRegistrationOptions, SubLensCapability>(PartialAdapter<SubLensCapability>.Adapt<SubLensParams, SubLens>(handler), RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), SubLensContainer.From));
        }

        public static ILanguageServerRegistry ObserveSubLens(this ILanguageServerRegistry registry, Action<SubLensParams, IObserver<IEnumerable<SubLens>>, SubLensCapability, CancellationToken> handler, RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.CodeLens, _ => new LanguageProtocolDelegatingHandlers.PartialResults<SubLensParams, SubLensContainer?, SubLens, SubLensRegistrationOptions, SubLensCapability>(PartialAdapter<SubLensCapability>.Adapt<SubLensParams, SubLens>(handler), RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), SubLensContainer.From));
        }

        public static ILanguageServerRegistry OnSubLens(this ILanguageServerRegistry registry, Func<SubLensParams, Task<SubLensContainer?>> handler, Func<SubLens, Task<SubLens>> resolveHandler, RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> registrationOptions)
        {
            var id = Guid.NewGuid();
            return registry.AddHandler(TextDocumentNames.CodeLens, new LanguageProtocolDelegatingHandlers.Request<SubLensParams, SubLensContainer?, SubLensRegistrationOptions, SubLensCapability>(id, HandlerAdapter<SubLensCapability>.Adapt<SubLensParams, SubLensContainer?>(handler), RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions))).AddHandler(TextDocumentNames.CodeLensResolve, new LanguageProtocolDelegatingHandlers.Request<SubLens, SubLens, SubLensRegistrationOptions, SubLensCapability>(id, HandlerAdapter<SubLensCapability>.Adapt<SubLens, SubLens>(resolveHandler), RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnSubLens(this ILanguageServerRegistry registry, Func<SubLensParams, CancellationToken, Task<SubLensContainer?>> handler, Func<SubLens, CancellationToken, Task<SubLens>> resolveHandler, RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> registrationOptions)
        {
            var id = Guid.NewGuid();
            return registry.AddHandler(TextDocumentNames.CodeLens, new LanguageProtocolDelegatingHandlers.Request<SubLensParams, SubLensContainer?, SubLensRegistrationOptions, SubLensCapability>(id, HandlerAdapter<SubLensCapability>.Adapt<SubLensParams, SubLensContainer?>(handler), RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions))).AddHandler(TextDocumentNames.CodeLensResolve, new LanguageProtocolDelegatingHandlers.Request<SubLens, SubLens, SubLensRegistrationOptions, SubLensCapability>(id, HandlerAdapter<SubLensCapability>.Adapt<SubLens, SubLens>(resolveHandler), RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnSubLens(this ILanguageServerRegistry registry, Func<SubLensParams, SubLensCapability, CancellationToken, Task<SubLensContainer?>> handler, Func<SubLens, SubLensCapability, CancellationToken, Task<SubLens>> resolveHandler, RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> registrationOptions)
        {
            var id = Guid.NewGuid();
            return registry.AddHandler(TextDocumentNames.CodeLens, new LanguageProtocolDelegatingHandlers.Request<SubLensParams, SubLensContainer?, SubLensRegistrationOptions, SubLensCapability>(id, HandlerAdapter<SubLensCapability>.Adapt<SubLensParams, SubLensContainer?>(handler), RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions))).AddHandler(TextDocumentNames.CodeLensResolve, new LanguageProtocolDelegatingHandlers.Request<SubLens, SubLens, SubLensRegistrationOptions, SubLensCapability>(id, HandlerAdapter<SubLensCapability>.Adapt<SubLens, SubLens>(resolveHandler), RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry ObserveSubLens(this ILanguageServerRegistry registry, Action<SubLensParams, IObserver<IEnumerable<SubLens>>> handler, Func<SubLens, Task<SubLens>> resolveHandler, RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> registrationOptions)
        {
            var id = Guid.NewGuid();
            return registry.AddHandler(TextDocumentNames.CodeLens, _ => new LanguageProtocolDelegatingHandlers.PartialResults<SubLensParams, SubLensContainer?, SubLens, SubLensRegistrationOptions, SubLensCapability>(PartialAdapter<SubLensCapability>.Adapt<SubLensParams, SubLens>(handler), RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), SubLensContainer.From)).AddHandler(TextDocumentNames.CodeLensResolve, new LanguageProtocolDelegatingHandlers.Request<SubLens, SubLens, SubLensRegistrationOptions, SubLensCapability>(id, HandlerAdapter<SubLensCapability>.Adapt<SubLens, SubLens>(resolveHandler), RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry ObserveSubLens(this ILanguageServerRegistry registry, Action<SubLensParams, IObserver<IEnumerable<SubLens>>, CancellationToken> handler, Func<SubLens, CancellationToken, Task<SubLens>> resolveHandler, RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> registrationOptions)
        {
            var id = Guid.NewGuid();
            return registry.AddHandler(TextDocumentNames.CodeLens, _ => new LanguageProtocolDelegatingHandlers.PartialResults<SubLensParams, SubLensContainer?, SubLens, SubLensRegistrationOptions, SubLensCapability>(PartialAdapter<SubLensCapability>.Adapt<SubLensParams, SubLens>(handler), RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), SubLensContainer.From)).AddHandler(TextDocumentNames.CodeLensResolve, new LanguageProtocolDelegatingHandlers.Request<SubLens, SubLens, SubLensRegistrationOptions, SubLensCapability>(id, HandlerAdapter<SubLensCapability>.Adapt<SubLens, SubLens>(resolveHandler), RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry ObserveSubLens(this ILanguageServerRegistry registry, Action<SubLensParams, IObserver<IEnumerable<SubLens>>, SubLensCapability, CancellationToken> handler, Func<SubLens, SubLensCapability, CancellationToken, Task<SubLens>> resolveHandler, RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> registrationOptions)
        {
            var id = Guid.NewGuid();
            return registry.AddHandler(TextDocumentNames.CodeLens, _ => new LanguageProtocolDelegatingHandlers.PartialResults<SubLensParams, SubLensContainer?, SubLens, SubLensRegistrationOptions, SubLensCapability>(PartialAdapter<SubLensCapability>.Adapt<SubLensParams, SubLens>(handler), RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), SubLensContainer.From)).AddHandler(TextDocumentNames.CodeLensResolve, new LanguageProtocolDelegatingHandlers.Request<SubLens, SubLens, SubLensRegistrationOptions, SubLensCapability>(id, HandlerAdapter<SubLensCapability>.Adapt<SubLens, SubLens>(resolveHandler), RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnSubLens<T>(this ILanguageServerRegistry registry, Func<SubLensParams, Task<SubLensContainer<T>?>> handler, Func<SubLens<T>, Task<SubLens<T>>> resolveHandler, RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> registrationOptions)
            where T : class?, IHandlerIdentity?
        {
            return registry.AddHandler(new DelegatingSubLensHandler<T>(RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions), HandlerAdapter<SubLensCapability>.Adapt<SubLensParams, SubLensContainer<T>?>(handler), HandlerAdapter<SubLensCapability>.Adapt<SubLens<T>, SubLens<T>>(resolveHandler)));
        }

        public static ILanguageServerRegistry OnSubLens<T>(this ILanguageServerRegistry registry, Func<SubLensParams, CancellationToken, Task<SubLensContainer<T>?>> handler, Func<SubLens<T>, CancellationToken, Task<SubLens<T>>> resolveHandler, RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> registrationOptions)
            where T : class?, IHandlerIdentity?
        {
            return registry.AddHandler(new DelegatingSubLensHandler<T>(RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions), HandlerAdapter<SubLensCapability>.Adapt<SubLensParams, SubLensContainer<T>?>(handler), HandlerAdapter<SubLensCapability>.Adapt<SubLens<T>, SubLens<T>>(resolveHandler)));
        }

        public static ILanguageServerRegistry OnSubLens<T>(this ILanguageServerRegistry registry, Func<SubLensParams, SubLensCapability, CancellationToken, Task<SubLensContainer<T>?>> handler, Func<SubLens<T>, SubLensCapability, CancellationToken, Task<SubLens<T>>> resolveHandler, RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> registrationOptions)
            where T : class?, IHandlerIdentity?
        {
            return registry.AddHandler(new DelegatingSubLensHandler<T>(RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions), HandlerAdapter<SubLensCapability>.Adapt<SubLensParams, SubLensContainer<T>?>(handler), HandlerAdapter<SubLensCapability>.Adapt<SubLens<T>, SubLens<T>>(resolveHandler)));
        }

        public static ILanguageServerRegistry ObserveSubLens<T>(this ILanguageServerRegistry registry, Action<SubLensParams, IObserver<IEnumerable<SubLens<T>>>> handler, Func<SubLens<T>, Task<SubLens<T>>> resolveHandler, RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> registrationOptions)
            where T : class?, IHandlerIdentity?
        {
            return registry.AddHandler(_ => new DelegatingSubLensPartialHandler<T>(_.GetService<IProgressManager>(), RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions), PartialAdapter<SubLensCapability>.Adapt<SubLensParams, SubLens<T>>(handler), HandlerAdapter<SubLensCapability>.Adapt<SubLens<T>, SubLens<T>>(resolveHandler)));
        }

        public static ILanguageServerRegistry ObserveSubLens<T>(this ILanguageServerRegistry registry, Action<SubLensParams, IObserver<IEnumerable<SubLens<T>>>, CancellationToken> handler, Func<SubLens<T>, CancellationToken, Task<SubLens<T>>> resolveHandler, RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> registrationOptions)
            where T : class?, IHandlerIdentity?
        {
            return registry.AddHandler(_ => new DelegatingSubLensPartialHandler<T>(_.GetService<IProgressManager>(), RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions), PartialAdapter<SubLensCapability>.Adapt<SubLensParams, SubLens<T>>(handler), HandlerAdapter<SubLensCapability>.Adapt<SubLens<T>, SubLens<T>>(resolveHandler)));
        }

        public static ILanguageServerRegistry ObserveSubLens<T>(this ILanguageServerRegistry registry, Action<SubLensParams, IObserver<IEnumerable<SubLens<T>>>, SubLensCapability, CancellationToken> handler, Func<SubLens<T>, SubLensCapability, CancellationToken, Task<SubLens<T>>> resolveHandler, RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> registrationOptions)
            where T : class?, IHandlerIdentity?
        {
            return registry.AddHandler(_ => new DelegatingSubLensPartialHandler<T>(_.GetService<IProgressManager>(), RegistrationAdapter<SubLensCapability>.Adapt<SubLensRegistrationOptions>(registrationOptions), PartialAdapter<SubLensCapability>.Adapt<SubLensParams, SubLens<T>>(handler), HandlerAdapter<SubLensCapability>.Adapt<SubLens<T>, SubLens<T>>(resolveHandler)));
        }

        public static IRequestProgressObservable<IEnumerable<SubLens>, SubLensContainer?> RequestSubLens(this ITextDocumentLanguageClient mediator, SubLensParams request, CancellationToken cancellationToken = default) => mediator.ProgressManager.MonitorUntil(request, SubLensContainer.From, cancellationToken);
        public static IRequestProgressObservable<IEnumerable<SubLens>, SubLensContainer?> RequestSubLens(this ILanguageClient mediator, SubLensParams request, CancellationToken cancellationToken = default) => mediator.ProgressManager.MonitorUntil(request, SubLensContainer.From, cancellationToken);
        private sealed class DelegatingSubLensHandler<T> : SubLensHandlerBase<T> where T : class?, IHandlerIdentity?
        {
            private readonly RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> _registrationOptionsFactory;
            protected internal override SubLensRegistrationOptions CreateRegistrationOptions(SubLensCapability capability, ClientCapabilities clientCapabilities) => _registrationOptionsFactory(capability, clientCapabilities);
            private readonly Func<SubLensParams, SubLensCapability, CancellationToken, Task<SubLensContainer<T>?>> _handleParams;
            private readonly Func<SubLens<T>, SubLensCapability, CancellationToken, Task<SubLens<T>>> _handleResolve;
            public DelegatingSubLensHandler(RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> registrationOptionsFactory, Func<SubLensParams, SubLensCapability, CancellationToken, Task<SubLensContainer<T>?>> handleParams, Func<SubLens<T>, SubLensCapability, CancellationToken, Task<SubLens<T>>> handleResolve) : base()
            {
                _registrationOptionsFactory = registrationOptionsFactory;
                _handleParams = handleParams;
                _handleResolve = handleResolve;
            }

            protected override Task<SubLensContainer<T>?> HandleParams(SubLensParams request, CancellationToken cancellationToken) => _handleParams(request, Capability, cancellationToken);
            protected override Task<SubLens<T>> HandleResolve(SubLens<T> request, CancellationToken cancellationToken) => _handleResolve(request, Capability, cancellationToken);
        }

        private sealed class DelegatingSubLensPartialHandler<T> : SubLensPartialHandlerBase<T> where T : class?, IHandlerIdentity?
        {
            private readonly RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> _registrationOptionsFactory;
            protected internal override SubLensRegistrationOptions CreateRegistrationOptions(SubLensCapability capability, ClientCapabilities clientCapabilities) => _registrationOptionsFactory(capability, clientCapabilities);
            private readonly Action<SubLensParams, IObserver<IEnumerable<SubLens<T>>>, SubLensCapability, CancellationToken> _handle;
            private readonly Func<SubLens<T>, SubLensCapability, CancellationToken, Task<SubLens<T>>> _handleResolve;
            public DelegatingSubLensPartialHandler(IProgressManager progressManager, RegistrationOptionsDelegate<SubLensRegistrationOptions, SubLensCapability> registrationOptionsFactory, Action<SubLensParams, IObserver<IEnumerable<SubLens<T>>>, SubLensCapability, CancellationToken> handle, Func<SubLens<T>, SubLensCapability, CancellationToken, Task<SubLens<T>>> handleResolve) : base(progressManager)
            {
                _registrationOptionsFactory = registrationOptionsFactory;
                _handle = handle;
                _handleResolve = handleResolve;
            }

            protected override void Handle(SubLensParams request, IObserver<IEnumerable<SubLens<T>>> results, CancellationToken cancellationToken) => _handle(request, results, Capability, cancellationToken);
            protected override Task<SubLens<T>> HandleResolve(SubLens<T> request, CancellationToken cancellationToken) => _handleResolve(request, Capability, cancellationToken);
        }
    }
#nullable restore
}