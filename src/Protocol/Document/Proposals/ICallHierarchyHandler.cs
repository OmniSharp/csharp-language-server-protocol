using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals
{
    [Obsolete(Constants.Proposal)]
    [Parallel]
    [Method(TextDocumentNames.PrepareCallHierarchy, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface ICallHierarchyPrepareHandler :
        IJsonRpcRequestHandler<CallHierarchyPrepareParams, Container<CallHierarchyItem>?>,
        IRegistration<CallHierarchyRegistrationOptions, CallHierarchyCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    [Parallel]
    [Method(TextDocumentNames.CallHierarchyIncoming, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface ICallHierarchyIncomingHandler : IJsonRpcRequestHandler<CallHierarchyIncomingCallsParams, Container<CallHierarchyIncomingCall>?>,
                                                     IRegistration<CallHierarchyRegistrationOptions, CallHierarchyCapability>, ICanBeIdentifiedHandler
    {
    }

    [Obsolete(Constants.Proposal)]
    [Parallel]
    [Method(TextDocumentNames.CallHierarchyOutgoing, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface ICallHierarchyOutgoingHandler : IJsonRpcRequestHandler<CallHierarchyOutgoingCallsParams, Container<CallHierarchyOutgoingCall>?>,
                                                     IRegistration<CallHierarchyRegistrationOptions, CallHierarchyCapability>, ICanBeIdentifiedHandler
    {
    }

    [Obsolete(Constants.Proposal)]
    public abstract class CallHierarchyHandlerBase : AbstractHandlers.Base<CallHierarchyRegistrationOptions, CallHierarchyCapability>, ICallHierarchyPrepareHandler, ICallHierarchyIncomingHandler,
                                                 ICallHierarchyOutgoingHandler
    {
        public abstract Task<Container<CallHierarchyItem>?> Handle(CallHierarchyPrepareParams request, CancellationToken cancellationToken);
        public abstract Task<Container<CallHierarchyIncomingCall>?> Handle(CallHierarchyIncomingCallsParams request, CancellationToken cancellationToken);
        public abstract Task<Container<CallHierarchyOutgoingCall>?> Handle(CallHierarchyOutgoingCallsParams request, CancellationToken cancellationToken);
        Guid ICanBeIdentifiedHandler.Id { get; } = Guid.NewGuid();
    }

    [Obsolete(Constants.Proposal)]
    public abstract class PartialCallHierarchyHandlerBase :
        AbstractHandlers.PartialResults<CallHierarchyPrepareParams, Container<CallHierarchyItem>?, CallHierarchyItem, CallHierarchyRegistrationOptions, CallHierarchyCapability>,
        ICallHierarchyPrepareHandler, ICallHierarchyIncomingHandler, ICallHierarchyOutgoingHandler
    {
        private readonly ICallHierarchyIncomingHandler _incoming;
        private readonly ICallHierarchyOutgoingHandler _outgoing;
        Guid ICanBeIdentifiedHandler.Id { get; } = Guid.NewGuid();

        protected PartialCallHierarchyHandlerBase(IProgressManager progressManager) : base(
            progressManager,
            lenses => new Container<CallHierarchyItem>(lenses)
        )
        {
            _incoming = new PartialIncoming(progressManager, this);
            _outgoing = new PartialOutgoing(progressManager, this);
        }

        public Task<Container<CallHierarchyIncomingCall>?> Handle(CallHierarchyIncomingCallsParams request, CancellationToken cancellationToken) =>
            _incoming.Handle(request, cancellationToken);

        public Task<Container<CallHierarchyOutgoingCall>?> Handle(CallHierarchyOutgoingCallsParams request, CancellationToken cancellationToken) =>
            _outgoing.Handle(request, cancellationToken);

        protected abstract void Handle(CallHierarchyIncomingCallsParams request, IObserver<IEnumerable<CallHierarchyIncomingCall>> results, CancellationToken cancellationToken);
        protected abstract void Handle(CallHierarchyOutgoingCallsParams request, IObserver<IEnumerable<CallHierarchyOutgoingCall>> results, CancellationToken cancellationToken);

        class PartialIncoming :
            AbstractHandlers.PartialResults<CallHierarchyIncomingCallsParams, Container<CallHierarchyIncomingCall>?, CallHierarchyIncomingCall,
                CallHierarchyRegistrationOptions, CallHierarchyCapability
            >, ICallHierarchyIncomingHandler
        {
            private readonly PartialCallHierarchyHandlerBase _self;
            Guid ICanBeIdentifiedHandler.Id { get; } = Guid.Empty;

            public PartialIncoming(
                IProgressManager progressManager, PartialCallHierarchyHandlerBase self
            ) : base(progressManager, x => new Container<CallHierarchyIncomingCall>(x))
            {
                _self = self;
            }

            protected override void Handle(
                CallHierarchyIncomingCallsParams request, IObserver<IEnumerable<CallHierarchyIncomingCall>> results, CancellationToken cancellationToken
            ) => _self.Handle(request, results, cancellationToken);

            protected override CallHierarchyRegistrationOptions CreateRegistrationOptions(CallHierarchyCapability capability) => ((IRegistration<CallHierarchyRegistrationOptions, CallHierarchyCapability>) _self).GetRegistrationOptions(capability);
        }

        class PartialOutgoing :
            AbstractHandlers.PartialResults<CallHierarchyOutgoingCallsParams, Container<CallHierarchyOutgoingCall>?, CallHierarchyOutgoingCall,                                                                                                                                                              CallHierarchyRegistrationOptions, CallHierarchyCapability
            >, ICallHierarchyOutgoingHandler
        {
            private readonly PartialCallHierarchyHandlerBase _self;
            Guid ICanBeIdentifiedHandler.Id { get; } = Guid.Empty;

            public PartialOutgoing(IProgressManager progressManager, PartialCallHierarchyHandlerBase self) : base(
                progressManager, x => new Container<CallHierarchyOutgoingCall>(x)
            )
            {
                _self = self;
            }

            protected override void Handle(
                CallHierarchyOutgoingCallsParams request, IObserver<IEnumerable<CallHierarchyOutgoingCall>> results, CancellationToken cancellationToken
            ) => _self.Handle(request, results, cancellationToken);

            protected override CallHierarchyRegistrationOptions CreateRegistrationOptions(CallHierarchyCapability capability) => ((IRegistration<CallHierarchyRegistrationOptions, CallHierarchyCapability>) _self).GetRegistrationOptions(capability);
        }
    }


    public abstract class CallHierarchyHandlerBase<T> : CallHierarchyHandlerBase where T : HandlerIdentity?, new()
    {
        public sealed override async Task<Container<CallHierarchyItem>?> Handle(CallHierarchyPrepareParams request, CancellationToken cancellationToken)
        {
            var response = await HandlePrepare(request, cancellationToken);
            return new Container<CallHierarchyItem>(response.Select(z => (CallHierarchyItem) z));
        }

        public sealed override Task<Container<CallHierarchyIncomingCall>?> Handle(CallHierarchyIncomingCallsParams request, CancellationToken cancellationToken)
        {
            return HandleIncomingCalls(
                new CallHierarchyIncomingCallsParams<T>() {
                    Item = request.Item,
                    PartialResultToken = request.PartialResultToken,
                    WorkDoneToken = request.WorkDoneToken
                },
                cancellationToken
            );
        }

        public sealed override Task<Container<CallHierarchyOutgoingCall>?> Handle(CallHierarchyOutgoingCallsParams request, CancellationToken cancellationToken)
        {
            return HandleOutgoingCalls(
                new CallHierarchyOutgoingCallsParams<T>() {
                    Item = request.Item,
                    PartialResultToken = request.PartialResultToken,
                    WorkDoneToken = request.WorkDoneToken
                },
                cancellationToken
            );
        }

        protected abstract Task<Container<CallHierarchyItem<T>>?> HandlePrepare(CallHierarchyPrepareParams request, CancellationToken cancellationToken);
        protected abstract Task<Container<CallHierarchyIncomingCall>?> HandleIncomingCalls(CallHierarchyIncomingCallsParams<T> request, CancellationToken cancellationToken);
        protected abstract Task<Container<CallHierarchyOutgoingCall>?> HandleOutgoingCalls(CallHierarchyOutgoingCallsParams<T> request, CancellationToken cancellationToken);
    }

    public abstract class PartialCallHierarchyHandlerBase<T> : PartialCallHierarchyHandlerBase where T : HandlerIdentity?, new()
    {
        protected PartialCallHierarchyHandlerBase(IProgressManager progressManager) : base(progressManager)
        {
        }

        protected sealed override void Handle(CallHierarchyPrepareParams request, IObserver<IEnumerable<CallHierarchyItem>> results, CancellationToken cancellationToken) => Handle(
            request,
            Observer.Create<IEnumerable<CallHierarchyItem<T>>>(
                x => results.OnNext(x.Select(z => (CallHierarchyItem) z)),
                results.OnError,
                results.OnCompleted
            ), cancellationToken
        );

        protected abstract void Handle(CallHierarchyPrepareParams request, IObserver<IEnumerable<CallHierarchyItem<T>>> results, CancellationToken cancellationToken);

        protected sealed override void Handle(
            CallHierarchyIncomingCallsParams request, IObserver<IEnumerable<CallHierarchyIncomingCall>> results, CancellationToken cancellationToken
        ) => Handle(
            new CallHierarchyIncomingCallsParams<T>() {
                Item = request.Item,
                PartialResultToken = request.PartialResultToken,
                WorkDoneToken = request.WorkDoneToken
            },
            results,
            cancellationToken
        );

        protected abstract void Handle(CallHierarchyIncomingCallsParams<T> request, IObserver<IEnumerable<CallHierarchyIncomingCall>> results, CancellationToken cancellationToken);

        protected sealed override void Handle(
            CallHierarchyOutgoingCallsParams request, IObserver<IEnumerable<CallHierarchyOutgoingCall>> results, CancellationToken cancellationToken
        ) => Handle(
            new CallHierarchyOutgoingCallsParams<T>() {
                Item = request.Item,
                PartialResultToken = request.PartialResultToken,
                WorkDoneToken = request.WorkDoneToken
            },
            results,
            cancellationToken
        );

        protected abstract void Handle(CallHierarchyOutgoingCallsParams<T> request, IObserver<IEnumerable<CallHierarchyOutgoingCall>> results, CancellationToken cancellationToken);
    }

    [Obsolete(Constants.Proposal)]
    public static partial class CallHierarchyExtensions
    {
        public static ILanguageServerRegistry OnCallHierarchy(
            this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams, CallHierarchyCapability, CancellationToken, Task<Container<CallHierarchyItem>?>> handler,
            Func<CallHierarchyIncomingCallsParams, CallHierarchyCapability, CancellationToken, Task<Container<CallHierarchyIncomingCall>?>> incomingHandler,
            Func<CallHierarchyOutgoingCallsParams, CallHierarchyCapability, CancellationToken, Task<Container<CallHierarchyOutgoingCall>?>> outgoingHandler,
            Func<CallHierarchyCapability, CallHierarchyRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= _ => new CallHierarchyRegistrationOptions();
            var id = Guid.NewGuid();
            return registry.AddHandler(
                                TextDocumentNames.PrepareCallHierarchy,
                                new LanguageProtocolDelegatingHandlers.Request<CallHierarchyPrepareParams,
                                    Container<CallHierarchyItem>?,
                                    CallHierarchyRegistrationOptions,
                                    CallHierarchyCapability>(id, handler, registrationOptionsFactory)
                            )
                           .AddHandler(
                                TextDocumentNames.CallHierarchyIncoming,
                                new LanguageProtocolDelegatingHandlers.Request<CallHierarchyIncomingCallsParams,
                                    Container<CallHierarchyIncomingCall>?,
                                    CallHierarchyRegistrationOptions,
                                    CallHierarchyCapability>(id, incomingHandler, registrationOptionsFactory)
                            )
                           .AddHandler(
                                TextDocumentNames.CallHierarchyOutgoing,
                                new LanguageProtocolDelegatingHandlers.Request<CallHierarchyOutgoingCallsParams,
                                    Container<CallHierarchyOutgoingCall>?,
                                    CallHierarchyRegistrationOptions,
                                    CallHierarchyCapability>(id, outgoingHandler, registrationOptionsFactory)
                            )
                ;
        }

        public static ILanguageServerRegistry OnCallHierarchy<T>(
            this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams, CallHierarchyCapability, CancellationToken, Task<Container<CallHierarchyItem<T>>?>> handler,
            Func<CallHierarchyIncomingCallsParams<T>, CallHierarchyCapability, CancellationToken, Task<Container<CallHierarchyIncomingCall>?>> incomingHandler,
            Func<CallHierarchyOutgoingCallsParams<T>, CallHierarchyCapability, CancellationToken, Task<Container<CallHierarchyOutgoingCall>?>> outgoingHandler,
            Func<CallHierarchyCapability, CallHierarchyRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CallHierarchyRegistrationOptions();
            return registry.AddHandler(
                new DelegatingCallHierarchyHandler<T>(
                    handler,
                    incomingHandler,
                    outgoingHandler,
                    registrationOptionsFactory
                )
            );
            ;
        }

        public static ILanguageServerRegistry OnCallHierarchy(
            this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams, CallHierarchyCapability, Task<Container<CallHierarchyItem>?>> handler,
            Func<CallHierarchyIncomingCallsParams, CallHierarchyCapability, Task<Container<CallHierarchyIncomingCall>?>> incomingHandler,
            Func<CallHierarchyOutgoingCallsParams, CallHierarchyCapability, Task<Container<CallHierarchyOutgoingCall>?>> outgoingHandler,
            Func<CallHierarchyCapability, CallHierarchyRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= _ => new CallHierarchyRegistrationOptions();
            var id = Guid.NewGuid();
            return registry.AddHandler(
                                TextDocumentNames.PrepareCallHierarchy,
                                new LanguageProtocolDelegatingHandlers.Request<CallHierarchyPrepareParams,
                                    Container<CallHierarchyItem>?,
                                    CallHierarchyRegistrationOptions,
                                    CallHierarchyCapability>(id, handler, registrationOptionsFactory)
                            )
                           .AddHandler(
                                TextDocumentNames.CallHierarchyIncoming,
                                new LanguageProtocolDelegatingHandlers.Request<CallHierarchyIncomingCallsParams,
                                    Container<CallHierarchyIncomingCall>?,
                                    CallHierarchyRegistrationOptions,
                                    CallHierarchyCapability>(id, incomingHandler, registrationOptionsFactory)
                            )
                           .AddHandler(
                                TextDocumentNames.CallHierarchyOutgoing,
                                new LanguageProtocolDelegatingHandlers.Request<CallHierarchyOutgoingCallsParams,
                                    Container<CallHierarchyOutgoingCall>?,
                                    CallHierarchyRegistrationOptions,
                                    CallHierarchyCapability>(id, outgoingHandler, registrationOptionsFactory)
                            )
                ;
        }

        public static ILanguageServerRegistry OnCallHierarchy<T>(
            this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams, CallHierarchyCapability, Task<Container<CallHierarchyItem<T>>?>> handler,
            Func<CallHierarchyIncomingCallsParams<T>, CallHierarchyCapability, Task<Container<CallHierarchyIncomingCall>?>> incomingHandler,
            Func<CallHierarchyOutgoingCallsParams<T>, CallHierarchyCapability, Task<Container<CallHierarchyOutgoingCall>?>> outgoingHandler,
            Func<CallHierarchyCapability, CallHierarchyRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CallHierarchyRegistrationOptions();
            return registry.AddHandler(
                new DelegatingCallHierarchyHandler<T>(
                    (request, capability, arg3) => handler(request, capability),
                    (request, capability, arg3) => incomingHandler(request, capability),
                    (request, capability, arg3) => outgoingHandler(request, capability),
                    registrationOptionsFactory
                )
            );
            ;
        }

        public static ILanguageServerRegistry OnCallHierarchy(
            this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams, CancellationToken, Task<Container<CallHierarchyItem>?>> handler,
            Func<CallHierarchyIncomingCallsParams, CancellationToken, Task<Container<CallHierarchyIncomingCall>?>> incomingHandler,
            Func<CallHierarchyOutgoingCallsParams, CancellationToken, Task<Container<CallHierarchyOutgoingCall>?>> outgoingHandler,
            Func<CallHierarchyRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= () => new CallHierarchyRegistrationOptions();
            var id = Guid.NewGuid();
            return
                registry.AddHandler(
                             TextDocumentNames.PrepareCallHierarchy,
                             new LanguageProtocolDelegatingHandlers.RequestRegistration<CallHierarchyPrepareParams,
                                 Container<CallHierarchyItem>?,
                                 CallHierarchyRegistrationOptions>(id, handler, registrationOptionsFactory)
                         )
                        .AddHandler(
                             TextDocumentNames.CallHierarchyIncoming,
                             new LanguageProtocolDelegatingHandlers.RequestRegistration<CallHierarchyIncomingCallsParams,
                                 Container<CallHierarchyIncomingCall>?,
                                 CallHierarchyRegistrationOptions>(id, incomingHandler, registrationOptionsFactory)
                         )
                        .AddHandler(
                             TextDocumentNames.CallHierarchyOutgoing,
                             new LanguageProtocolDelegatingHandlers.RequestRegistration<CallHierarchyOutgoingCallsParams,
                                 Container<CallHierarchyOutgoingCall>?,
                                 CallHierarchyRegistrationOptions>(id, outgoingHandler, registrationOptionsFactory)
                         )
                ;
        }

        public static ILanguageServerRegistry OnCallHierarchy<T>(
            this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams, CancellationToken, Task<Container<CallHierarchyItem<T>>?>> handler,
            Func<CallHierarchyIncomingCallsParams<T>, CancellationToken, Task<Container<CallHierarchyIncomingCall>?>> incomingHandler,
            Func<CallHierarchyOutgoingCallsParams<T>, CancellationToken, Task<Container<CallHierarchyOutgoingCall>?>> outgoingHandler,
            Func<CallHierarchyCapability, CallHierarchyRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CallHierarchyRegistrationOptions();
            return registry.AddHandler(
                new DelegatingCallHierarchyHandler<T>(
                    (request, capability, arg3) => handler(request, arg3),
                    (request, capability, arg3) => incomingHandler(request, arg3),
                    (request, capability, arg3) => outgoingHandler(request, arg3),
                    registrationOptionsFactory
                )
            );
            ;
        }

        public static ILanguageServerRegistry OnCallHierarchy(
            this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams, Task<Container<CallHierarchyItem>?>> handler,
            Func<CallHierarchyIncomingCallsParams, Task<Container<CallHierarchyIncomingCall>?>> incomingHandler,
            Func<CallHierarchyOutgoingCallsParams, Task<Container<CallHierarchyOutgoingCall>?>> outgoingHandler,
            Func<CallHierarchyRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= () => new CallHierarchyRegistrationOptions();
            var id = Guid.NewGuid();
            return
                registry.AddHandler(
                             TextDocumentNames.PrepareCallHierarchy,
                             new LanguageProtocolDelegatingHandlers.RequestRegistration<CallHierarchyPrepareParams,
                                 Container<CallHierarchyItem>?,
                                 CallHierarchyRegistrationOptions>(id, handler, registrationOptionsFactory)
                         )
                        .AddHandler(
                             TextDocumentNames.CallHierarchyIncoming,
                             new LanguageProtocolDelegatingHandlers.RequestRegistration<CallHierarchyIncomingCallsParams,
                                 Container<CallHierarchyIncomingCall>?,
                                 CallHierarchyRegistrationOptions>(id, incomingHandler, registrationOptionsFactory)
                         )
                        .AddHandler(
                             TextDocumentNames.CallHierarchyOutgoing,
                             new LanguageProtocolDelegatingHandlers.RequestRegistration<CallHierarchyOutgoingCallsParams,
                                 Container<CallHierarchyOutgoingCall>?,
                                 CallHierarchyRegistrationOptions>(id, outgoingHandler, registrationOptionsFactory)
                         )
                ;
        }

        public static ILanguageServerRegistry OnCallHierarchy<T>(
            this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams, Task<Container<CallHierarchyItem<T>>?>> handler,
            Func<CallHierarchyIncomingCallsParams<T>, Task<Container<CallHierarchyIncomingCall>?>> incomingHandler,
            Func<CallHierarchyOutgoingCallsParams<T>, Task<Container<CallHierarchyOutgoingCall>?>> outgoingHandler,
            Func<CallHierarchyCapability, CallHierarchyRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CallHierarchyRegistrationOptions();
            return registry.AddHandler(
                new DelegatingCallHierarchyHandler<T>(
                    (request, capability, arg3) => handler(request),
                    (request, capability, arg3) => incomingHandler(request),
                    (request, capability, arg3) => outgoingHandler(request),
                    registrationOptionsFactory
                )
            );
            ;
        }

        public static ILanguageServerRegistry OnCallHierarchy(
            this ILanguageServerRegistry registry,
            Action<CallHierarchyPrepareParams, IObserver<IEnumerable<CallHierarchyItem>>, CallHierarchyCapability, CancellationToken> handler,
            Action<CallHierarchyIncomingCallsParams, IObserver<IEnumerable<CallHierarchyIncomingCall>>, CallHierarchyCapability, CancellationToken> incomingHandler,
            Action<CallHierarchyOutgoingCallsParams, IObserver<IEnumerable<CallHierarchyOutgoingCall>>, CallHierarchyCapability, CancellationToken> outgoingHandler,
            Func<CallHierarchyCapability, CallHierarchyRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= _ => new CallHierarchyRegistrationOptions();
            var id = Guid.NewGuid();
            return registry.AddHandler(
                                TextDocumentNames.PrepareCallHierarchy,
                                _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyPrepareParams,
                                    Container<CallHierarchyItem>?, CallHierarchyItem,
                                    CallHierarchyRegistrationOptions,
                                    CallHierarchyCapability>(id, handler, registrationOptionsFactory, _.GetRequiredService<IProgressManager>(), x => new Container<CallHierarchyItem>(x))
                            )
                           .AddHandler(
                                TextDocumentNames.CallHierarchyIncoming,
                                _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyIncomingCallsParams,
                                    Container<CallHierarchyIncomingCall>?, CallHierarchyIncomingCall,
                                    CallHierarchyRegistrationOptions,
                                    CallHierarchyCapability>(
                                    id, incomingHandler, registrationOptionsFactory,
                                    _.GetRequiredService<IProgressManager>(), x => new Container<CallHierarchyIncomingCall>(x)
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CallHierarchyOutgoing,
                                _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyOutgoingCallsParams,
                                    Container<CallHierarchyOutgoingCall>?, CallHierarchyOutgoingCall,
                                    CallHierarchyRegistrationOptions,
                                    CallHierarchyCapability>(
                                    id, outgoingHandler, registrationOptionsFactory,
                                    _.GetRequiredService<IProgressManager>(), x => new Container<CallHierarchyOutgoingCall>(x)
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCallHierarchy<T>(
            this ILanguageServerRegistry registry,
            Action<CallHierarchyPrepareParams, IObserver<IEnumerable<CallHierarchyItem<T>>>, CallHierarchyCapability, CancellationToken> handler,
            Action<CallHierarchyIncomingCallsParams<T>, IObserver<IEnumerable<CallHierarchyIncomingCall>>, CallHierarchyCapability, CancellationToken> incomingHandler,
            Action<CallHierarchyOutgoingCallsParams<T>, IObserver<IEnumerable<CallHierarchyOutgoingCall>>, CallHierarchyCapability, CancellationToken> outgoingHandler,
            Func<CallHierarchyCapability, CallHierarchyRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CallHierarchyRegistrationOptions();
            return registry.AddHandler(
                _ => new DelegatingPartialCallHierarchyHandler<T>(
                    _.GetRequiredService<IProgressManager>(),
                    handler,
                    incomingHandler,
                    outgoingHandler,
                    registrationOptionsFactory
                )
            );
        }

        public static ILanguageServerRegistry OnCallHierarchy(
            this ILanguageServerRegistry registry,
            Action<CallHierarchyPrepareParams, IObserver<IEnumerable<CallHierarchyItem>>, CallHierarchyCapability> handler,
            Action<CallHierarchyIncomingCallsParams, IObserver<IEnumerable<CallHierarchyIncomingCall>>, CallHierarchyCapability> incomingHandler,
            Action<CallHierarchyOutgoingCallsParams, IObserver<IEnumerable<CallHierarchyOutgoingCall>>, CallHierarchyCapability> outgoingHandler,
            Func<CallHierarchyCapability, CallHierarchyRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= _ => new CallHierarchyRegistrationOptions();
            var id = Guid.NewGuid();
            return
                registry.AddHandler(
                             TextDocumentNames.PrepareCallHierarchy,
                             _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyPrepareParams,
                                 Container<CallHierarchyItem>?, CallHierarchyItem,
                                 CallHierarchyRegistrationOptions,
                                 CallHierarchyCapability>(id, handler, registrationOptionsFactory, _.GetRequiredService<IProgressManager>(), x => new Container<CallHierarchyItem>(x))
                         )
                        .AddHandler(
                             TextDocumentNames.CallHierarchyIncoming,
                             _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyIncomingCallsParams,
                                 Container<CallHierarchyIncomingCall>?, CallHierarchyIncomingCall,
                                 CallHierarchyRegistrationOptions,
                                 CallHierarchyCapability>(
                                 id, incomingHandler, registrationOptionsFactory,
                                 _.GetRequiredService<IProgressManager>(), x => new Container<CallHierarchyIncomingCall>(x)
                             )
                         )
                        .AddHandler(
                             TextDocumentNames.CallHierarchyOutgoing,
                             _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyOutgoingCallsParams,
                                 Container<CallHierarchyOutgoingCall>?, CallHierarchyOutgoingCall,
                                 CallHierarchyRegistrationOptions,
                                 CallHierarchyCapability>(
                                 id, outgoingHandler, registrationOptionsFactory,
                                 _.GetRequiredService<IProgressManager>(), x => new Container<CallHierarchyOutgoingCall>(x)
                             )
                         )
                ;
        }

        public static ILanguageServerRegistry OnCallHierarchy<T>(
            this ILanguageServerRegistry registry,
            Action<CallHierarchyPrepareParams, IObserver<IEnumerable<CallHierarchyItem<T>>>, CallHierarchyCapability> handler,
            Action<CallHierarchyIncomingCallsParams<T>, IObserver<IEnumerable<CallHierarchyIncomingCall>>, CallHierarchyCapability> incomingHandler,
            Action<CallHierarchyOutgoingCallsParams<T>, IObserver<IEnumerable<CallHierarchyOutgoingCall>>, CallHierarchyCapability> outgoingHandler,
            Func<CallHierarchyCapability, CallHierarchyRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CallHierarchyRegistrationOptions();
            return registry.AddHandler(
                _ => new DelegatingPartialCallHierarchyHandler<T>(
                    _.GetRequiredService<IProgressManager>(),
                    (request, observer, arg3, arg4) => handler(request, observer, arg3),
                    (request, observer, arg3, arg4) => incomingHandler(request, observer, arg3),
                    (request, observer, arg3, arg4) => outgoingHandler(request, observer, arg3),
                    registrationOptionsFactory
                )
            );
        }

        public static ILanguageServerRegistry OnCallHierarchy(
            this ILanguageServerRegistry registry,
            Action<CallHierarchyPrepareParams, IObserver<IEnumerable<CallHierarchyItem>>, CancellationToken> handler,
            Action<CallHierarchyIncomingCallsParams, IObserver<IEnumerable<CallHierarchyIncomingCall>>, CancellationToken> incomingHandler,
            Action<CallHierarchyOutgoingCallsParams, IObserver<IEnumerable<CallHierarchyOutgoingCall>>, CancellationToken> outgoingHandler,
            Func<CallHierarchyRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= () => new CallHierarchyRegistrationOptions();
            var id = Guid.NewGuid();
            return
                registry.AddHandler(
                             TextDocumentNames.PrepareCallHierarchy,
                             _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyPrepareParams,
                                 Container<CallHierarchyItem>?, CallHierarchyItem,
                                 CallHierarchyRegistrationOptions>(id, handler, registrationOptionsFactory, _.GetRequiredService<IProgressManager>(), x => new Container<CallHierarchyItem>(x))
                         )
                        .AddHandler(
                             TextDocumentNames.CallHierarchyIncoming,
                             _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyIncomingCallsParams,
                                 Container<CallHierarchyIncomingCall>?, CallHierarchyIncomingCall,
                                 CallHierarchyRegistrationOptions>(
                                 id, incomingHandler, registrationOptionsFactory,
                                 _.GetRequiredService<IProgressManager>(), x => new Container<CallHierarchyIncomingCall>(x)
                             )
                         )
                        .AddHandler(
                             TextDocumentNames.CallHierarchyOutgoing,
                             _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyOutgoingCallsParams,
                                 Container<CallHierarchyOutgoingCall>?, CallHierarchyOutgoingCall,
                                 CallHierarchyRegistrationOptions>(
                                 id, outgoingHandler, registrationOptionsFactory,
                                 _.GetRequiredService<IProgressManager>(), x => new Container<CallHierarchyOutgoingCall>(x)
                             )
                         )
                ;
        }

        public static ILanguageServerRegistry OnCallHierarchy<T>(
            this ILanguageServerRegistry registry,
            Action<CallHierarchyPrepareParams, IObserver<IEnumerable<CallHierarchyItem<T>>>, CancellationToken> handler,
            Action<CallHierarchyIncomingCallsParams<T>, IObserver<IEnumerable<CallHierarchyIncomingCall>>, CancellationToken> incomingHandler,
            Action<CallHierarchyOutgoingCallsParams<T>, IObserver<IEnumerable<CallHierarchyOutgoingCall>>, CancellationToken> outgoingHandler,
            Func<CallHierarchyCapability, CallHierarchyRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CallHierarchyRegistrationOptions();
            return registry.AddHandler(
                _ => new DelegatingPartialCallHierarchyHandler<T>(
                    _.GetRequiredService<IProgressManager>(),
                    (request, observer, arg3, arg4) => handler(request, observer, arg4),
                    (request, observer, arg3, arg4) => incomingHandler(request, observer, arg4),
                    (request, observer, arg3, arg4) => outgoingHandler(request, observer, arg4),
                    registrationOptionsFactory
                )
            );
        }

        public static ILanguageServerRegistry OnCallHierarchy(
            this ILanguageServerRegistry registry,
            Action<CallHierarchyPrepareParams, IObserver<IEnumerable<CallHierarchyItem>>> handler,
            Action<CallHierarchyIncomingCallsParams, IObserver<IEnumerable<CallHierarchyIncomingCall>>> incomingHandler,
            Action<CallHierarchyOutgoingCallsParams, IObserver<IEnumerable<CallHierarchyOutgoingCall>>> outgoingHandler,
            Func<CallHierarchyRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= () => new CallHierarchyRegistrationOptions();
            var id = Guid.NewGuid();
            return
                registry.AddHandler(
                             TextDocumentNames.PrepareCallHierarchy,
                             _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyPrepareParams,
                                 Container<CallHierarchyItem>?, CallHierarchyItem,
                                 CallHierarchyRegistrationOptions>(handler, registrationOptionsFactory, _.GetRequiredService<IProgressManager>(), x => new Container<CallHierarchyItem>(x))
                         )
                        .AddHandler(
                             TextDocumentNames.CallHierarchyIncoming,
                             _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyIncomingCallsParams,
                                 Container<CallHierarchyIncomingCall>?, CallHierarchyIncomingCall,
                                 CallHierarchyRegistrationOptions>(
                                 incomingHandler, registrationOptionsFactory,
                                 _.GetRequiredService<IProgressManager>(), x => new Container<CallHierarchyIncomingCall>(x)
                             )
                         )
                        .AddHandler(
                             TextDocumentNames.CallHierarchyOutgoing,
                             _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyOutgoingCallsParams,
                                 Container<CallHierarchyOutgoingCall>?, CallHierarchyOutgoingCall,
                                 CallHierarchyRegistrationOptions>(
                                 outgoingHandler, registrationOptionsFactory,
                                 _.GetRequiredService<IProgressManager>(), x => new Container<CallHierarchyOutgoingCall>(x)
                             )
                         )
                ;
        }

        public static ILanguageServerRegistry OnCallHierarchy<T>(
            this ILanguageServerRegistry registry,
            Action<CallHierarchyPrepareParams, IObserver<IEnumerable<CallHierarchyItem<T>>>> handler,
            Action<CallHierarchyIncomingCallsParams<T>, IObserver<IEnumerable<CallHierarchyIncomingCall>>> incomingHandler,
            Action<CallHierarchyOutgoingCallsParams<T>, IObserver<IEnumerable<CallHierarchyOutgoingCall>>> outgoingHandler,
            Func<CallHierarchyCapability, CallHierarchyRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CallHierarchyRegistrationOptions();
            return registry.AddHandler(
                _ => new DelegatingPartialCallHierarchyHandler<T>(
                    _.GetRequiredService<IProgressManager>(),
                    (request, observer, arg3, arg4) => handler(request, observer),
                    (request, observer, arg3, arg4) => incomingHandler(request, observer),
                    (request, observer, arg3, arg4) => outgoingHandler(request, observer),
                    registrationOptionsFactory
                )
            );
        }

        private class DelegatingCallHierarchyHandler<T> : CallHierarchyHandlerBase<T> where T : HandlerIdentity?, new()
        {
            private readonly Func<CallHierarchyPrepareParams, CallHierarchyCapability, CancellationToken, Task<Container<CallHierarchyItem<T>>?>> _handlePrepare;

            private readonly Func<CallHierarchyIncomingCallsParams<T>, CallHierarchyCapability, CancellationToken, Task<Container<CallHierarchyIncomingCall>?>>
                _handleIncomingCalls;

            private readonly Func<CallHierarchyOutgoingCallsParams<T>, CallHierarchyCapability, CancellationToken, Task<Container<CallHierarchyOutgoingCall>?>>
                _handleOutgoingCalls;

            private readonly Func<CallHierarchyCapability, CallHierarchyRegistrationOptions> _registrationOptionsFactory;

            public DelegatingCallHierarchyHandler(
                Func<CallHierarchyPrepareParams, CallHierarchyCapability, CancellationToken, Task<Container<CallHierarchyItem<T>>?>> handlePrepare,
                Func<CallHierarchyIncomingCallsParams<T>, CallHierarchyCapability, CancellationToken, Task<Container<CallHierarchyIncomingCall>?>> handleIncomingCalls,
                Func<CallHierarchyOutgoingCallsParams<T>, CallHierarchyCapability, CancellationToken, Task<Container<CallHierarchyOutgoingCall>?>> handleOutgoingCalls,
                Func<CallHierarchyCapability, CallHierarchyRegistrationOptions> registrationOptionsFactory
            )
            {
                _handlePrepare = handlePrepare;
                _handleIncomingCalls = handleIncomingCalls;
                _handleOutgoingCalls = handleOutgoingCalls;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            protected override Task<Container<CallHierarchyItem<T>>?> HandlePrepare(CallHierarchyPrepareParams request, CancellationToken cancellationToken) =>
                _handlePrepare(request, Capability, cancellationToken);

            protected override Task<Container<CallHierarchyIncomingCall>?> HandleIncomingCalls(CallHierarchyIncomingCallsParams<T> request, CancellationToken cancellationToken) =>
                _handleIncomingCalls(request, Capability, cancellationToken);

            protected override Task<Container<CallHierarchyOutgoingCall>?> HandleOutgoingCalls(CallHierarchyOutgoingCallsParams<T> request, CancellationToken cancellationToken) =>
                _handleOutgoingCalls(request, Capability, cancellationToken);

            protected override CallHierarchyRegistrationOptions CreateRegistrationOptions(CallHierarchyCapability capability) => _registrationOptionsFactory(capability);
        }

        private class DelegatingPartialCallHierarchyHandler<T> : PartialCallHierarchyHandlerBase<T> where T : HandlerIdentity?, new()
        {
            private readonly Action<CallHierarchyPrepareParams, IObserver<IEnumerable<CallHierarchyItem<T>>>, CallHierarchyCapability, CancellationToken> _handleParams;

            private readonly Action<CallHierarchyIncomingCallsParams<T>, IObserver<IEnumerable<CallHierarchyIncomingCall>>, CallHierarchyCapability, CancellationToken>
                _handleIncoming;

            private readonly Action<CallHierarchyOutgoingCallsParams<T>, IObserver<IEnumerable<CallHierarchyOutgoingCall>>, CallHierarchyCapability, CancellationToken>
                _handleOutgoing;

            private readonly Func<CallHierarchyCapability, CallHierarchyRegistrationOptions> _registrationOptionsFactory;

            public DelegatingPartialCallHierarchyHandler(
                IProgressManager progressManager,
                Action<CallHierarchyPrepareParams, IObserver<IEnumerable<CallHierarchyItem<T>>>, CallHierarchyCapability, CancellationToken> handleParams,
                Action<CallHierarchyIncomingCallsParams<T>, IObserver<IEnumerable<CallHierarchyIncomingCall>>, CallHierarchyCapability, CancellationToken> handleIncoming,
                Action<CallHierarchyOutgoingCallsParams<T>, IObserver<IEnumerable<CallHierarchyOutgoingCall>>, CallHierarchyCapability, CancellationToken> handleOutgoing,
                Func<CallHierarchyCapability, CallHierarchyRegistrationOptions> registrationOptionsFactory
            ) : base(progressManager)
            {
                _handleParams = handleParams;
                _handleIncoming = handleIncoming;
                _handleOutgoing = handleOutgoing;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            protected override void Handle(CallHierarchyPrepareParams request, IObserver<IEnumerable<CallHierarchyItem<T>>> results, CancellationToken cancellationToken) =>
                _handleParams(request, results, Capability, cancellationToken);

            protected override void Handle(
                CallHierarchyIncomingCallsParams<T> request, IObserver<IEnumerable<CallHierarchyIncomingCall>> results, CancellationToken cancellationToken
            ) => _handleIncoming(request, results, Capability, cancellationToken);

            protected override void Handle(
                CallHierarchyOutgoingCallsParams<T> request, IObserver<IEnumerable<CallHierarchyOutgoingCall>> results, CancellationToken cancellationToken
            ) => _handleOutgoing(request, results, Capability, cancellationToken);

            protected override CallHierarchyRegistrationOptions CreateRegistrationOptions(CallHierarchyCapability capability) => _registrationOptionsFactory(capability);
        }
    }
}
