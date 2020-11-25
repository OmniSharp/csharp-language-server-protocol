using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Serial]
        [Method(TextDocumentNames.DidChange, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(TextDocumentChangeRegistrationOptions)), Capability(typeof(SynchronizationCapability))]
        public partial class DidChangeTextDocumentParams : IRequest
        {
            /// <summary>
            /// The document that did change. The version number points
            /// to the version after all provided content changes have
            /// been applied.
            /// </summary>
            public VersionedTextDocumentIdentifier TextDocument { get; set; } = null!;

            /// <summary>
            /// The actual content changes.
            /// </summary>
            public Container<TextDocumentContentChangeEvent> ContentChanges { get; set; } = null!;
        }

        /// <summary>
        /// Descibe options to be used when registered for text document change events.
        /// </summary>
        [GenerateRegistrationOptions]
        public partial class TextDocumentChangeRegistrationOptions : ITextDocumentRegistrationOptions
        {
            /// <summary>
            /// How documents are synced to the server. See TextDocumentSyncKind.Full
            /// and TextDocumentSyncKindIncremental.
            /// </summary>
            public TextDocumentSyncKind SyncKind { get; set; }
        }

        /// <summary>
        /// An event describing a change to a text document. If range and rangeLength are omitted
        /// the new text is considered to be the full content of the document.
        /// </summary>
        public class TextDocumentContentChangeEvent
        {
            /// <summary>
            /// The range of the document that changed.
            /// </summary>
            [Optional]
            public Range? Range { get; set; }

            /// <summary>
            /// The length of the range that got replaced.
            /// </summary>
            /// <remarks>
            /// <see cref="uint"/> in the LSP spec
            /// </remarks>
            [Optional]
            public int RangeLength { get; set; }

            /// <summary>
            /// The new text of the document.
            /// </summary>
            public string Text { get; set; } = null!;
        }

        public class TextDocumentEdit
        {
            /// <summary>
            /// The text document to change.
            /// </summary>
            public VersionedTextDocumentIdentifier TextDocument { get; set; } = null!;

            /// <summary>
            /// The edits to be applied.
            /// </summary>
            public TextEditContainer Edits { get; set; } = null!;
        }

        [Serial]
        [Method(TextDocumentNames.DidOpen, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(TextDocumentOpenRegistrationOptions)), Capability(typeof(SynchronizationCapability))]
        public partial class DidOpenTextDocumentParams : IRequest
        {
            /// <summary>
            /// The document that was opened.
            /// </summary>
            public TextDocumentItem TextDocument { get; set; } = null!;
        }

        [GenerateRegistrationOptions]
        public partial class TextDocumentOpenRegistrationOptions : ITextDocumentRegistrationOptions
        {
        }

        [Parallel]
        [Method(TextDocumentNames.DidClose, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(TextDocumentCloseRegistrationOptions)), Capability(typeof(SynchronizationCapability))]
        public partial class DidCloseTextDocumentParams : ITextDocumentIdentifierParams, IRequest
        {
            /// <summary>
            /// The document that was closed.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; set; } = null!;
        }

        [GenerateRegistrationOptions]
        public partial class TextDocumentCloseRegistrationOptions : ITextDocumentRegistrationOptions
        {
        }

        [Serial]
        [Method(TextDocumentNames.DidSave, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(TextDocumentSaveRegistrationOptions)), Capability(typeof(SynchronizationCapability))]
        public partial class DidSaveTextDocumentParams : ITextDocumentIdentifierParams, IRequest
        {
            /// <summary>
            /// The document that was saved.
            /// </summary>
            /// <remarks>
            /// TODO: Change to RequiredVersionedTextDocumentIdentifier (or in the future will be VersionedTextDocumentIdentifier)
            /// </remarks>
            public TextDocumentIdentifier TextDocument { get; set; } = null!;

            /// <summary>
            /// Optional the content when saved. Depends on the includeText value
            /// when the save notification was requested.
            /// </summary>
            [Optional]
            public string? Text { get; set; }
        }

        [GenerateRegistrationOptions]
        public partial class TextDocumentSaveRegistrationOptions : ITextDocumentRegistrationOptions
        {
            /// <summary>
            /// The client is supposed to include the content on save.
            /// </summary>
            [Optional]
            public bool IncludeText { get; set; }
        }

        [GenerateRegistrationOptions]
        public partial class TextDocumentSyncRegistrationOptions : ITextDocumentRegistrationOptions
        {
            public TextDocumentSyncRegistrationOptions()
            {
                SyncKind = TextDocumentSyncKind.Full;
            }

            public TextDocumentSyncRegistrationOptions(TextDocumentSyncKind syncKind)
            {
                SyncKind = syncKind;
            }

            /// <summary>
            /// The client is supposed to include the content on save.
            /// </summary>
            [Optional]
            public bool IncludeText { get; set; }

            /// <summary>
            /// How documents are synced to the server. See TextDocumentSyncKind.Full
            /// and TextDocumentSyncKindIncremental.
            /// </summary>
            public TextDocumentSyncKind SyncKind { get; init; }

            public static implicit operator TextDocumentSaveRegistrationOptions(TextDocumentSyncRegistrationOptions options)
            {
                return new() {
                    DocumentSelector = options.DocumentSelector,
                    IncludeText = options.IncludeText
                };
            }

            public static implicit operator TextDocumentChangeRegistrationOptions(TextDocumentSyncRegistrationOptions options)
            {
                return new() {
                    DocumentSelector = options.DocumentSelector,
                    SyncKind = options.SyncKind,
                };
            }

            public static implicit operator TextDocumentOpenRegistrationOptions(TextDocumentSyncRegistrationOptions options)
            {
                return new() {
                    DocumentSelector = options.DocumentSelector,
                };
            }

            public static implicit operator TextDocumentCloseRegistrationOptions(TextDocumentSyncRegistrationOptions options)
            {
                return new() {
                    DocumentSelector = options.DocumentSelector,
                };
            }
        }

        /// <summary>
        /// The parameters send in a will save text document notification.
        /// </summary>
        [Parallel]
        [Method(TextDocumentNames.WillSave, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(TextDocumenWillSaveRegistrationOptions)), Capability(typeof(SynchronizationCapability))]
        public partial class WillSaveTextDocumentParams : IRequest
        {
            /// <summary>
            /// The document that will be saved.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; set; } = null!;

            /// <summary>
            /// The 'TextDocumentSaveReason'.
            /// </summary>
            public TextDocumentSaveReason Reason { get; set; }
        }

        [GenerateRegistrationOptions]
        public partial class TextDocumenWillSaveRegistrationOptions : ITextDocumentRegistrationOptions
        {
        }

        /// <summary>
        /// The parameters send in a will save text document notification.
        /// </summary>
        [Parallel]
        [Method(TextDocumentNames.WillSaveWaitUntil, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(TextDocumentWillSaveWaitUntilRegistrationOptions)), Capability(typeof(SynchronizationCapability))]
        public partial class WillSaveWaitUntilTextDocumentParams : IRequest<TextEditContainer?>
        {
            /// <summary>
            /// The document that will be saved.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; set; } = null!;

            /// <summary>
            /// The 'TextDocumentSaveReason'.
            /// </summary>
            public TextDocumentSaveReason Reason { get; set; }
        }

        [GenerateRegistrationOptions]
        public partial class TextDocumentWillSaveWaitUntilRegistrationOptions : ITextDocumentRegistrationOptions
        {
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.Synchronization))]
        public class SynchronizationCapability : DynamicCapability,
                                                 ConnectedCapability<IDidChangeTextDocumentHandler>,
                                                 ConnectedCapability<IDidCloseTextDocumentHandler>,
                                                 ConnectedCapability<IDidOpenTextDocumentHandler>,
                                                 ConnectedCapability<IDidSaveTextDocumentHandler>,
                                                 ConnectedCapability<IWillSaveTextDocumentHandler>,
                                                 ConnectedCapability<IWillSaveWaitUntilTextDocumentHandler>
        {
            /// <summary>
            /// The client supports sending will save notifications.
            /// </summary>
            [Optional]
            public bool WillSave { get; set; }

            /// <summary>
            /// The client supports sending a will save request and
            /// waits for a response providing text edits which will
            /// be applied to the document before it is saved.
            /// </summary>
            [Optional]
            public bool WillSaveWaitUntil { get; set; }

            /// <summary>
            /// The client supports did save notifications.
            /// </summary>
            [Optional]
            public bool DidSave { get; set; }
        }
    }

    namespace Document
    {
        public interface ITextDocumentSyncHandler : IDidChangeTextDocumentHandler, IDidOpenTextDocumentHandler,
                                                    IDidCloseTextDocumentHandler, IDidSaveTextDocumentHandler, ITextDocumentIdentifier
        {
        }

        public abstract class TextDocumentSyncHandlerBase : ITextDocumentSyncHandler
        {

            public TextDocumentSyncHandlerBase()
            {
            }

            public abstract TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri);
            public abstract Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken);
            public abstract Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken);
            public abstract Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken);
            public abstract Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken);

            private TextDocumentSyncRegistrationOptions? _registrationOptions;

            protected TextDocumentSyncRegistrationOptions RegistrationOptions
            {
                get => _registrationOptions!;
                private set => _registrationOptions = value;
            }

            protected SynchronizationCapability Capability { get; private set; } = default!;

            protected abstract TextDocumentSyncRegistrationOptions CreateRegistrationOptions(SynchronizationCapability capability);
            private TextDocumentSyncRegistrationOptions AssignRegistrationOptions(SynchronizationCapability capability)
                {
                Capability = capability;
                return CreateRegistrationOptions(capability);
            }

            TextDocumentChangeRegistrationOptions IRegistration<TextDocumentChangeRegistrationOptions, SynchronizationCapability>.GetRegistrationOptions(
                SynchronizationCapability capability
            ) => _registrationOptions ?? AssignRegistrationOptions(capability);

            TextDocumentOpenRegistrationOptions IRegistration<TextDocumentOpenRegistrationOptions, SynchronizationCapability>.GetRegistrationOptions(
                SynchronizationCapability capability
            ) => _registrationOptions ?? AssignRegistrationOptions(capability);

            TextDocumentCloseRegistrationOptions IRegistration<TextDocumentCloseRegistrationOptions, SynchronizationCapability>.GetRegistrationOptions(
                SynchronizationCapability capability
            ) => _registrationOptions ?? AssignRegistrationOptions(capability);

            TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions, SynchronizationCapability>.GetRegistrationOptions(
                SynchronizationCapability capability
            ) => _registrationOptions ?? AssignRegistrationOptions(capability);
        }


        public static class TextDocumentSyncExtensions
        {
            public static ILanguageServerRegistry OnTextDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                Func<DidOpenTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onOpenHandler,
                Func<DidCloseTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onCloseHandler,
                Func<DidChangeTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onChangeHandler,
                Func<DidSaveTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onSaveHandler,
                Func<SynchronizationCapability, TextDocumentSyncRegistrationOptions>? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        onOpenHandler,
                        onCloseHandler,
                        onChangeHandler,
                        onSaveHandler,
                        RegistrationAdapter<SynchronizationCapability>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnTextDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                Func<DidOpenTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onOpenHandler,
                Func<DidCloseTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onCloseHandler,
                Func<DidChangeTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onChangeHandler,
                Func<DidSaveTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onSaveHandler,
                TextDocumentSyncRegistrationOptions? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<SynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<SynchronizationCapability>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnTextDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                Action<DidOpenTextDocumentParams, SynchronizationCapability, CancellationToken> onOpenHandler,
                Action<DidCloseTextDocumentParams, SynchronizationCapability, CancellationToken> onCloseHandler,
                Action<DidChangeTextDocumentParams, SynchronizationCapability, CancellationToken> onChangeHandler,
                Action<DidSaveTextDocumentParams, SynchronizationCapability, CancellationToken> onSaveHandler,
                Func<SynchronizationCapability, TextDocumentSyncRegistrationOptions>? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<SynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onSaveHandler),
                        registrationOptions
                    )
                );
            }

            public static ILanguageServerRegistry OnTextDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                Action<DidOpenTextDocumentParams, SynchronizationCapability, CancellationToken> onOpenHandler,
                Action<DidCloseTextDocumentParams, SynchronizationCapability, CancellationToken> onCloseHandler,
                Action<DidChangeTextDocumentParams, SynchronizationCapability, CancellationToken> onChangeHandler,
                Action<DidSaveTextDocumentParams, SynchronizationCapability, CancellationToken> onSaveHandler,
                TextDocumentSyncRegistrationOptions? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<SynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<SynchronizationCapability>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnTextDocumentSync(
                this ILanguageServerRegistry registry,
                TextDocumentSyncKind kind,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                Action<DidOpenTextDocumentParams, SynchronizationCapability> onOpenHandler,
                Action<DidCloseTextDocumentParams, SynchronizationCapability> onCloseHandler,
                Action<DidChangeTextDocumentParams, SynchronizationCapability> onChangeHandler,
                Action<DidSaveTextDocumentParams, SynchronizationCapability> onSaveHandler,
                Func<SynchronizationCapability, TextDocumentSyncRegistrationOptions>? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<SynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<SynchronizationCapability>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnTextDocumentSync(
                this ILanguageServerRegistry registry,
                TextDocumentSyncKind kind,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                Action<DidOpenTextDocumentParams, SynchronizationCapability> onOpenHandler,
                Action<DidCloseTextDocumentParams, SynchronizationCapability> onCloseHandler,
                Action<DidChangeTextDocumentParams, SynchronizationCapability> onChangeHandler,
                Action<DidSaveTextDocumentParams, SynchronizationCapability> onSaveHandler,
                TextDocumentSyncRegistrationOptions? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<SynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<SynchronizationCapability>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnTextDocumentSync(
                this ILanguageServerRegistry registry,
                TextDocumentSyncKind kind,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                Func<DidOpenTextDocumentParams, CancellationToken, Task> onOpenHandler,
                Func<DidCloseTextDocumentParams, CancellationToken, Task> onCloseHandler,
                Func<DidChangeTextDocumentParams, CancellationToken, Task> onChangeHandler,
                Func<DidSaveTextDocumentParams, CancellationToken, Task> onSaveHandler,
                Func<SynchronizationCapability, TextDocumentSyncRegistrationOptions>? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<SynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<SynchronizationCapability>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnTextDocumentSync(
                this ILanguageServerRegistry registry,
                TextDocumentSyncKind kind,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                Func<DidOpenTextDocumentParams, CancellationToken, Task> onOpenHandler,
                Func<DidCloseTextDocumentParams, CancellationToken, Task> onCloseHandler,
                Func<DidChangeTextDocumentParams, CancellationToken, Task> onChangeHandler,
                Func<DidSaveTextDocumentParams, CancellationToken, Task> onSaveHandler,
                TextDocumentSyncRegistrationOptions? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<SynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<SynchronizationCapability>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnTextDocumentSync(
                this ILanguageServerRegistry registry,
                TextDocumentSyncKind kind,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                Action<DidOpenTextDocumentParams, CancellationToken> onOpenHandler,
                Action<DidCloseTextDocumentParams, CancellationToken> onCloseHandler,
                Action<DidChangeTextDocumentParams, CancellationToken> onChangeHandler,
                Action<DidSaveTextDocumentParams, CancellationToken> onSaveHandler,
                Func<SynchronizationCapability, TextDocumentSyncRegistrationOptions>? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<SynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<SynchronizationCapability>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnTextDocumentSync(
                this ILanguageServerRegistry registry,
                TextDocumentSyncKind kind,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                Action<DidOpenTextDocumentParams, CancellationToken> onOpenHandler,
                Action<DidCloseTextDocumentParams, CancellationToken> onCloseHandler,
                Action<DidChangeTextDocumentParams, CancellationToken> onChangeHandler,
                Action<DidSaveTextDocumentParams, CancellationToken> onSaveHandler,
                TextDocumentSyncRegistrationOptions? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<SynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<SynchronizationCapability>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnTextDocumentSync(
                this ILanguageServerRegistry registry,
                TextDocumentSyncKind kind,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                Func<DidOpenTextDocumentParams, Task> onOpenHandler,
                Func<DidCloseTextDocumentParams, Task> onCloseHandler,
                Func<DidChangeTextDocumentParams, Task> onChangeHandler,
                Func<DidSaveTextDocumentParams, Task> onSaveHandler,
                Func<SynchronizationCapability, TextDocumentSyncRegistrationOptions>? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<SynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<SynchronizationCapability>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnTextDocumentSync(
                this ILanguageServerRegistry registry,
                TextDocumentSyncKind kind,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                Func<DidOpenTextDocumentParams, Task> onOpenHandler,
                Func<DidCloseTextDocumentParams, Task> onCloseHandler,
                Func<DidChangeTextDocumentParams, Task> onChangeHandler,
                Func<DidSaveTextDocumentParams, Task> onSaveHandler,
                TextDocumentSyncRegistrationOptions? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<SynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<SynchronizationCapability>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnTextDocumentSync(
                this ILanguageServerRegistry registry,
                TextDocumentSyncKind kind,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                Action<DidOpenTextDocumentParams> onOpenHandler,
                Action<DidCloseTextDocumentParams> onCloseHandler,
                Action<DidChangeTextDocumentParams> onChangeHandler,
                Action<DidSaveTextDocumentParams> onSaveHandler,
                Func<SynchronizationCapability, TextDocumentSyncRegistrationOptions>? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<SynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<SynchronizationCapability>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnTextDocumentSync(
                this ILanguageServerRegistry registry,
                TextDocumentSyncKind kind,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                Action<DidOpenTextDocumentParams> onOpenHandler,
                Action<DidCloseTextDocumentParams> onCloseHandler,
                Action<DidChangeTextDocumentParams> onChangeHandler,
                Action<DidSaveTextDocumentParams> onSaveHandler,
                TextDocumentSyncRegistrationOptions? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<SynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<SynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<SynchronizationCapability>.Adapt(registrationOptions)
                    )
                );
            }

            private class DelegatingHandler : TextDocumentSyncHandlerBase
            {
                private readonly Func<DidOpenTextDocumentParams, SynchronizationCapability, CancellationToken, Task> _onOpenHandler;
                private readonly Func<DidCloseTextDocumentParams, SynchronizationCapability, CancellationToken, Task> _onCloseHandler;
                private readonly Func<DidChangeTextDocumentParams, SynchronizationCapability, CancellationToken, Task> _onChangeHandler;
                private readonly Func<DidSaveTextDocumentParams, SynchronizationCapability, CancellationToken, Task> _onSaveHandler;
                private readonly Func<SynchronizationCapability, TextDocumentSyncRegistrationOptions> _registrationOptionsFactory;
                private readonly Func<DocumentUri, TextDocumentAttributes> _getTextDocumentAttributes;

                public DelegatingHandler(
                    Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                    Func<DidOpenTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onOpenHandler,
                    Func<DidCloseTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onCloseHandler,
                    Func<DidChangeTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onChangeHandler,
                    Func<DidSaveTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onSaveHandler,
                    Func<SynchronizationCapability, TextDocumentSyncRegistrationOptions> registrationOptionsFactory
                )
                {
                    _onOpenHandler = onOpenHandler;
                    _onSaveHandler = onSaveHandler;
                    _registrationOptionsFactory = registrationOptionsFactory;
                    _onChangeHandler = onChangeHandler;
                    _onCloseHandler = onCloseHandler;
                    _getTextDocumentAttributes = getTextDocumentAttributes;
                }

                public override async Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
                {
                    await _onOpenHandler.Invoke(request, Capability, cancellationToken).ConfigureAwait(false);
                    return Unit.Value;
                }

                public override async Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
                {
                    await _onChangeHandler.Invoke(request, Capability, cancellationToken).ConfigureAwait(false);
                    return Unit.Value;
                }

                public override async Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
                {
                    await _onSaveHandler.Invoke(request, Capability, cancellationToken).ConfigureAwait(false);
                    return Unit.Value;
                }

                public override async Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
                {
                    await _onCloseHandler.Invoke(request, Capability, cancellationToken).ConfigureAwait(false);
                    return Unit.Value;
                }

                protected override TextDocumentSyncRegistrationOptions CreateRegistrationOptions(SynchronizationCapability capability) => _registrationOptionsFactory(capability);

                public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri) => _getTextDocumentAttributes.Invoke(uri);
            }
        }
    }
}
