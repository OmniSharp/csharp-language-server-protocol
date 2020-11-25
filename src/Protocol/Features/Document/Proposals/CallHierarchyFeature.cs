using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models.Proposals
    {
        /// <summary>
        /// The parameter of a `textDocument/prepareCallHierarchy` request.
        ///
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [Parallel]
        [Method(TextDocumentNames.PrepareCallHierarchy, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(CallHierarchyRegistrationOptions)), Capability(typeof(CallHierarchyCapability))]
        public partial class CallHierarchyPrepareParams : TextDocumentPositionParams, IWorkDoneProgressParams,
                                                          IPartialItemsRequest<Container<CallHierarchyItem>?, CallHierarchyItem>
        {
        }

        /// <summary>
        /// Represents programming constructs like functions or constructors in the context
        /// of call hierarchy.
        ///
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        [GenerateTypedData]
        public partial class CallHierarchyItem : ICanBeResolved
        {
            /// <summary>
            /// The name of this item.
            /// </summary>
            public string Name { get; set; } = null!;

            /// <summary>
            /// The kind of this item.
            /// </summary>
            public SymbolKind Kind { get; set; }

            /// <summary>
            /// Tags for this item.
            /// </summary>
            [Optional]
            public Container<SymbolTag>? Tags { get; set; }

            /// <summary>
            /// More detail for this item, e.g. the signature of a function.
            /// </summary>
            [Optional]
            public string? Detail { get; set; }

            /// <summary>
            /// The resource identifier of this item.
            /// </summary>
            public DocumentUri Uri { get; set; } = null!;

            /// <summary>
            /// The range enclosing this symbol not including leading/trailing whitespace but everything else, e.g. comments and code.
            /// </summary>
            public Range Range { get; set; } = null!;

            /// <summary>
            /// The range that should be selected and revealed when this symbol is being picked, e.g. the name of a function.
            /// Must be contained by the [`range`](#CallHierarchyItem.range).
            /// </summary>
            public Range SelectionRange { get; set; } = null!;

            /// <summary>
            /// A data entry field that is preserved between a call hierarchy prepare and
            /// incoming calls or outgoing calls requests.
            /// </summary>
            [Optional]
            public JToken? Data { get; set; }

            private string DebuggerDisplay =>
                $"[{Kind.ToString()}] " +
                $"{Name} " +
                $"@ {Uri} " +
                $"{Range}";

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;
        }

        [Obsolete(Constants.Proposal)]
        public abstract class CallHierarchyBaseCallParams : ICanBeResolved
        {
            public CallHierarchyItem Item { get; set; } = null!;

            JToken? ICanBeResolved.Data
            {
                get => ( (ICanBeResolved) Item )?.Data;
                set {
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    if (Item != null) ( (ICanBeResolved) Item ).Data = value;
                }
            }
        }

        [Obsolete(Constants.Proposal)]
        public abstract class CallHierarchyBaseCallParams<T> : ICanBeResolved
            where T : HandlerIdentity?, new()
        {
            public CallHierarchyItem<T> Item { get; set; } = null!;

            JToken? ICanBeResolved.Data
            {
                get => ( (ICanBeResolved) Item )?.Data;
                set {
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    if (Item != null) ( (ICanBeResolved) Item ).Data = value;
                }
            }
        }

        /// <summary>
        /// The parameter of a `callHierarchy/incomingCalls` request.
        ///
        /// @since 3.16.0
        /// </summary>
        [Parallel]
        [Obsolete(Constants.Proposal)]
        [Method(TextDocumentNames.CallHierarchyIncoming, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals", Name = "CallHierarchyIncoming"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(CallHierarchyRegistrationOptions)), Capability(typeof(CallHierarchyCapability))]
        public partial class CallHierarchyIncomingCallsParams : CallHierarchyBaseCallParams, IWorkDoneProgressParams,
                                                                IPartialItemsRequest<Container<CallHierarchyIncomingCall>?, CallHierarchyIncomingCall>
        {
        }

        /// <summary>
        /// The parameter of a `callHierarchy/incomingCalls` request.
        ///
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [Method(TextDocumentNames.CallHierarchyIncoming, Direction.ClientToServer)]
        public partial class CallHierarchyIncomingCallsParams<T> : CallHierarchyBaseCallParams<T>, IWorkDoneProgressParams,
                                                                   IPartialItemsRequest<Container<CallHierarchyIncomingCall>?, CallHierarchyIncomingCall>
            where T : HandlerIdentity?, new()
        {
        }

        /// <summary>
        /// Represents an incoming call, e.g. a caller of a method or constructor.
        ///
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        public partial class CallHierarchyIncomingCall
        {
            /// <summary>
            /// The item that makes the call.
            /// </summary>
            public CallHierarchyItem From { get; set; } = null!;

            /// <summary>
            /// The range at which at which the calls appears. This is relative to the caller
            /// denoted by [`this.from`](#CallHierarchyIncomingCall.from).
            /// </summary>
            public Container<Range> FromRanges { get; set; } = null!;
        }

        /// <summary>
        /// The parameter of a `callHierarchy/outgoingCalls` request.
        ///
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [Parallel]
        [Method(TextDocumentNames.CallHierarchyOutgoing, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals", Name = "CallHierarchyOutgoing"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(CallHierarchyRegistrationOptions)), Capability(typeof(CallHierarchyCapability))]
        public partial class CallHierarchyOutgoingCallsParams : CallHierarchyBaseCallParams, IWorkDoneProgressParams,
                                                                IPartialItemsRequest<Container<CallHierarchyOutgoingCall>?, CallHierarchyOutgoingCall>

        {
            public static CallHierarchyOutgoingCallsParams<T> Create<T>(CallHierarchyOutgoingCallsParams item)
                where T : HandlerIdentity?, new()
            {
                return new CallHierarchyOutgoingCallsParams<T>() {
                    Item = item.Item,
                    PartialResultToken = item.PartialResultToken,
                    WorkDoneToken = item.PartialResultToken
                };
            }
        }

        /// <summary>
        /// The parameter of a `callHierarchy/outgoingCalls` request.
        ///
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [Method(TextDocumentNames.CallHierarchyOutgoing, Direction.ClientToServer)]
        public partial class CallHierarchyOutgoingCallsParams<T> : CallHierarchyBaseCallParams<T>, IWorkDoneProgressParams,
                                                                   IPartialItemsRequest<Container<CallHierarchyOutgoingCall>?, CallHierarchyOutgoingCall>
            where T : HandlerIdentity?, new()
        {
            public static CallHierarchyOutgoingCallsParams Create(CallHierarchyOutgoingCallsParams<T> item)
            {
                return new CallHierarchyOutgoingCallsParams() {
                    Item = item.Item,
                    PartialResultToken = item.PartialResultToken,
                    WorkDoneToken = item.PartialResultToken
                };
            }

            public static implicit operator CallHierarchyOutgoingCallsParams(CallHierarchyOutgoingCallsParams<T> item) => Create(item);
            public static implicit operator CallHierarchyOutgoingCallsParams<T>(CallHierarchyOutgoingCallsParams item) => CallHierarchyOutgoingCallsParams.Create<T>(item);
        }

        /// <summary>
        /// Represents an outgoing call, e.g. calling a getter from a method or a method from a constructor etc.
        ///
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        public partial class CallHierarchyOutgoingCall
        {
            /// <summary>
            /// The item that is called.
            /// </summary>
            public CallHierarchyItem To { get; set; } = null!;

            /// <summary>
            /// The range at which this item is called. This is the range relative to the caller, e.g the item
            /// passed to [`provideCallHierarchyOutgoingCalls`](#CallHierarchyItemProvider.provideCallHierarchyOutgoingCalls)
            /// and not [`this.to`](#CallHierarchyOutgoingCall.to).
            /// </summary>
            public Container<Range> FromRanges { get; set; } = null!;
        }

        /// <summary>
        /// Call hierarchy options used during static or dynamic registration.
        ///
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [GenerateRegistrationOptions(nameof(ServerCapabilities.CallHierarchyProvider))]
        public partial class CallHierarchyRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions, IStaticRegistrationOptions
        {
        }
    }

    namespace Client.Capabilities
    {
        /// <summary>
        /// Capabilities specific to the `textDocument/callHierarchy`.
        ///
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.CallHierarchy))]
        public partial class CallHierarchyCapability : DynamicCapability, ConnectedCapability<ICallHierarchyPrepareHandler>,
                                                       ConnectedCapability<ICallHierarchyIncomingHandler>, ConnectedCapability<ICallHierarchyOutgoingHandler>
        {
        }
    }

    namespace Document.Proposals
    {
        [Obsolete(Constants.Proposal)]
        public abstract class CallHierarchyHandlerBase : AbstractHandlers.Base<CallHierarchyRegistrationOptions, CallHierarchyCapability>, ICallHierarchyPrepareHandler,
                                                         ICallHierarchyIncomingHandler,
                                                         ICallHierarchyOutgoingHandler
        {
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            protected CallHierarchyHandlerBase(Guid id)
            {
                _id = id;
            }

            protected CallHierarchyHandlerBase() : this(Guid.NewGuid())
            {
            }

            public abstract Task<Container<CallHierarchyItem>?> Handle(CallHierarchyPrepareParams request, CancellationToken cancellationToken);
            public abstract Task<Container<CallHierarchyIncomingCall>?> Handle(CallHierarchyIncomingCallsParams request, CancellationToken cancellationToken);
            public abstract Task<Container<CallHierarchyOutgoingCall>?> Handle(CallHierarchyOutgoingCallsParams request, CancellationToken cancellationToken);
        }

        [Obsolete(Constants.Proposal)]
        public abstract class PartialCallHierarchyHandlerBase : AbstractHandlers.PartialResults<CallHierarchyPrepareParams, Container<CallHierarchyItem>?, CallHierarchyItem,
                                                                    CallHierarchyRegistrationOptions, CallHierarchyCapability>,
                                                                ICallHierarchyPrepareHandler, ICallHierarchyIncomingHandler, ICallHierarchyOutgoingHandler
        {
            private readonly ICallHierarchyIncomingHandler _incoming;
            private readonly ICallHierarchyOutgoingHandler _outgoing;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            protected PartialCallHierarchyHandlerBase(Guid id, IProgressManager progressManager) : base(progressManager, Container<CallHierarchyItem>.From)
            {
                _id = id;
                _incoming = new PartialIncoming(id, progressManager, this);
                _outgoing = new PartialOutgoing(id, progressManager, this);
            }

            protected PartialCallHierarchyHandlerBase(IProgressManager progressManager) : this(Guid.NewGuid(), progressManager)
            {
            }

            public Task<Container<CallHierarchyIncomingCall>?> Handle(CallHierarchyIncomingCallsParams request, CancellationToken cancellationToken) =>
                _incoming.Handle(request, cancellationToken);

            public Task<Container<CallHierarchyOutgoingCall>?> Handle(CallHierarchyOutgoingCallsParams request, CancellationToken cancellationToken) =>
                _outgoing.Handle(request, cancellationToken);

            protected abstract void Handle(
                CallHierarchyIncomingCallsParams request, IObserver<IEnumerable<CallHierarchyIncomingCall>> results, CancellationToken cancellationToken
            );

            protected abstract void Handle(
                CallHierarchyOutgoingCallsParams request, IObserver<IEnumerable<CallHierarchyOutgoingCall>> results, CancellationToken cancellationToken
            );

            private class PartialIncoming : AbstractHandlers.PartialResults<
                                                CallHierarchyIncomingCallsParams,
                                                Container<CallHierarchyIncomingCall>?,
                                                CallHierarchyIncomingCall,
                                                CallHierarchyRegistrationOptions,
                                                CallHierarchyCapability
                                            >, ICallHierarchyIncomingHandler
            {
                private readonly PartialCallHierarchyHandlerBase _self;
                private readonly Guid _id;
                Guid ICanBeIdentifiedHandler.Id => _id;

                public PartialIncoming(Guid id, IProgressManager progressManager, PartialCallHierarchyHandlerBase self) :
                    base(progressManager, Container<CallHierarchyIncomingCall>.From)
                {
                    _id = id;
                    _self = self;
                }

                protected override void Handle(
                    CallHierarchyIncomingCallsParams request,
                    IObserver<IEnumerable<CallHierarchyIncomingCall>> results,
                    CancellationToken cancellationToken
                ) => _self.Handle(request, results, cancellationToken);

                protected internal override CallHierarchyRegistrationOptions CreateRegistrationOptions(CallHierarchyCapability capability) =>
                    ( (IRegistration<CallHierarchyRegistrationOptions, CallHierarchyCapability>) _self ).GetRegistrationOptions(capability);
            }

            class PartialOutgoing :
                AbstractHandlers.PartialResults<CallHierarchyOutgoingCallsParams, Container<CallHierarchyOutgoingCall>?, CallHierarchyOutgoingCall, CallHierarchyRegistrationOptions
                  , CallHierarchyCapability>, ICallHierarchyOutgoingHandler
            {
                private readonly PartialCallHierarchyHandlerBase _self;
                private readonly Guid _id;
                Guid ICanBeIdentifiedHandler.Id => _id;

                public PartialOutgoing(Guid id, IProgressManager progressManager, PartialCallHierarchyHandlerBase self) :
                    base(progressManager, Container<CallHierarchyOutgoingCall>.From)

                {
                    _id = id;
                    _self = self;
                }

                protected override void Handle(
                    CallHierarchyOutgoingCallsParams request, IObserver<IEnumerable<CallHierarchyOutgoingCall>> results, CancellationToken cancellationToken
                ) => _self.Handle(request, results, cancellationToken);

                protected internal override CallHierarchyRegistrationOptions CreateRegistrationOptions(CallHierarchyCapability capability) =>
                    ( (IRegistration<CallHierarchyRegistrationOptions, CallHierarchyCapability>) _self ).GetRegistrationOptions(capability);
            }
        }

        [Obsolete(Constants.Proposal)]
        public abstract class CallHierarchyHandlerBase<T> : CallHierarchyHandlerBase where T : HandlerIdentity?, new()
        {
            protected CallHierarchyHandlerBase(Guid id) : base(id)
            {
            }

            protected CallHierarchyHandlerBase() : this(Guid.NewGuid())
            {
            }

            public sealed override async Task<Container<CallHierarchyItem>?> Handle(CallHierarchyPrepareParams request, CancellationToken cancellationToken)
            {
                var response = await HandlePrepare(request, cancellationToken);
                return Container<CallHierarchyItem>.From(response?.Select(CallHierarchyItem.From)!);
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

        [Obsolete(Constants.Proposal)]
        public abstract class PartialCallHierarchyHandlerBase<T> : PartialCallHierarchyHandlerBase where T : HandlerIdentity?, new()
        {
            protected PartialCallHierarchyHandlerBase(IProgressManager progressManager) : base(progressManager)
            {
            }

            protected sealed override void Handle(CallHierarchyPrepareParams request, IObserver<IEnumerable<CallHierarchyItem>> results, CancellationToken cancellationToken) =>
                Handle(
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

            protected abstract void Handle(
                CallHierarchyIncomingCallsParams<T> request, IObserver<IEnumerable<CallHierarchyIncomingCall>> results, CancellationToken cancellationToken
            );

            protected sealed override void Handle(
                CallHierarchyOutgoingCallsParams request, IObserver<IEnumerable<CallHierarchyOutgoingCall>> results, CancellationToken cancellationToken
            ) => Handle(
                new CallHierarchyOutgoingCallsParams<T> {
                    Item = request.Item,
                    PartialResultToken = request.PartialResultToken,
                    WorkDoneToken = request.WorkDoneToken
                },
                results,
                cancellationToken
            );

            protected abstract void Handle(
                CallHierarchyOutgoingCallsParams<T> request, IObserver<IEnumerable<CallHierarchyOutgoingCall>> results, CancellationToken cancellationToken
            );
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
                var id = Guid.NewGuid();
                return registry
                      .AddHandler(
                           TextDocumentNames.PrepareCallHierarchy,
                           new LanguageProtocolDelegatingHandlers.Request<
                               CallHierarchyPrepareParams,
                               Container<CallHierarchyItem>?,
                               CallHierarchyRegistrationOptions,
                               CallHierarchyCapability
                           >(
                               id,
                               HandlerAdapter<CallHierarchyCapability>.Adapt(handler),
                               RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory)
                           )
                       )
                      .AddHandler(
                           TextDocumentNames.CallHierarchyIncoming,
                           new LanguageProtocolDelegatingHandlers.Request<
                               CallHierarchyIncomingCallsParams,
                               Container<CallHierarchyIncomingCall>?,
                               CallHierarchyRegistrationOptions,
                               CallHierarchyCapability
                           >(
                               id,
                               HandlerAdapter<CallHierarchyCapability>.Adapt(incomingHandler),
                               RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory)
                           )
                       )
                      .AddHandler(
                           TextDocumentNames.CallHierarchyOutgoing,
                           new LanguageProtocolDelegatingHandlers.Request<
                               CallHierarchyOutgoingCallsParams,
                               Container<CallHierarchyOutgoingCall>?,
                               CallHierarchyRegistrationOptions,
                               CallHierarchyCapability
                           >(
                               id,
                               HandlerAdapter<CallHierarchyCapability>.Adapt(outgoingHandler),
                               RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory)
                           )
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
                return registry.AddHandler(
                    new DelegatingCallHierarchyHandler<T>(
                        HandlerAdapter<CallHierarchyCapability>.Adapt(handler),
                        HandlerAdapter<CallHierarchyCapability>.Adapt(incomingHandler),
                        HandlerAdapter<CallHierarchyCapability>.Adapt(outgoingHandler),
                        RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory)
                    )
                );
            }

            public static ILanguageServerRegistry OnCallHierarchy(
                this ILanguageServerRegistry registry,
                Func<CallHierarchyPrepareParams, CallHierarchyCapability, Task<Container<CallHierarchyItem>?>> handler,
                Func<CallHierarchyIncomingCallsParams, CallHierarchyCapability, Task<Container<CallHierarchyIncomingCall>?>> incomingHandler,
                Func<CallHierarchyOutgoingCallsParams, CallHierarchyCapability, Task<Container<CallHierarchyOutgoingCall>?>> outgoingHandler,
                Func<CallHierarchyCapability, CallHierarchyRegistrationOptions>? registrationOptionsFactory
            )
            {
                var id = Guid.NewGuid();
                return registry
                      .AddHandler(
                           TextDocumentNames.PrepareCallHierarchy,
                           new LanguageProtocolDelegatingHandlers.Request<
                               CallHierarchyPrepareParams,
                               Container<CallHierarchyItem>?,
                               CallHierarchyRegistrationOptions,
                               CallHierarchyCapability
                           >(
                               id,
                               HandlerAdapter<CallHierarchyCapability>.Adapt(handler),
                               RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory)
                           )
                       )
                      .AddHandler(
                           TextDocumentNames.CallHierarchyIncoming,
                           new LanguageProtocolDelegatingHandlers.Request<
                               CallHierarchyIncomingCallsParams,
                               Container<CallHierarchyIncomingCall>?,
                               CallHierarchyRegistrationOptions,
                               CallHierarchyCapability
                           >(
                               id,
                               HandlerAdapter<CallHierarchyCapability>.Adapt(incomingHandler),
                               RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory)
                           )
                       )
                      .AddHandler(
                           TextDocumentNames.CallHierarchyOutgoing,
                           new LanguageProtocolDelegatingHandlers.Request<CallHierarchyOutgoingCallsParams,
                               Container<CallHierarchyOutgoingCall>?,
                               CallHierarchyRegistrationOptions,
                               CallHierarchyCapability
                           >(
                               id,
                               HandlerAdapter<CallHierarchyCapability>.Adapt(outgoingHandler),
                               RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory)
                           )
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
                return registry.AddHandler(
                    new DelegatingCallHierarchyHandler<T>(
                        HandlerAdapter<CallHierarchyCapability>.Adapt(handler),
                        HandlerAdapter<CallHierarchyCapability>.Adapt(incomingHandler),
                        HandlerAdapter<CallHierarchyCapability>.Adapt(outgoingHandler),
                        RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory)
                    )
                );
            }

            public static ILanguageServerRegistry OnCallHierarchy(
                this ILanguageServerRegistry registry,
                Func<CallHierarchyPrepareParams, CancellationToken, Task<Container<CallHierarchyItem>?>> handler,
                Func<CallHierarchyIncomingCallsParams, CancellationToken, Task<Container<CallHierarchyIncomingCall>?>> incomingHandler,
                Func<CallHierarchyOutgoingCallsParams, CancellationToken, Task<Container<CallHierarchyOutgoingCall>?>> outgoingHandler,
                Func<CallHierarchyRegistrationOptions>? registrationOptionsFactory
            )
            {
                var id = Guid.NewGuid();
                return
                    registry
                       .AddHandler(
                            TextDocumentNames.PrepareCallHierarchy,
                            new LanguageProtocolDelegatingHandlers.Request<
                                CallHierarchyPrepareParams,
                                Container<CallHierarchyItem>?,
                                CallHierarchyRegistrationOptions,
                                CallHierarchyCapability
                            >(
                                id,
                                HandlerAdapter<CallHierarchyCapability>.Adapt(handler),
                                RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory)
                            )
                        )
                       .AddHandler(
                            TextDocumentNames.CallHierarchyIncoming,
                            new LanguageProtocolDelegatingHandlers.Request<
                                CallHierarchyIncomingCallsParams,
                                Container<CallHierarchyIncomingCall>?,
                                CallHierarchyRegistrationOptions,
                                CallHierarchyCapability
                            >(
                                id, HandlerAdapter<CallHierarchyCapability>.Adapt(incomingHandler),
                                RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory)
                            )
                        )
                       .AddHandler(
                            TextDocumentNames.CallHierarchyOutgoing,
                            new LanguageProtocolDelegatingHandlers.Request<
                                CallHierarchyOutgoingCallsParams,
                                Container<CallHierarchyOutgoingCall>?,
                                CallHierarchyRegistrationOptions,
                                CallHierarchyCapability
                            >(
                                id, HandlerAdapter<CallHierarchyCapability>.Adapt(outgoingHandler),
                                RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory)
                            )
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
                return registry.AddHandler(
                    new DelegatingCallHierarchyHandler<T>(
                        HandlerAdapter<CallHierarchyCapability>.Adapt(handler),
                        HandlerAdapter<CallHierarchyCapability>.Adapt(incomingHandler),
                        HandlerAdapter<CallHierarchyCapability>.Adapt(outgoingHandler),
                        RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory)
                    )
                );
            }

            public static ILanguageServerRegistry OnCallHierarchy(
                this ILanguageServerRegistry registry,
                Func<CallHierarchyPrepareParams, Task<Container<CallHierarchyItem>?>> handler,
                Func<CallHierarchyIncomingCallsParams, Task<Container<CallHierarchyIncomingCall>?>> incomingHandler,
                Func<CallHierarchyOutgoingCallsParams, Task<Container<CallHierarchyOutgoingCall>?>> outgoingHandler,
                Func<CallHierarchyRegistrationOptions>? registrationOptionsFactory
            )
            {
                var id = Guid.NewGuid();
                return
                    registry
                       .AddHandler(
                            TextDocumentNames.PrepareCallHierarchy,
                            new LanguageProtocolDelegatingHandlers.Request<
                                CallHierarchyPrepareParams,
                                Container<CallHierarchyItem>?,
                                CallHierarchyRegistrationOptions,
                                CallHierarchyCapability
                            >(
                                id,
                                HandlerAdapter<CallHierarchyCapability>.Adapt(handler),
                                RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory)
                            )
                        )
                       .AddHandler(
                            TextDocumentNames.CallHierarchyIncoming,
                            new LanguageProtocolDelegatingHandlers.Request<
                                CallHierarchyIncomingCallsParams,
                                Container<CallHierarchyIncomingCall>?,
                                CallHierarchyRegistrationOptions,
                                CallHierarchyCapability
                            >(
                                id,
                                HandlerAdapter<CallHierarchyCapability>.Adapt(incomingHandler),
                                RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory)
                            )
                        )
                       .AddHandler(
                            TextDocumentNames.CallHierarchyOutgoing,
                            new LanguageProtocolDelegatingHandlers.Request<
                                CallHierarchyOutgoingCallsParams,
                                Container<CallHierarchyOutgoingCall>?,
                                CallHierarchyRegistrationOptions,
                                CallHierarchyCapability
                            >(
                                id,
                                HandlerAdapter<CallHierarchyCapability>.Adapt(outgoingHandler),
                                RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory)
                            )
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
                return registry.AddHandler(
                    new DelegatingCallHierarchyHandler<T>(
                        HandlerAdapter<CallHierarchyCapability>.Adapt(handler),
                        HandlerAdapter<CallHierarchyCapability>.Adapt(incomingHandler),
                        HandlerAdapter<CallHierarchyCapability>.Adapt(outgoingHandler),
                        RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory)
                    )
                );
            }

            public static ILanguageServerRegistry OnCallHierarchy(
                this ILanguageServerRegistry registry,
                Action<CallHierarchyPrepareParams, IObserver<IEnumerable<CallHierarchyItem>>, CallHierarchyCapability, CancellationToken> handler,
                Action<CallHierarchyIncomingCallsParams, IObserver<IEnumerable<CallHierarchyIncomingCall>>, CallHierarchyCapability, CancellationToken> incomingHandler,
                Action<CallHierarchyOutgoingCallsParams, IObserver<IEnumerable<CallHierarchyOutgoingCall>>, CallHierarchyCapability, CancellationToken> outgoingHandler,
                Func<CallHierarchyCapability, CallHierarchyRegistrationOptions>? registrationOptionsFactory
            )
            {
                var id = Guid.NewGuid();
                return registry.AddHandler(
                                    TextDocumentNames.PrepareCallHierarchy,
                                    _ => new LanguageProtocolDelegatingHandlers.PartialResults<
                                        CallHierarchyPrepareParams,
                                        Container<CallHierarchyItem>?, CallHierarchyItem,
                                        CallHierarchyRegistrationOptions,
                                        CallHierarchyCapability
                                    >(
                                        id,
                                        PartialAdapter<CallHierarchyCapability>.Adapt(handler),
                                        RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory),
                                        _.GetRequiredService<IProgressManager>(),
                                        Container<CallHierarchyItem>.From
                                    )
                                )
                               .AddHandler(
                                    TextDocumentNames.CallHierarchyIncoming,
                                    _ => new LanguageProtocolDelegatingHandlers.PartialResults<
                                        CallHierarchyIncomingCallsParams,
                                        Container<CallHierarchyIncomingCall>?, CallHierarchyIncomingCall,
                                        CallHierarchyRegistrationOptions,
                                        CallHierarchyCapability
                                    >(
                                        id,
                                        PartialAdapter<CallHierarchyCapability>.Adapt(incomingHandler),
                                        RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory),
                                        _.GetRequiredService<IProgressManager>(),
                                        Container<CallHierarchyIncomingCall>.From
                                    )
                                )
                               .AddHandler(
                                    TextDocumentNames.CallHierarchyOutgoing,
                                    _ => new LanguageProtocolDelegatingHandlers.PartialResults<
                                        CallHierarchyOutgoingCallsParams,
                                        Container<CallHierarchyOutgoingCall>?, CallHierarchyOutgoingCall,
                                        CallHierarchyRegistrationOptions,
                                        CallHierarchyCapability
                                    >(
                                        id,
                                        PartialAdapter<CallHierarchyCapability>.Adapt(outgoingHandler),
                                        RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory),
                                        _.GetRequiredService<IProgressManager>(),
                                        Container<CallHierarchyOutgoingCall>.From
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
                return registry.AddHandler(
                    _ =>
                        new DelegatingPartialCallHierarchyHandler<T>(
                            _.GetRequiredService<IProgressManager>(),
                            PartialAdapter<CallHierarchyCapability>.Adapt(handler),
                            PartialAdapter<CallHierarchyCapability>.Adapt(incomingHandler),
                            PartialAdapter<CallHierarchyCapability>.Adapt(outgoingHandler),
                            RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory)
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
                var id = Guid.NewGuid();
                return
                    registry.AddHandler(
                                 TextDocumentNames.PrepareCallHierarchy,
                                 _ => new LanguageProtocolDelegatingHandlers.PartialResults<
                                     CallHierarchyPrepareParams,
                                     Container<CallHierarchyItem>?, CallHierarchyItem,
                                     CallHierarchyRegistrationOptions,
                                     CallHierarchyCapability
                                 >(
                                     id,
                                     PartialAdapter<CallHierarchyCapability>.Adapt(handler),
                                     RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory),
                                     _.GetRequiredService<IProgressManager>(),
                                     Container<CallHierarchyItem>.From
                                 )
                             )
                            .AddHandler(
                                 TextDocumentNames.CallHierarchyIncoming,
                                 _ => new LanguageProtocolDelegatingHandlers.PartialResults<
                                     CallHierarchyIncomingCallsParams,
                                     Container<CallHierarchyIncomingCall>?, CallHierarchyIncomingCall,
                                     CallHierarchyRegistrationOptions,
                                     CallHierarchyCapability
                                 >(
                                     id,
                                     PartialAdapter<CallHierarchyCapability>.Adapt(incomingHandler),
                                     RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory),
                                     _.GetRequiredService<IProgressManager>(),
                                     Container<CallHierarchyIncomingCall>.From
                                 )
                             )
                            .AddHandler(
                                 TextDocumentNames.CallHierarchyOutgoing,
                                 _ => new LanguageProtocolDelegatingHandlers.PartialResults<
                                     CallHierarchyOutgoingCallsParams,
                                     Container<CallHierarchyOutgoingCall>?, CallHierarchyOutgoingCall,
                                     CallHierarchyRegistrationOptions,
                                     CallHierarchyCapability
                                 >(
                                     id,
                                     PartialAdapter<CallHierarchyCapability>.Adapt(outgoingHandler),
                                     RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory),
                                     _.GetRequiredService<IProgressManager>(),
                                     Container<CallHierarchyOutgoingCall>.From
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
                return registry.AddHandler(
                    _ => new DelegatingPartialCallHierarchyHandler<T>(
                        _.GetRequiredService<IProgressManager>(),
                        PartialAdapter<CallHierarchyCapability>.Adapt(handler),
                        PartialAdapter<CallHierarchyCapability>.Adapt(incomingHandler),
                        PartialAdapter<CallHierarchyCapability>.Adapt(outgoingHandler),
                        RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory)
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
                var id = Guid.NewGuid();
                return
                    registry.AddHandler(
                                 TextDocumentNames.PrepareCallHierarchy,
                                 _ => new LanguageProtocolDelegatingHandlers.PartialResults<
                                     CallHierarchyPrepareParams,
                                     Container<CallHierarchyItem>?,
                                     CallHierarchyItem,
                                     CallHierarchyRegistrationOptions
                                 >(
                                     id,
                                     PartialAdapter.Adapt(handler),
                                     RegistrationAdapter.Adapt(registrationOptionsFactory),
                                     _.GetRequiredService<IProgressManager>(),
                                     Container<CallHierarchyItem>.From
                                 )
                             )
                            .AddHandler(
                                 TextDocumentNames.CallHierarchyIncoming,
                                 _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyIncomingCallsParams,
                                     Container<CallHierarchyIncomingCall>?, CallHierarchyIncomingCall,
                                     CallHierarchyRegistrationOptions
                                 >(
                                     id,
                                     PartialAdapter.Adapt(incomingHandler),
                                     RegistrationAdapter.Adapt(registrationOptionsFactory),
                                     _.GetRequiredService<IProgressManager>(),
                                     Container<CallHierarchyIncomingCall>.From
                                 )
                             )
                            .AddHandler(
                                 TextDocumentNames.CallHierarchyOutgoing,
                                 _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyOutgoingCallsParams,
                                     Container<CallHierarchyOutgoingCall>?, CallHierarchyOutgoingCall,
                                     CallHierarchyRegistrationOptions
                                 >(
                                     id,
                                     PartialAdapter.Adapt(outgoingHandler),
                                     RegistrationAdapter.Adapt(registrationOptionsFactory),
                                     _.GetRequiredService<IProgressManager>(),
                                     Container<CallHierarchyOutgoingCall>.From
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
                return registry.AddHandler(
                    _ => new DelegatingPartialCallHierarchyHandler<T>(
                        _.GetRequiredService<IProgressManager>(),
                        PartialAdapter<CallHierarchyCapability>.Adapt(handler),
                        PartialAdapter<CallHierarchyCapability>.Adapt(incomingHandler),
                        PartialAdapter<CallHierarchyCapability>.Adapt(outgoingHandler),
                        RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory)
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
                var id = Guid.NewGuid();
                return
                    registry.AddHandler(
                                 TextDocumentNames.PrepareCallHierarchy,
                                 _ => new LanguageProtocolDelegatingHandlers.PartialResults<
                                     CallHierarchyPrepareParams,
                                     Container<CallHierarchyItem>?, CallHierarchyItem,
                                     CallHierarchyRegistrationOptions
                                 >(
                                     id,
                                     PartialAdapter.Adapt(handler),
                                     RegistrationAdapter.Adapt(registrationOptionsFactory),
                                     _.GetRequiredService<IProgressManager>(),
                                     Container<CallHierarchyItem>.From
                                 )
                             )
                            .AddHandler(
                                 TextDocumentNames.CallHierarchyIncoming,
                                 _ => new LanguageProtocolDelegatingHandlers.PartialResults<
                                     CallHierarchyIncomingCallsParams,
                                     Container<CallHierarchyIncomingCall>?,
                                     CallHierarchyIncomingCall,
                                     CallHierarchyRegistrationOptions
                                 >(
                                     id,
                                     PartialAdapter.Adapt(incomingHandler),
                                     RegistrationAdapter.Adapt(registrationOptionsFactory),
                                     _.GetRequiredService<IProgressManager>(),
                                     Container<CallHierarchyIncomingCall>.From
                                 )
                             )
                            .AddHandler(
                                 TextDocumentNames.CallHierarchyOutgoing,
                                 _ => new LanguageProtocolDelegatingHandlers.PartialResults<
                                     CallHierarchyOutgoingCallsParams,
                                     Container<CallHierarchyOutgoingCall>?,
                                     CallHierarchyOutgoingCall,
                                     CallHierarchyRegistrationOptions
                                 >(
                                     id,
                                     PartialAdapter.Adapt(outgoingHandler),
                                     RegistrationAdapter.Adapt(registrationOptionsFactory),
                                     _.GetRequiredService<IProgressManager>(),
                                     Container<CallHierarchyOutgoingCall>.From
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
                return registry.AddHandler(
                    _ => new DelegatingPartialCallHierarchyHandler<T>(
                        _.GetRequiredService<IProgressManager>(),
                        PartialAdapter<CallHierarchyCapability>.Adapt(handler),
                        PartialAdapter<CallHierarchyCapability>.Adapt(incomingHandler),
                        PartialAdapter<CallHierarchyCapability>.Adapt(outgoingHandler),
                        RegistrationAdapter<CallHierarchyCapability>.Adapt(registrationOptionsFactory)
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

                protected override Task<Container<CallHierarchyIncomingCall>?> HandleIncomingCalls(
                    CallHierarchyIncomingCallsParams<T> request, CancellationToken cancellationToken
                ) =>
                    _handleIncomingCalls(request, Capability, cancellationToken);

                protected override Task<Container<CallHierarchyOutgoingCall>?> HandleOutgoingCalls(
                    CallHierarchyOutgoingCallsParams<T> request, CancellationToken cancellationToken
                ) =>
                    _handleOutgoingCalls(request, Capability, cancellationToken);

                protected internal override CallHierarchyRegistrationOptions CreateRegistrationOptions(CallHierarchyCapability capability) => _registrationOptionsFactory(capability);
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

                protected internal override CallHierarchyRegistrationOptions CreateRegistrationOptions(CallHierarchyCapability capability) => _registrationOptionsFactory(capability);
            }
        }
    }
}
