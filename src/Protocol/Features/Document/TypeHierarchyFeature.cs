using System;
using System.Collections.Generic;
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
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        /// <summary>
        /// The parameter of a `textDocument/prepareTypeHierarchy` request.
        ///
        /// @since 3.16.0
        /// </summary>
        [Parallel]
        [Method(TextDocumentNames.PrepareTypeHierarchy, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(TypeHierarchyRegistrationOptions))]
        [Capability(typeof(TypeHierarchyCapability))]
        public partial record TypeHierarchyPrepareParams : TextDocumentPositionParams, IWorkDoneProgressParams,
                                                           IPartialItemsRequest<Container<TypeHierarchyItem>?, TypeHierarchyItem>;

        /// <summary>
        /// Represents programming constructs like functions or constructors in the context
        /// of call hierarchy.
        ///
        /// @since 3.16.0
        /// </summary>
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        [GenerateTypedData]
        public partial record TypeHierarchyItem : ICanBeResolved
        {
            /// <summary>
            /// The name of this item.
            /// </summary>
            public string Name { get; init; } = null!;

            /// <summary>
            /// The kind of this item.
            /// </summary>
            public SymbolKind Kind { get; init; }

            /// <summary>
            /// Tags for this item.
            /// </summary>
            [Optional]
            public Container<SymbolTag>? Tags { get; init; }

            /// <summary>
            /// More detail for this item, e.g. the signature of a function.
            /// </summary>
            [Optional]
            public string? Detail { get; init; }

            /// <summary>
            /// The resource identifier of this item.
            /// </summary>
            public DocumentUri Uri { get; init; } = null!;

            /// <summary>
            /// The range enclosing this symbol not including leading/trailing whitespace but everything else, e.g. comments and code.
            /// </summary>
            public Range Range { get; init; } = null!;

            /// <summary>
            /// The range that should be selected and revealed when this symbol is being picked, e.g. the name of a function.
            /// Must be contained by the [`range`](#TypeHierarchyItem.range).
            /// </summary>
            public Range SelectionRange { get; init; } = null!;

            /// <summary>
            /// A data entry field that is preserved between a call hierarchy prepare and
            /// supertypes calls or subtypes calls requests.
            /// </summary>
            [Optional]
            public JToken? Data { get; init; }

            private string DebuggerDisplay =>
                $"[{Kind.ToString()}] " +
                $"{Name} " +
                $"@ {Uri} " +
                $"{Range}";

            /// <inheritdoc />
            public override string ToString()
            {
                return DebuggerDisplay;
            }
        }

        public abstract record TypeHierarchyBaseCallParams : ICanBeResolved
        {
            public TypeHierarchyItem Item { get; init; } = null!;

            JToken? ICanBeResolved.Data
            {
                get => Item.GetRawData();
                init => Item.SetRawData(value);
            }
        }

        public abstract record TypeHierarchyBaseCallParams<T> : ICanBeResolved
            where T : class?, IHandlerIdentity?
        {
            public TypeHierarchyItem<T> Item { get; init; } = null!;

            JToken? ICanBeResolved.Data
            {
                get => Item.GetRawData();
                init => Item.SetRawData(value);
            }
        }

        /// <summary>
        /// The parameter of a `TypeHierarchy/supertypesCalls` request.
        ///
        /// @since 3.16.0
        /// </summary>
        [Parallel]
        [Method(TextDocumentNames.TypeHierarchySupertypes, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document", Name = "TypeHierarchySupertypes")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
        [Capability(typeof(TypeHierarchyCapability))]
        public partial record TypeHierarchySupertypesCallsParams : TypeHierarchyBaseCallParams, IWorkDoneProgressParams,
                                                                 IPartialItemsRequest<Container<TypeHierarchySupertypesCall>?, TypeHierarchySupertypesCall>,
                                                                 IDoesNotParticipateInRegistration;

        /// <summary>
        /// The parameter of a `TypeHierarchy/supertypesCalls` request.
        ///
        /// @since 3.16.0
        /// </summary>
        [Method(TextDocumentNames.TypeHierarchySupertypes, Direction.ClientToServer)]
        public partial record TypeHierarchySupertypesCallsParams<T> : TypeHierarchyBaseCallParams<T>, IWorkDoneProgressParams,
                                                                    IPartialItemsRequest<Container<TypeHierarchySupertypesCall>?, TypeHierarchySupertypesCall>,
                                                                    IDoesNotParticipateInRegistration
            where T : class?, IHandlerIdentity?;

        /// <summary>
        /// Represents an supertypes call, e.g. a caller of a method or constructor.
        ///
        /// @since 3.16.0
        /// </summary>
        public partial record TypeHierarchySupertypesCall
        {
            /// <summary>
            /// The item that makes the call.
            /// </summary>
            public TypeHierarchyItem From { get; init; } = null!;

            /// <summary>
            /// The range at which at which the calls appears. This is relative to the caller
            /// denoted by [`this.from`](#TypeHierarchySupertypesCall.from).
            /// </summary>
            public Container<Range> FromRanges { get; init; } = null!;
        }

        /// <summary>
        /// The parameter of a `TypeHierarchy/subtypesCalls` request.
        ///
        /// @since 3.16.0
        /// </summary>
        [Parallel]
        [Method(TextDocumentNames.TypeHierarchySubtypes, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document", Name = "TypeHierarchySubtypes")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
        [Capability(typeof(TypeHierarchyCapability))]
        public partial record TypeHierarchySubtypesCallsParams : TypeHierarchyBaseCallParams, IWorkDoneProgressParams,
                                                                 IPartialItemsRequest<Container<TypeHierarchySubtypesCall>?, TypeHierarchySubtypesCall>,
                                                                 IDoesNotParticipateInRegistration

        {
            public static TypeHierarchySubtypesCallsParams<T> Create<T>(TypeHierarchySubtypesCallsParams item)
                where T : class?, IHandlerIdentity?
            {
                return new TypeHierarchySubtypesCallsParams<T>
                {
                    Item = item.Item,
                    PartialResultToken = item.PartialResultToken,
                    WorkDoneToken = item.PartialResultToken
                };
            }
        }

        /// <summary>
        /// The parameter of a `TypeHierarchy/subtypesCalls` request.
        ///
        /// @since 3.16.0
        /// </summary>
        [Method(TextDocumentNames.TypeHierarchySubtypes, Direction.ClientToServer)]
        public partial record TypeHierarchySubtypesCallsParams<T> : TypeHierarchyBaseCallParams<T>, IWorkDoneProgressParams,
                                                                    IPartialItemsRequest<Container<TypeHierarchySubtypesCall>?, TypeHierarchySubtypesCall>,
                                                                    IDoesNotParticipateInRegistration
            where T : class?, IHandlerIdentity?
        {
            public static TypeHierarchySubtypesCallsParams Create(TypeHierarchySubtypesCallsParams<T> item)
            {
                return new TypeHierarchySubtypesCallsParams
                {
                    Item = item.Item,
                    PartialResultToken = item.PartialResultToken,
                    WorkDoneToken = item.PartialResultToken
                };
            }

            public static implicit operator TypeHierarchySubtypesCallsParams(TypeHierarchySubtypesCallsParams<T> item)
            {
                return Create(item);
            }

            public static implicit operator TypeHierarchySubtypesCallsParams<T>(TypeHierarchySubtypesCallsParams item)
            {
                return TypeHierarchySubtypesCallsParams.Create<T>(item);
            }
        }

        /// <summary>
        /// Represents an subtypes call, e.g. calling a getter from a method or a method from a constructor etc.
        ///
        /// @since 3.16.0
        /// </summary>
        public partial record TypeHierarchySubtypesCall
        {
            /// <summary>
            /// The item that is called.
            /// </summary>
            public TypeHierarchyItem To { get; init; } = null!;

            /// <summary>
            /// The range at which this item is called. This is the range relative to the caller, e.g the item
            /// passed to [`provideTypeHierarchySubtypesCalls`](#TypeHierarchyItemProvider.provideTypeHierarchySubtypesCalls)
            /// and not [`this.to`](#TypeHierarchySubtypesCall.to).
            /// </summary>
            public Container<Range> FromRanges { get; init; } = null!;
        }

        /// <summary>
        /// Call hierarchy options used during static or dynamic registration.
        ///
        /// @since 3.16.0
        /// </summary>
        [GenerateRegistrationOptions(nameof(ServerCapabilities.TypeHierarchyProvider))]
        [RegistrationName(TextDocumentNames.PrepareTypeHierarchy)]
        public partial class TypeHierarchyRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions, IStaticRegistrationOptions
        {
        }
    }

    namespace Client.Capabilities
    {
        /// <summary>
        /// Capabilities specific to the `textDocument/TypeHierarchy`.
        ///
        /// @since 3.16.0
        /// </summary>
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.TypeHierarchy))]
        public partial class TypeHierarchyCapability : DynamicCapability
        {
        }
    }

    namespace Document
    {
        public abstract class TypeHierarchyHandlerBase : AbstractHandlers.Base<TypeHierarchyRegistrationOptions, TypeHierarchyCapability>,
                                                         ITypeHierarchyPrepareHandler,
                                                         ITypeHierarchySupertypesHandler,
                                                         ITypeHierarchySubtypesHandler
        {
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            protected TypeHierarchyHandlerBase(Guid id)
            {
                _id = id;
            }

            protected TypeHierarchyHandlerBase() : this(Guid.NewGuid())
            {
            }

            public abstract Task<Container<TypeHierarchyItem>?> Handle(TypeHierarchyPrepareParams request, CancellationToken cancellationToken);
            public abstract Task<Container<TypeHierarchySupertypesCall>?> Handle(TypeHierarchySupertypesCallsParams request, CancellationToken cancellationToken);
            public abstract Task<Container<TypeHierarchySubtypesCall>?> Handle(TypeHierarchySubtypesCallsParams request, CancellationToken cancellationToken);
        }

        public abstract class PartialTypeHierarchyHandlerBase : AbstractHandlers.PartialResults<TypeHierarchyPrepareParams, Container<TypeHierarchyItem>?,
                                                                    TypeHierarchyItem,
                                                                    TypeHierarchyRegistrationOptions, TypeHierarchyCapability>,
                                                                ITypeHierarchyPrepareHandler, ITypeHierarchySupertypesHandler, ITypeHierarchySubtypesHandler
        {
            private readonly ITypeHierarchySupertypesHandler _supertypes;
            private readonly ITypeHierarchySubtypesHandler _subtypes;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            protected PartialTypeHierarchyHandlerBase(Guid id, IProgressManager progressManager) : base(progressManager, Container<TypeHierarchyItem>.From)
            {
                _id = id;
                _supertypes = new PartialSupertypes(id, progressManager, this);
                _subtypes = new PartialSubtypes(id, progressManager, this);
            }

            protected PartialTypeHierarchyHandlerBase(IProgressManager progressManager) : this(Guid.NewGuid(), progressManager)
            {
            }

            public Task<Container<TypeHierarchySupertypesCall>?> Handle(TypeHierarchySupertypesCallsParams request, CancellationToken cancellationToken)
            {
                return _supertypes.Handle(request, cancellationToken);
            }

            public Task<Container<TypeHierarchySubtypesCall>?> Handle(TypeHierarchySubtypesCallsParams request, CancellationToken cancellationToken)
            {
                return _subtypes.Handle(request, cancellationToken);
            }

            protected abstract void Handle(
                TypeHierarchySupertypesCallsParams request, IObserver<IEnumerable<TypeHierarchySupertypesCall>> results, CancellationToken cancellationToken
            );

            protected abstract void Handle(
                TypeHierarchySubtypesCallsParams request, IObserver<IEnumerable<TypeHierarchySubtypesCall>> results, CancellationToken cancellationToken
            );

            private class PartialSupertypes : AbstractHandlers.PartialResults<
                                                TypeHierarchySupertypesCallsParams,
                                                Container<TypeHierarchySupertypesCall>?,
                                                TypeHierarchySupertypesCall,
                                                TypeHierarchyRegistrationOptions,
                                                TypeHierarchyCapability
                                            >, ITypeHierarchySupertypesHandler
            {
                private readonly PartialTypeHierarchyHandlerBase _self;
                private readonly Guid _id;
                Guid ICanBeIdentifiedHandler.Id => _id;

                public PartialSupertypes(Guid id, IProgressManager progressManager, PartialTypeHierarchyHandlerBase self) :
                    base(progressManager, Container<TypeHierarchySupertypesCall>.From)
                {
                    _id = id;
                    _self = self;
                }

                protected override void Handle(
                    TypeHierarchySupertypesCallsParams request,
                    IObserver<IEnumerable<TypeHierarchySupertypesCall>> results,
                    CancellationToken cancellationToken
                )
                {
                    _self.Handle(request, results, cancellationToken);
                }

                protected internal override TypeHierarchyRegistrationOptions CreateRegistrationOptions(
                    TypeHierarchyCapability capability, ClientCapabilities clientCapabilities
                )
                {
                    return ( (IRegistration<TypeHierarchyRegistrationOptions, TypeHierarchyCapability>)_self ).GetRegistrationOptions(
                        capability, clientCapabilities
                    );
                }
            }

            private class PartialSubtypes : AbstractHandlers.PartialResults<TypeHierarchySubtypesCallsParams, Container<TypeHierarchySubtypesCall>?,
                                                TypeHierarchySubtypesCall, TypeHierarchyRegistrationOptions
                                              , TypeHierarchyCapability>, ITypeHierarchySubtypesHandler
            {
                private readonly PartialTypeHierarchyHandlerBase _self;
                private readonly Guid _id;
                Guid ICanBeIdentifiedHandler.Id => _id;

                public PartialSubtypes(Guid id, IProgressManager progressManager, PartialTypeHierarchyHandlerBase self) :
                    base(progressManager, Container<TypeHierarchySubtypesCall>.From)

                {
                    _id = id;
                    _self = self;
                }

                protected override void Handle(
                    TypeHierarchySubtypesCallsParams request, IObserver<IEnumerable<TypeHierarchySubtypesCall>> results, CancellationToken cancellationToken
                )
                {
                    _self.Handle(request, results, cancellationToken);
                }

                protected internal override TypeHierarchyRegistrationOptions CreateRegistrationOptions(
                    TypeHierarchyCapability capability, ClientCapabilities clientCapabilities
                )
                {
                    return ( (IRegistration<TypeHierarchyRegistrationOptions, TypeHierarchyCapability>)_self ).GetRegistrationOptions(
                        capability, clientCapabilities
                    );
                }
            }
        }

        public abstract class TypeHierarchyHandlerBase<T> : TypeHierarchyHandlerBase where T : class?, IHandlerIdentity?
        {
            protected TypeHierarchyHandlerBase(Guid id) : base(id)
            {
            }

            protected TypeHierarchyHandlerBase() : this(Guid.NewGuid())
            {
            }

            public sealed override async Task<Container<TypeHierarchyItem>?> Handle(TypeHierarchyPrepareParams request, CancellationToken cancellationToken)
            {
                var response = await HandlePrepare(request, cancellationToken).ConfigureAwait(false);
                return Container<TypeHierarchyItem>.From(response?.Select(TypeHierarchyItem.From)!);
            }

            public sealed override Task<Container<TypeHierarchySupertypesCall>?> Handle(
                TypeHierarchySupertypesCallsParams request, CancellationToken cancellationToken
            )
            {
                return HandleSupertypesCalls(
                    new TypeHierarchySupertypesCallsParams<T>
                    {
                        Item = request.Item,
                        PartialResultToken = request.PartialResultToken,
                        WorkDoneToken = request.WorkDoneToken
                    },
                    cancellationToken
                );
            }

            public sealed override Task<Container<TypeHierarchySubtypesCall>?> Handle(
                TypeHierarchySubtypesCallsParams request, CancellationToken cancellationToken
            )
            {
                return HandleSubtypesCalls(
                    new TypeHierarchySubtypesCallsParams<T>
                    {
                        Item = request.Item,
                        PartialResultToken = request.PartialResultToken,
                        WorkDoneToken = request.WorkDoneToken
                    },
                    cancellationToken
                );
            }

            protected abstract Task<Container<TypeHierarchyItem<T>>?> HandlePrepare(TypeHierarchyPrepareParams request, CancellationToken cancellationToken);

            protected abstract Task<Container<TypeHierarchySupertypesCall>?> HandleSupertypesCalls(
                TypeHierarchySupertypesCallsParams<T> request, CancellationToken cancellationToken
            );

            protected abstract Task<Container<TypeHierarchySubtypesCall>?> HandleSubtypesCalls(
                TypeHierarchySubtypesCallsParams<T> request, CancellationToken cancellationToken
            );
        }

        public abstract class PartialTypeHierarchyHandlerBase<T> : PartialTypeHierarchyHandlerBase where T : class?, IHandlerIdentity?
        {
            protected PartialTypeHierarchyHandlerBase(IProgressManager progressManager) : base(progressManager)
            {
            }

            protected sealed override void Handle(
                TypeHierarchyPrepareParams request, IObserver<IEnumerable<TypeHierarchyItem>> results, CancellationToken cancellationToken
            )
            {
                Handle(
                    request,
                    Observer.Create<IEnumerable<TypeHierarchyItem<T>>>(
                        x => results.OnNext(x.Select(z => (TypeHierarchyItem)z)),
                        results.OnError,
                        results.OnCompleted
                    ), cancellationToken
                );
            }

            protected abstract void Handle(
                TypeHierarchyPrepareParams request, IObserver<IEnumerable<TypeHierarchyItem<T>>> results, CancellationToken cancellationToken
            );

            protected sealed override void Handle(
                TypeHierarchySupertypesCallsParams request, IObserver<IEnumerable<TypeHierarchySupertypesCall>> results, CancellationToken cancellationToken
            )
            {
                Handle(
                    new TypeHierarchySupertypesCallsParams<T>
                    {
                        Item = request.Item,
                        PartialResultToken = request.PartialResultToken,
                        WorkDoneToken = request.WorkDoneToken
                    },
                    results,
                    cancellationToken
                );
            }

            protected abstract void Handle(
                TypeHierarchySupertypesCallsParams<T> request, IObserver<IEnumerable<TypeHierarchySupertypesCall>> results, CancellationToken cancellationToken
            );

            protected sealed override void Handle(
                TypeHierarchySubtypesCallsParams request, IObserver<IEnumerable<TypeHierarchySubtypesCall>> results, CancellationToken cancellationToken
            )
            {
                Handle(
                    new TypeHierarchySubtypesCallsParams<T>
                    {
                        Item = request.Item,
                        PartialResultToken = request.PartialResultToken,
                        WorkDoneToken = request.WorkDoneToken
                    },
                    results,
                    cancellationToken
                );
            }

            protected abstract void Handle(
                TypeHierarchySubtypesCallsParams<T> request, IObserver<IEnumerable<TypeHierarchySubtypesCall>> results, CancellationToken cancellationToken
            );
        }

        public static partial class TypeHierarchyExtensions
        {
            public static ILanguageServerRegistry OnTypeHierarchy(
                this ILanguageServerRegistry registry,
                Func<TypeHierarchyPrepareParams, TypeHierarchyCapability, CancellationToken, Task<Container<TypeHierarchyItem>?>> handler,
                Func<TypeHierarchySupertypesCallsParams, TypeHierarchyCapability, CancellationToken, Task<Container<TypeHierarchySupertypesCall>?>> supertypesHandler,
                Func<TypeHierarchySubtypesCallsParams, TypeHierarchyCapability, CancellationToken, Task<Container<TypeHierarchySubtypesCall>?>> subtypesHandler,
                RegistrationOptionsDelegate<TypeHierarchyRegistrationOptions, TypeHierarchyCapability>? registrationOptionsFactory
            )
            {
                var id = Guid.NewGuid();
                return registry
                      .AddHandler(
                           TextDocumentNames.PrepareTypeHierarchy,
                           new LanguageProtocolDelegatingHandlers.Request<
                               TypeHierarchyPrepareParams,
                               Container<TypeHierarchyItem>?,
                               TypeHierarchyRegistrationOptions,
                               TypeHierarchyCapability
                           >(
                               id,
                               HandlerAdapter<TypeHierarchyCapability>.Adapt(handler),
                               RegistrationAdapter<TypeHierarchyCapability>.Adapt(registrationOptionsFactory)
                           )
                       )
                      .AddHandler(
                           TextDocumentNames.TypeHierarchySupertypes,
                           new LanguageProtocolDelegatingHandlers.Request<
                               TypeHierarchySupertypesCallsParams,
                               Container<TypeHierarchySupertypesCall>?,
                               TypeHierarchyRegistrationOptions,
                               TypeHierarchyCapability
                           >(
                               id,
                               HandlerAdapter<TypeHierarchyCapability>.Adapt(supertypesHandler),
                               RegistrationAdapter<TypeHierarchyCapability>.Adapt(registrationOptionsFactory)
                           )
                       )
                      .AddHandler(
                           TextDocumentNames.TypeHierarchySubtypes,
                           new LanguageProtocolDelegatingHandlers.Request<
                               TypeHierarchySubtypesCallsParams,
                               Container<TypeHierarchySubtypesCall>?,
                               TypeHierarchyRegistrationOptions,
                               TypeHierarchyCapability
                           >(
                               id,
                               HandlerAdapter<TypeHierarchyCapability>.Adapt(subtypesHandler),
                               RegistrationAdapter<TypeHierarchyCapability>.Adapt(registrationOptionsFactory)
                           )
                       )
                    ;
            }

            public static ILanguageServerRegistry OnTypeHierarchy<T>(
                this ILanguageServerRegistry registry,
                Func<TypeHierarchyPrepareParams, TypeHierarchyCapability, CancellationToken, Task<Container<TypeHierarchyItem<T>>?>> handler,
                Func<TypeHierarchySupertypesCallsParams<T>, TypeHierarchyCapability, CancellationToken, Task<Container<TypeHierarchySupertypesCall>?>>
                    supertypesHandler,
                Func<TypeHierarchySubtypesCallsParams<T>, TypeHierarchyCapability, CancellationToken, Task<Container<TypeHierarchySubtypesCall>?>>
                    subtypesHandler,
                RegistrationOptionsDelegate<TypeHierarchyRegistrationOptions, TypeHierarchyCapability>? registrationOptionsFactory
            ) where T : class?, IHandlerIdentity?
            {
                return registry.AddHandler(
                    new DelegatingTypeHierarchyHandler<T>(
                        HandlerAdapter<TypeHierarchyCapability>.Adapt(handler),
                        HandlerAdapter<TypeHierarchyCapability>.Adapt(supertypesHandler),
                        HandlerAdapter<TypeHierarchyCapability>.Adapt(subtypesHandler),
                        RegistrationAdapter<TypeHierarchyCapability>.Adapt(registrationOptionsFactory)
                    )
                );
            }

            public static ILanguageServerRegistry OnTypeHierarchy(
                this ILanguageServerRegistry registry,
                Func<TypeHierarchyPrepareParams, TypeHierarchyCapability, Task<Container<TypeHierarchyItem>?>> handler,
                Func<TypeHierarchySupertypesCallsParams, TypeHierarchyCapability, Task<Container<TypeHierarchySupertypesCall>?>> supertypesHandler,
                Func<TypeHierarchySubtypesCallsParams, TypeHierarchyCapability, Task<Container<TypeHierarchySubtypesCall>?>> subtypesHandler,
                RegistrationOptionsDelegate<TypeHierarchyRegistrationOptions, TypeHierarchyCapability>? registrationOptionsFactory
            )
            {
                var id = Guid.NewGuid();
                return registry
                      .AddHandler(
                           TextDocumentNames.PrepareTypeHierarchy,
                           new LanguageProtocolDelegatingHandlers.Request<
                               TypeHierarchyPrepareParams,
                               Container<TypeHierarchyItem>?,
                               TypeHierarchyRegistrationOptions,
                               TypeHierarchyCapability
                           >(
                               id,
                               HandlerAdapter<TypeHierarchyCapability>.Adapt(handler),
                               RegistrationAdapter<TypeHierarchyCapability>.Adapt(registrationOptionsFactory)
                           )
                       )
                      .AddHandler(
                           TextDocumentNames.TypeHierarchySupertypes,
                           new LanguageProtocolDelegatingHandlers.Request<
                               TypeHierarchySupertypesCallsParams,
                               Container<TypeHierarchySupertypesCall>?,
                               TypeHierarchyRegistrationOptions,
                               TypeHierarchyCapability
                           >(
                               id,
                               HandlerAdapter<TypeHierarchyCapability>.Adapt(supertypesHandler),
                               RegistrationAdapter<TypeHierarchyCapability>.Adapt(registrationOptionsFactory)
                           )
                       )
                      .AddHandler(
                           TextDocumentNames.TypeHierarchySubtypes,
                           new LanguageProtocolDelegatingHandlers.Request<TypeHierarchySubtypesCallsParams,
                               Container<TypeHierarchySubtypesCall>?,
                               TypeHierarchyRegistrationOptions,
                               TypeHierarchyCapability
                           >(
                               id,
                               HandlerAdapter<TypeHierarchyCapability>.Adapt(subtypesHandler),
                               RegistrationAdapter<TypeHierarchyCapability>.Adapt(registrationOptionsFactory)
                           )
                       )
                    ;
            }

            public static ILanguageServerRegistry OnTypeHierarchy<T>(
                this ILanguageServerRegistry registry,
                Func<TypeHierarchyPrepareParams, TypeHierarchyCapability, Task<Container<TypeHierarchyItem<T>>?>> handler,
                Func<TypeHierarchySupertypesCallsParams<T>, TypeHierarchyCapability, Task<Container<TypeHierarchySupertypesCall>?>> supertypesHandler,
                Func<TypeHierarchySubtypesCallsParams<T>, TypeHierarchyCapability, Task<Container<TypeHierarchySubtypesCall>?>> subtypesHandler,
                RegistrationOptionsDelegate<TypeHierarchyRegistrationOptions, TypeHierarchyCapability>? registrationOptionsFactory
            ) where T : class?, IHandlerIdentity?
            {
                return registry.AddHandler(
                    new DelegatingTypeHierarchyHandler<T>(
                        HandlerAdapter<TypeHierarchyCapability>.Adapt(handler),
                        HandlerAdapter<TypeHierarchyCapability>.Adapt(supertypesHandler),
                        HandlerAdapter<TypeHierarchyCapability>.Adapt(subtypesHandler),
                        RegistrationAdapter<TypeHierarchyCapability>.Adapt(registrationOptionsFactory)
                    )
                );
            }

            public static ILanguageServerRegistry OnTypeHierarchy<T>(
                this ILanguageServerRegistry registry,
                Func<TypeHierarchyPrepareParams, CancellationToken, Task<Container<TypeHierarchyItem<T>>?>> handler,
                Func<TypeHierarchySupertypesCallsParams<T>, CancellationToken, Task<Container<TypeHierarchySupertypesCall>?>> supertypesHandler,
                Func<TypeHierarchySubtypesCallsParams<T>, CancellationToken, Task<Container<TypeHierarchySubtypesCall>?>> subtypesHandler,
                RegistrationOptionsDelegate<TypeHierarchyRegistrationOptions, TypeHierarchyCapability>? registrationOptionsFactory
            ) where T : class?, IHandlerIdentity?
            {
                return registry.AddHandler(
                    new DelegatingTypeHierarchyHandler<T>(
                        HandlerAdapter<TypeHierarchyCapability>.Adapt(handler),
                        HandlerAdapter<TypeHierarchyCapability>.Adapt(supertypesHandler),
                        HandlerAdapter<TypeHierarchyCapability>.Adapt(subtypesHandler),
                        RegistrationAdapter<TypeHierarchyCapability>.Adapt(registrationOptionsFactory)
                    )
                );
            }

            public static ILanguageServerRegistry OnTypeHierarchy<T>(
                this ILanguageServerRegistry registry,
                Func<TypeHierarchyPrepareParams, Task<Container<TypeHierarchyItem<T>>?>> handler,
                Func<TypeHierarchySupertypesCallsParams<T>, Task<Container<TypeHierarchySupertypesCall>?>> supertypesHandler,
                Func<TypeHierarchySubtypesCallsParams<T>, Task<Container<TypeHierarchySubtypesCall>?>> subtypesHandler,
                RegistrationOptionsDelegate<TypeHierarchyRegistrationOptions, TypeHierarchyCapability>? registrationOptionsFactory
            ) where T : class?, IHandlerIdentity?
            {
                return registry.AddHandler(
                    new DelegatingTypeHierarchyHandler<T>(
                        HandlerAdapter<TypeHierarchyCapability>.Adapt(handler),
                        HandlerAdapter<TypeHierarchyCapability>.Adapt(supertypesHandler),
                        HandlerAdapter<TypeHierarchyCapability>.Adapt(subtypesHandler),
                        RegistrationAdapter<TypeHierarchyCapability>.Adapt(registrationOptionsFactory)
                    )
                );
            }

            public static ILanguageServerRegistry OnTypeHierarchy(
                this ILanguageServerRegistry registry,
                Action<TypeHierarchyPrepareParams, IObserver<IEnumerable<TypeHierarchyItem>>, TypeHierarchyCapability, CancellationToken> handler,
                Action<TypeHierarchySupertypesCallsParams, IObserver<IEnumerable<TypeHierarchySupertypesCall>>, TypeHierarchyCapability, CancellationToken>
                    supertypesHandler,
                Action<TypeHierarchySubtypesCallsParams, IObserver<IEnumerable<TypeHierarchySubtypesCall>>, TypeHierarchyCapability, CancellationToken>
                    subtypesHandler,
                RegistrationOptionsDelegate<TypeHierarchyRegistrationOptions, TypeHierarchyCapability>? registrationOptionsFactory
            )
            {
                var id = Guid.NewGuid();
                return registry.AddHandler(
                                    TextDocumentNames.PrepareTypeHierarchy,
                                    _ => new LanguageProtocolDelegatingHandlers.PartialResults<
                                        TypeHierarchyPrepareParams,
                                        Container<TypeHierarchyItem>?, TypeHierarchyItem,
                                        TypeHierarchyRegistrationOptions,
                                        TypeHierarchyCapability
                                    >(
                                        id,
                                        PartialAdapter<TypeHierarchyCapability>.Adapt(handler),
                                        RegistrationAdapter<TypeHierarchyCapability>.Adapt(registrationOptionsFactory),
                                        _.GetRequiredService<IProgressManager>(),
                                        Container<TypeHierarchyItem>.From
                                    )
                                )
                               .AddHandler(
                                    TextDocumentNames.TypeHierarchySupertypes,
                                    _ => new LanguageProtocolDelegatingHandlers.PartialResults<
                                        TypeHierarchySupertypesCallsParams,
                                        Container<TypeHierarchySupertypesCall>?, TypeHierarchySupertypesCall,
                                        TypeHierarchyRegistrationOptions,
                                        TypeHierarchyCapability
                                    >(
                                        id,
                                        PartialAdapter<TypeHierarchyCapability>.Adapt(supertypesHandler),
                                        RegistrationAdapter<TypeHierarchyCapability>.Adapt(registrationOptionsFactory),
                                        _.GetRequiredService<IProgressManager>(),
                                        Container<TypeHierarchySupertypesCall>.From
                                    )
                                )
                               .AddHandler(
                                    TextDocumentNames.TypeHierarchySubtypes,
                                    _ => new LanguageProtocolDelegatingHandlers.PartialResults<
                                        TypeHierarchySubtypesCallsParams,
                                        Container<TypeHierarchySubtypesCall>?, TypeHierarchySubtypesCall,
                                        TypeHierarchyRegistrationOptions,
                                        TypeHierarchyCapability
                                    >(
                                        id,
                                        PartialAdapter<TypeHierarchyCapability>.Adapt(subtypesHandler),
                                        RegistrationAdapter<TypeHierarchyCapability>.Adapt(registrationOptionsFactory),
                                        _.GetRequiredService<IProgressManager>(),
                                        Container<TypeHierarchySubtypesCall>.From
                                    )
                                )
                    ;
            }

            public static ILanguageServerRegistry OnTypeHierarchy<T>(
                this ILanguageServerRegistry registry,
                Action<TypeHierarchyPrepareParams, IObserver<IEnumerable<TypeHierarchyItem<T>>>, TypeHierarchyCapability, CancellationToken> handler,
                Action<TypeHierarchySupertypesCallsParams<T>, IObserver<IEnumerable<TypeHierarchySupertypesCall>>, TypeHierarchyCapability, CancellationToken>
                    supertypesHandler,
                Action<TypeHierarchySubtypesCallsParams<T>, IObserver<IEnumerable<TypeHierarchySubtypesCall>>, TypeHierarchyCapability, CancellationToken>
                    subtypesHandler,
                RegistrationOptionsDelegate<TypeHierarchyRegistrationOptions, TypeHierarchyCapability>? registrationOptionsFactory
            ) where T : class?, IHandlerIdentity?
            {
                return registry.AddHandler(
                    _ =>
                        new DelegatingPartialTypeHierarchyHandler<T>(
                            _.GetRequiredService<IProgressManager>(),
                            PartialAdapter<TypeHierarchyCapability>.Adapt(handler),
                            PartialAdapter<TypeHierarchyCapability>.Adapt(supertypesHandler),
                            PartialAdapter<TypeHierarchyCapability>.Adapt(subtypesHandler),
                            RegistrationAdapter<TypeHierarchyCapability>.Adapt(registrationOptionsFactory)
                        )
                );
            }

            public static ILanguageServerRegistry OnTypeHierarchy(
                this ILanguageServerRegistry registry,
                Action<TypeHierarchyPrepareParams, IObserver<IEnumerable<TypeHierarchyItem>>, TypeHierarchyCapability> handler,
                Action<TypeHierarchySupertypesCallsParams, IObserver<IEnumerable<TypeHierarchySupertypesCall>>, TypeHierarchyCapability> supertypesHandler,
                Action<TypeHierarchySubtypesCallsParams, IObserver<IEnumerable<TypeHierarchySubtypesCall>>, TypeHierarchyCapability> subtypesHandler,
                RegistrationOptionsDelegate<TypeHierarchyRegistrationOptions, TypeHierarchyCapability>? registrationOptionsFactory
            )
            {
                var id = Guid.NewGuid();
                return
                    registry.AddHandler(
                                 TextDocumentNames.PrepareTypeHierarchy,
                                 _ => new LanguageProtocolDelegatingHandlers.PartialResults<
                                     TypeHierarchyPrepareParams,
                                     Container<TypeHierarchyItem>?, TypeHierarchyItem,
                                     TypeHierarchyRegistrationOptions,
                                     TypeHierarchyCapability
                                 >(
                                     id,
                                     PartialAdapter<TypeHierarchyCapability>.Adapt(handler),
                                     RegistrationAdapter<TypeHierarchyCapability>.Adapt(registrationOptionsFactory),
                                     _.GetRequiredService<IProgressManager>(),
                                     Container<TypeHierarchyItem>.From
                                 )
                             )
                            .AddHandler(
                                 TextDocumentNames.TypeHierarchySupertypes,
                                 _ => new LanguageProtocolDelegatingHandlers.PartialResults<
                                     TypeHierarchySupertypesCallsParams,
                                     Container<TypeHierarchySupertypesCall>?, TypeHierarchySupertypesCall,
                                     TypeHierarchyRegistrationOptions,
                                     TypeHierarchyCapability
                                 >(
                                     id,
                                     PartialAdapter<TypeHierarchyCapability>.Adapt(supertypesHandler),
                                     RegistrationAdapter<TypeHierarchyCapability>.Adapt(registrationOptionsFactory),
                                     _.GetRequiredService<IProgressManager>(),
                                     Container<TypeHierarchySupertypesCall>.From
                                 )
                             )
                            .AddHandler(
                                 TextDocumentNames.TypeHierarchySubtypes,
                                 _ => new LanguageProtocolDelegatingHandlers.PartialResults<
                                     TypeHierarchySubtypesCallsParams,
                                     Container<TypeHierarchySubtypesCall>?, TypeHierarchySubtypesCall,
                                     TypeHierarchyRegistrationOptions,
                                     TypeHierarchyCapability
                                 >(
                                     id,
                                     PartialAdapter<TypeHierarchyCapability>.Adapt(subtypesHandler),
                                     RegistrationAdapter<TypeHierarchyCapability>.Adapt(registrationOptionsFactory),
                                     _.GetRequiredService<IProgressManager>(),
                                     Container<TypeHierarchySubtypesCall>.From
                                 )
                             )
                    ;
            }

            public static ILanguageServerRegistry OnTypeHierarchy<T>(
                this ILanguageServerRegistry registry,
                Action<TypeHierarchyPrepareParams, IObserver<IEnumerable<TypeHierarchyItem<T>>>, TypeHierarchyCapability> handler,
                Action<TypeHierarchySupertypesCallsParams<T>, IObserver<IEnumerable<TypeHierarchySupertypesCall>>, TypeHierarchyCapability> supertypesHandler,
                Action<TypeHierarchySubtypesCallsParams<T>, IObserver<IEnumerable<TypeHierarchySubtypesCall>>, TypeHierarchyCapability> subtypesHandler,
                RegistrationOptionsDelegate<TypeHierarchyRegistrationOptions, TypeHierarchyCapability>? registrationOptionsFactory
            ) where T : class?, IHandlerIdentity?
            {
                return registry.AddHandler(
                    _ => new DelegatingPartialTypeHierarchyHandler<T>(
                        _.GetRequiredService<IProgressManager>(),
                        PartialAdapter<TypeHierarchyCapability>.Adapt(handler),
                        PartialAdapter<TypeHierarchyCapability>.Adapt(supertypesHandler),
                        PartialAdapter<TypeHierarchyCapability>.Adapt(subtypesHandler),
                        RegistrationAdapter<TypeHierarchyCapability>.Adapt(registrationOptionsFactory)
                    )
                );
            }

            public static ILanguageServerRegistry OnTypeHierarchy<T>(
                this ILanguageServerRegistry registry,
                Action<TypeHierarchyPrepareParams, IObserver<IEnumerable<TypeHierarchyItem<T>>>, CancellationToken> handler,
                Action<TypeHierarchySupertypesCallsParams<T>, IObserver<IEnumerable<TypeHierarchySupertypesCall>>, CancellationToken> supertypesHandler,
                Action<TypeHierarchySubtypesCallsParams<T>, IObserver<IEnumerable<TypeHierarchySubtypesCall>>, CancellationToken> subtypesHandler,
                RegistrationOptionsDelegate<TypeHierarchyRegistrationOptions, TypeHierarchyCapability>? registrationOptionsFactory
            ) where T : class?, IHandlerIdentity?
            {
                return registry.AddHandler(
                    _ => new DelegatingPartialTypeHierarchyHandler<T>(
                        _.GetRequiredService<IProgressManager>(),
                        PartialAdapter<TypeHierarchyCapability>.Adapt(handler),
                        PartialAdapter<TypeHierarchyCapability>.Adapt(supertypesHandler),
                        PartialAdapter<TypeHierarchyCapability>.Adapt(subtypesHandler),
                        RegistrationAdapter<TypeHierarchyCapability>.Adapt(registrationOptionsFactory)
                    )
                );
            }

            public static ILanguageServerRegistry OnTypeHierarchy<T>(
                this ILanguageServerRegistry registry,
                Action<TypeHierarchyPrepareParams, IObserver<IEnumerable<TypeHierarchyItem<T>>>> handler,
                Action<TypeHierarchySupertypesCallsParams<T>, IObserver<IEnumerable<TypeHierarchySupertypesCall>>> supertypesHandler,
                Action<TypeHierarchySubtypesCallsParams<T>, IObserver<IEnumerable<TypeHierarchySubtypesCall>>> subtypesHandler,
                RegistrationOptionsDelegate<TypeHierarchyRegistrationOptions, TypeHierarchyCapability>? registrationOptionsFactory
            ) where T : class?, IHandlerIdentity?
            {
                return registry.AddHandler(
                    _ => new DelegatingPartialTypeHierarchyHandler<T>(
                        _.GetRequiredService<IProgressManager>(),
                        PartialAdapter<TypeHierarchyCapability>.Adapt(handler),
                        PartialAdapter<TypeHierarchyCapability>.Adapt(supertypesHandler),
                        PartialAdapter<TypeHierarchyCapability>.Adapt(subtypesHandler),
                        RegistrationAdapter<TypeHierarchyCapability>.Adapt(registrationOptionsFactory)
                    )
                );
            }

            private class DelegatingTypeHierarchyHandler<T> : TypeHierarchyHandlerBase<T> where T : class?, IHandlerIdentity?
            {
                private readonly Func<TypeHierarchyPrepareParams, TypeHierarchyCapability, CancellationToken, Task<Container<TypeHierarchyItem<T>>?>>
                    _handlePrepare;

                private readonly Func<TypeHierarchySupertypesCallsParams<T>, TypeHierarchyCapability, CancellationToken,
                        Task<Container<TypeHierarchySupertypesCall>?>>
                    _handleSupertypesCalls;

                private readonly Func<TypeHierarchySubtypesCallsParams<T>, TypeHierarchyCapability, CancellationToken,
                        Task<Container<TypeHierarchySubtypesCall>?>>
                    _handleSubtypesCalls;

                private readonly RegistrationOptionsDelegate<TypeHierarchyRegistrationOptions, TypeHierarchyCapability> _registrationOptionsFactory;

                public DelegatingTypeHierarchyHandler(
                    Func<TypeHierarchyPrepareParams, TypeHierarchyCapability, CancellationToken, Task<Container<TypeHierarchyItem<T>>?>> handlePrepare,
                    Func<TypeHierarchySupertypesCallsParams<T>, TypeHierarchyCapability, CancellationToken, Task<Container<TypeHierarchySupertypesCall>?>>
                        handleSupertypesCalls,
                    Func<TypeHierarchySubtypesCallsParams<T>, TypeHierarchyCapability, CancellationToken, Task<Container<TypeHierarchySubtypesCall>?>>
                        handleSubtypesCalls,
                    RegistrationOptionsDelegate<TypeHierarchyRegistrationOptions, TypeHierarchyCapability> registrationOptionsFactory
                )
                {
                    _handlePrepare = handlePrepare;
                    _handleSupertypesCalls = handleSupertypesCalls;
                    _handleSubtypesCalls = handleSubtypesCalls;
                    _registrationOptionsFactory = registrationOptionsFactory;
                }

                protected override Task<Container<TypeHierarchyItem<T>>?> HandlePrepare(TypeHierarchyPrepareParams request, CancellationToken cancellationToken)
                {
                    return _handlePrepare(request, Capability, cancellationToken);
                }

                protected override Task<Container<TypeHierarchySupertypesCall>?> HandleSupertypesCalls(
                    TypeHierarchySupertypesCallsParams<T> request, CancellationToken cancellationToken
                )
                {
                    return _handleSupertypesCalls(request, Capability, cancellationToken);
                }

                protected override Task<Container<TypeHierarchySubtypesCall>?> HandleSubtypesCalls(
                    TypeHierarchySubtypesCallsParams<T> request, CancellationToken cancellationToken
                )
                {
                    return _handleSubtypesCalls(request, Capability, cancellationToken);
                }

                protected internal override TypeHierarchyRegistrationOptions CreateRegistrationOptions(
                    TypeHierarchyCapability capability, ClientCapabilities clientCapabilities
                )
                {
                    return _registrationOptionsFactory(capability, clientCapabilities);
                }
            }

            private class DelegatingPartialTypeHierarchyHandler<T> : PartialTypeHierarchyHandlerBase<T> where T : class?, IHandlerIdentity?
            {
                private readonly Action<TypeHierarchyPrepareParams, IObserver<IEnumerable<TypeHierarchyItem<T>>>, TypeHierarchyCapability, CancellationToken>
                    _handleParams;

                private readonly Action<TypeHierarchySupertypesCallsParams<T>, IObserver<IEnumerable<TypeHierarchySupertypesCall>>, TypeHierarchyCapability,
                        CancellationToken>
                    _handleSupertypes;

                private readonly Action<TypeHierarchySubtypesCallsParams<T>, IObserver<IEnumerable<TypeHierarchySubtypesCall>>, TypeHierarchyCapability,
                        CancellationToken>
                    _handleSubtypes;

                private readonly RegistrationOptionsDelegate<TypeHierarchyRegistrationOptions, TypeHierarchyCapability> _registrationOptionsFactory;

                public DelegatingPartialTypeHierarchyHandler(
                    IProgressManager progressManager,
                    Action<TypeHierarchyPrepareParams, IObserver<IEnumerable<TypeHierarchyItem<T>>>, TypeHierarchyCapability, CancellationToken> handleParams,
                    Action<TypeHierarchySupertypesCallsParams<T>, IObserver<IEnumerable<TypeHierarchySupertypesCall>>, TypeHierarchyCapability, CancellationToken>
                        handleSupertypes,
                    Action<TypeHierarchySubtypesCallsParams<T>, IObserver<IEnumerable<TypeHierarchySubtypesCall>>, TypeHierarchyCapability, CancellationToken>
                        handleSubtypes,
                    RegistrationOptionsDelegate<TypeHierarchyRegistrationOptions, TypeHierarchyCapability> registrationOptionsFactory
                ) : base(progressManager)
                {
                    _handleParams = handleParams;
                    _handleSupertypes = handleSupertypes;
                    _handleSubtypes = handleSubtypes;
                    _registrationOptionsFactory = registrationOptionsFactory;
                }

                protected override void Handle(
                    TypeHierarchyPrepareParams request, IObserver<IEnumerable<TypeHierarchyItem<T>>> results, CancellationToken cancellationToken
                )
                {
                    _handleParams(request, results, Capability, cancellationToken);
                }

                protected override void Handle(
                    TypeHierarchySupertypesCallsParams<T> request, IObserver<IEnumerable<TypeHierarchySupertypesCall>> results, CancellationToken cancellationToken
                )
                {
                    _handleSupertypes(request, results, Capability, cancellationToken);
                }

                protected override void Handle(
                    TypeHierarchySubtypesCallsParams<T> request, IObserver<IEnumerable<TypeHierarchySubtypesCall>> results, CancellationToken cancellationToken
                )
                {
                    _handleSubtypes(request, results, Capability, cancellationToken);
                }

                protected internal override TypeHierarchyRegistrationOptions CreateRegistrationOptions(
                    TypeHierarchyCapability capability, ClientCapabilities clientCapabilities
                )
                {
                    return _registrationOptionsFactory(capability, clientCapabilities);
                }
            }
        }
    }
}
