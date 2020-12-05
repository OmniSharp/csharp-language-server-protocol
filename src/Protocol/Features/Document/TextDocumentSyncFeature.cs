using System;
using System.Linq;
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
        public partial record DidChangeTextDocumentParams : IRequest
        {
            /// <summary>
            /// The document that did change. The version number points
            /// to the version after all provided content changes have
            /// been applied.
            /// </summary>
            public OptionalVersionedTextDocumentIdentifier TextDocument { get; init; }

            /// <summary>
            /// The actual content changes.
            /// </summary>
            public Container<TextDocumentContentChangeEvent> ContentChanges { get; init; }
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
        public record TextDocumentContentChangeEvent
        {
            /// <summary>
            /// The range of the document that changed.
            /// </summary>
            [Optional]
            public Range? Range { get; init; }

            /// <summary>
            /// The length of the range that got replaced.
            /// </summary>
            /// <remarks>
            /// <see cref="uint"/> in the LSP spec
            /// </remarks>
            [Optional]
            public int RangeLength { get; init; }

            /// <summary>
            /// The new text of the document.
            /// </summary>
            public string Text { get; init; }
        }

        public record TextDocumentEdit
        {
            /// <summary>
            /// The text document to change.
            /// </summary>
            public OptionalVersionedTextDocumentIdentifier TextDocument { get; init; }

            /// <summary>
            /// The edits to be applied.
            /// </summary>
            /// <remarks>
            /// This can contain both <see cref="TextEdit"/> and <see cref="AnnotatedTextEdit"/>
            /// </remarks>
            public TextEditContainer Edits { get; init; }
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

        [GenerateRegistrationOptions(nameof(ServerCapabilities.TextDocumentSync))]
        [RegistrationOptionsConverter(typeof(Converter))]
        public partial class TextDocumentSyncRegistrationOptions : ITextDocumentRegistrationOptions
        {
            public TextDocumentSyncRegistrationOptions()
            {
                Change = TextDocumentSyncKind.Full;
            }

            public TextDocumentSyncRegistrationOptions(TextDocumentSyncKind syncKind)
            {
                Change = syncKind;
            }

            /// <summary>
            /// Save notifications are sent to the server.
            /// </summary>
            [Optional]
            public BooleanOr<SaveOptions>? Save { get; set; }

            /// <summary>
            /// How documents are synced to the server. See TextDocumentSyncKind.Full
            /// and TextDocumentSyncKindIncremental.
            /// </summary>
            public TextDocumentSyncKind Change { get; set; }

            class Converter : RegistrationOptionsConverterBase<TextDocumentSyncRegistrationOptions, StaticOptions>
            {
                private readonly IHandlersManager _handlersManager;

                public Converter(IHandlersManager handlersManager)
                {
                    _handlersManager = handlersManager;
                }

                public override StaticOptions Convert(TextDocumentSyncRegistrationOptions source)
                {
                    return new() {
                        OpenClose = _handlersManager.Descriptors.Any(
                            z => z.HandlerType == typeof(IDidOpenTextDocumentHandler) || z.HandlerType == typeof(IDidCloseTextDocumentHandler)
                        ),
                        Change = source.Change,
                        WillSave = _handlersManager.Descriptors.Any(z => z.HandlerType == typeof(IWillSaveTextDocumentHandler)),
                        WillSaveWaitUntil = _handlersManager.Descriptors.Any(z => z.HandlerType == typeof(IWillSaveWaitUntilTextDocumentHandler)),
                        Save = new SaveOptions { IncludeText = source.Save?.Value?.IncludeText == true }
                    };
                }
            }

            public partial class StaticOptions : ITextDocumentSyncOptions
            {
                /// <summary>
                /// Open and close notifications are sent to the server.
                /// </summary>
                [Optional]
                public bool OpenClose { get; set; }

                /// <summary>
                /// Will save notifications are sent to the server.
                /// </summary>
                [Optional]
                public bool WillSave { get; set; }

                /// <summary>
                /// Will save wait until requests are sent to the server.
                /// </summary>
                [Optional]
                public bool WillSaveWaitUntil { get; set; }
            }

            public static implicit operator TextDocumentSaveRegistrationOptions(TextDocumentSyncRegistrationOptions options)
            {
                return new() {
                    DocumentSelector = options.DocumentSelector,
                    IncludeText = options.Save?.Value?.IncludeText == true
                };
            }

            public static implicit operator TextDocumentChangeRegistrationOptions(TextDocumentSyncRegistrationOptions options)
            {
                return new() {
                    DocumentSelector = options.DocumentSelector,
                    SyncKind = options.Change,
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
            public abstract TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri);
            public abstract Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken);
            public abstract Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken);
            public abstract Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken);
            public abstract Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken);

            private TextDocumentSyncRegistrationOptions? _registrationOptions;

            protected TextDocumentSyncRegistrationOptions RegistrationOptions => _registrationOptions!;

            protected SynchronizationCapability Capability { get; private set; } = default!;

            protected abstract TextDocumentSyncRegistrationOptions CreateRegistrationOptions(SynchronizationCapability capability, ClientCapabilities clientCapabilities);

            private TextDocumentSyncRegistrationOptions AssignRegistrationOptions(SynchronizationCapability capability, ClientCapabilities clientCapabilities)
            {
                Capability = capability;
                if (_registrationOptions is { }) return _registrationOptions;
                return _registrationOptions = CreateRegistrationOptions(capability, clientCapabilities);
            }

            TextDocumentChangeRegistrationOptions IRegistration<TextDocumentChangeRegistrationOptions, SynchronizationCapability>.GetRegistrationOptions(
                SynchronizationCapability capability, ClientCapabilities clientCapabilities
            ) => _registrationOptions ?? AssignRegistrationOptions(capability, clientCapabilities);

            TextDocumentOpenRegistrationOptions IRegistration<TextDocumentOpenRegistrationOptions, SynchronizationCapability>.GetRegistrationOptions(
                SynchronizationCapability capability, ClientCapabilities clientCapabilities
            ) => _registrationOptions ?? AssignRegistrationOptions(capability, clientCapabilities);

            TextDocumentCloseRegistrationOptions IRegistration<TextDocumentCloseRegistrationOptions, SynchronizationCapability>.GetRegistrationOptions(
                SynchronizationCapability capability, ClientCapabilities clientCapabilities
            ) => _registrationOptions ?? AssignRegistrationOptions(capability, clientCapabilities);

            TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions, SynchronizationCapability>.GetRegistrationOptions(
                SynchronizationCapability capability, ClientCapabilities clientCapabilities
            ) => _registrationOptions ?? AssignRegistrationOptions(capability, clientCapabilities);
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
                RegistrationOptionsDelegate<TextDocumentSyncRegistrationOptions, SynchronizationCapability>? registrationOptions
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
                RegistrationOptionsDelegate<TextDocumentSyncRegistrationOptions, SynchronizationCapability>? registrationOptions
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
                RegistrationOptionsDelegate<TextDocumentSyncRegistrationOptions, SynchronizationCapability>? registrationOptions
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
                RegistrationOptionsDelegate<TextDocumentSyncRegistrationOptions, SynchronizationCapability>? registrationOptions
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
                RegistrationOptionsDelegate<TextDocumentSyncRegistrationOptions, SynchronizationCapability>? registrationOptions
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
                RegistrationOptionsDelegate<TextDocumentSyncRegistrationOptions, SynchronizationCapability>? registrationOptions
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
                RegistrationOptionsDelegate<TextDocumentSyncRegistrationOptions, SynchronizationCapability>? registrationOptions
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
                private readonly RegistrationOptionsDelegate<TextDocumentSyncRegistrationOptions, SynchronizationCapability> _registrationOptionsFactory;
                private readonly Func<DocumentUri, TextDocumentAttributes> _getTextDocumentAttributes;

                public DelegatingHandler(
                    Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                    Func<DidOpenTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onOpenHandler,
                    Func<DidCloseTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onCloseHandler,
                    Func<DidChangeTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onChangeHandler,
                    Func<DidSaveTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onSaveHandler,
                    RegistrationOptionsDelegate<TextDocumentSyncRegistrationOptions, SynchronizationCapability> registrationOptionsFactory
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

                protected override TextDocumentSyncRegistrationOptions CreateRegistrationOptions(SynchronizationCapability capability, ClientCapabilities clientCapabilities) =>
                    _registrationOptionsFactory(capability, clientCapabilities);

                public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri) => _getTextDocumentAttributes.Invoke(uri);
            }
        }
    }
}
