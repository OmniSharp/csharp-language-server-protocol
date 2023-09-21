using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Minimatch;
using Newtonsoft.Json;
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
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(TextDocumentChangeRegistrationOptions))]
        [Capability(typeof(TextSynchronizationCapability))]
        public partial record DidChangeTextDocumentParams : IRequest
        {
            /// <summary>
            /// The document that did change. The version number points
            /// to the version after all provided content changes have
            /// been applied.
            /// </summary>
            public OptionalVersionedTextDocumentIdentifier TextDocument { get; init; } = null!;

            /// <summary>
            /// The actual content changes.
            /// </summary>
            public Container<TextDocumentContentChangeEvent> ContentChanges { get; init; } = null!;
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
        /// An event describing a change to a text document. If only a text is provided
        /// it is considered to be the full content of the document.
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
            /// <see cref="uint" /> in the LSP spec
            /// </remarks>
            [Optional]
            public int RangeLength { get; init; }

            /// <summary>
            /// The new text of the document.
            /// </summary>
            public string Text { get; init; } = null!;
        }

        public record TextDocumentEdit
        {
            /// <summary>
            /// The text document to change.
            /// </summary>
            public OptionalVersionedTextDocumentIdentifier TextDocument { get; init; } = null!;

            /// <summary>
            /// The edits to be applied.
            ///
            /// @since 3.16.0 - support for AnnotatedTextEdit. This is guarded by the
	        /// client capability `workspace.workspaceEdit.changeAnnotationSupport`
            /// </summary>
            /// <remarks>
            /// This can contain both <see cref="TextEdit" /> and <see cref="AnnotatedTextEdit" />
            /// </remarks>
            public TextEditContainer Edits { get; init; } = null!;
        }

        [Serial]
        [Method(TextDocumentNames.DidOpen, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(TextDocumentOpenRegistrationOptions))]
        [Capability(typeof(TextSynchronizationCapability))]
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
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(TextDocumentCloseRegistrationOptions))]
        [Capability(typeof(TextSynchronizationCapability))]
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
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(TextDocumentSaveRegistrationOptions))]
        [Capability(typeof(TextSynchronizationCapability))]
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

        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public class TextDocumentFilter : IEquatable<TextDocumentFilter>
        {
            public static TextDocumentFilter ForPattern(string wildcard) => new TextDocumentFilter { Pattern = wildcard };

            public static TextDocumentFilter ForLanguage(string language) => new TextDocumentFilter { Language = language };

            public static TextDocumentFilter ForScheme(string scheme) => new TextDocumentFilter { Scheme = scheme };

            /// <summary>
            /// A language id, like `typescript`.
            /// </summary>
            [Optional]
            public string? Language { get; init; }

            /// <summary>
            /// does the document filter contains a language
            /// </summary>
            [JsonIgnore]
            public bool HasLanguage => Language != null;

            /// <summary>
            /// A Uri [scheme](#Uri.scheme), like `file` or `untitled`.
            /// </summary>
            [Optional]
            public string? Scheme { get; init; }

            /// <summary>
            /// does the document filter contains a scheme
            /// </summary>
            [JsonIgnore]
            public bool HasScheme => Scheme != null;

            /// <summary>
            /// A glob pattern, like `*.{ts,js}`.
            ///
            /// Glob patterns can have the following syntax:
            /// - `*` to match one or more characters in a path segment
            /// - `?` to match on one character in a path segment
            /// - `**` to match any number of path segments, including none
            /// - `{}` to group sub patterns into an OR expression. (e.g. `**​/*.{ts,js}`
            ///   matches all TypeScript and JavaScript files)
            /// - `[]` to declare a range of characters to match in a path segment
            ///   (e.g., `example.[0-9]` to match on `example.0`, `example.1`, …)
            /// - `[!...]` to negate a range of characters to match in a path segment
            ///   (e.g., `example.[!0-9]` to match on `example.a`, `example.b`, but
            ///   not `example.0`)
            /// </summary>
            [Optional]
            public string? Pattern
            {
                get => _pattern;
                init
                {
                    _pattern = value;
                    _minimatcher = new Minimatcher(value!, new Options { MatchBase = true });
                }
            }

            /// <summary>
            /// does the document filter contains a paattern
            /// </summary>
            [JsonIgnore]
            public bool HasPattern => Pattern != null;

            private string? _pattern;
            private Minimatcher? _minimatcher;

            public static explicit operator string(TextDocumentFilter textDocumentFilter)
            {
                var items = new List<string>();
                if (textDocumentFilter.HasLanguage)
                {
                    items.Add(textDocumentFilter.Language!);
                }

                if (textDocumentFilter.HasScheme)
                {
                    items.Add(textDocumentFilter.Scheme!);
                }

                if (textDocumentFilter.HasPattern)
                {
                    items.Add(textDocumentFilter.Pattern!);
                }

                return $"[{string.Join(", ", items)}]";
            }

            public bool IsMatch(TextDocumentAttributes attributes)
            {
                if (HasLanguage && HasPattern && HasScheme)
                {
                    return Language == attributes.LanguageId && Scheme == attributes.Scheme && _minimatcher?.IsMatch(attributes.Uri.ToString()) == true;
                }

                if (HasLanguage && HasPattern)
                {
                    return Language == attributes.LanguageId && _minimatcher?.IsMatch(attributes.Uri.ToString()) == true;
                }

                if (HasLanguage && HasScheme)
                {
                    return Language == attributes.LanguageId && Scheme == attributes.Scheme;
                }

                if (HasPattern && HasScheme)
                {
                    return Scheme == attributes.Scheme && _minimatcher?.IsMatch(attributes.Uri.ToString()) == true;
                }

                if (HasLanguage)
                {
                    return Language == attributes.LanguageId;
                }

                if (HasScheme)
                {
                    return Scheme == attributes.Scheme;
                }

                if (HasPattern)
                {
                    return _minimatcher?.IsMatch(attributes.Uri.ToString()) == true;
                }

                return false;
            }

            public bool Equals(TextDocumentFilter? other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return _pattern == other._pattern && Language == other.Language && Scheme == other.Scheme;
            }

            public override bool Equals(object? obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((TextDocumentFilter)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = _pattern != null ? _pattern.GetHashCode() : 0;
                    hashCode = ( hashCode * 397 ) ^ ( Language != null ? Language.GetHashCode() : 0 );
                    hashCode = ( hashCode * 397 ) ^ ( Scheme != null ? Scheme.GetHashCode() : 0 );
                    return hashCode;
                }
            }

            public static bool operator ==(TextDocumentFilter left, TextDocumentFilter right) => Equals(left, right);

            public static bool operator !=(TextDocumentFilter left, TextDocumentFilter right) => !Equals(left, right);

            private string DebuggerDisplay => (string)this;

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;
        }

        /// <summary>
        /// A collection of document filters used to identify valid documents
        /// </summary>
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public class TextDocumentSelector : ContainerBase<TextDocumentFilter>
        {
            public TextDocumentSelector() : this(Enumerable.Empty<TextDocumentFilter>())
            {
            }

            public TextDocumentSelector(IEnumerable<TextDocumentFilter> items) : base(items)
            {
            }

            public TextDocumentSelector(params TextDocumentFilter[] items) : base(items)
            {
            }

            public static implicit operator TextDocumentSelector(TextDocumentFilter[] items) => new TextDocumentSelector(items);

            public static implicit operator TextDocumentSelector(Collection<TextDocumentFilter> items) => new TextDocumentSelector(items);

            public static implicit operator TextDocumentSelector(List<TextDocumentFilter> items) => new TextDocumentSelector(items);

            public static implicit operator string(TextDocumentSelector? documentSelector) =>
                documentSelector is not null ? string.Join(", ", documentSelector.Select(x => (string)x)) : string.Empty;

            public bool IsMatch(TextDocumentAttributes attributes) => this.Any(z => z.IsMatch(attributes));

            public override string ToString() => this;

            public static TextDocumentSelector ForPattern(params string[] wildcards) => new TextDocumentSelector(wildcards.Select(TextDocumentFilter.ForPattern));

            public static TextDocumentSelector ForLanguage(params string[] languages) => new TextDocumentSelector(languages.Select(TextDocumentFilter.ForLanguage));

            public static TextDocumentSelector ForScheme(params string[] schemes) => new TextDocumentSelector(schemes.Select(TextDocumentFilter.ForScheme));

            private string DebuggerDisplay => this;
        }

        public interface ITextDocumentSyncOptions
        {
            [Optional] bool OpenClose { get; set; }
            [Optional] TextDocumentSyncKind Change { get; set; }
            [Optional] bool WillSave { get; set; }
            [Optional] bool WillSaveWaitUntil { get; set; }
            [Optional] BooleanOr<SaveOptions> Save { get; set; }
        }

        public interface ITextDocumentRegistrationOptions : IRegistrationOptions
        {
            /// <summary>
            /// A document selector to identify the scope of the registration. If set to
            /// null the document selector provided on the client side will be used.
            /// </summary>
            TextDocumentSelector? DocumentSelector { get; set; }
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

            private class Converter : RegistrationOptionsConverterBase<TextDocumentSyncRegistrationOptions, StaticOptions>
            {
                private readonly IHandlersManager _handlersManager;

                public Converter(IHandlersManager handlersManager)
                {
                    _handlersManager = handlersManager;
                }

                public override StaticOptions Convert(TextDocumentSyncRegistrationOptions source)
                {
                    return new()
                    {
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
                return new()
                {
                    DocumentSelector = options.DocumentSelector,
                    IncludeText = options.Save?.Value?.IncludeText == true
                };
            }

            public static implicit operator TextDocumentChangeRegistrationOptions(TextDocumentSyncRegistrationOptions options)
            {
                return new()
                {
                    DocumentSelector = options.DocumentSelector,
                    SyncKind = options.Change,
                };
            }

            public static implicit operator TextDocumentOpenRegistrationOptions(TextDocumentSyncRegistrationOptions options)
            {
                return new()
                {
                    DocumentSelector = options.DocumentSelector,
                };
            }

            public static implicit operator TextDocumentCloseRegistrationOptions(TextDocumentSyncRegistrationOptions options)
            {
                return new()
                {
                    DocumentSelector = options.DocumentSelector,
                };
            }
        }

        /// <summary>
        /// The parameters send in a will save text document notification.
        /// </summary>
        [Parallel]
        [Method(TextDocumentNames.WillSave, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(TextDocumenWillSaveRegistrationOptions))]
        [Capability(typeof(TextSynchronizationCapability))]
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
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(TextDocumentWillSaveWaitUntilRegistrationOptions))]
        [Capability(typeof(TextSynchronizationCapability))]
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
        public class TextSynchronizationCapability : DynamicCapability

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

            protected TextDocumentSyncRegistrationOptions RegistrationOptions { get; private set; }

            protected ClientCapabilities ClientCapabilities { get; private set; }

            protected TextSynchronizationCapability Capability { get; private set; } = default!;

            protected abstract TextDocumentSyncRegistrationOptions CreateRegistrationOptions(
                TextSynchronizationCapability capability, ClientCapabilities clientCapabilities
            );

            private TextDocumentSyncRegistrationOptions AssignRegistrationOptions(
                TextSynchronizationCapability capability, ClientCapabilities clientCapabilities
            )
            {
                Capability = capability;
                if (RegistrationOptions is { }) return RegistrationOptions;
                ClientCapabilities = clientCapabilities;
                return RegistrationOptions = CreateRegistrationOptions(capability, clientCapabilities);
            }

            TextDocumentChangeRegistrationOptions IRegistration<TextDocumentChangeRegistrationOptions, TextSynchronizationCapability>.GetRegistrationOptions(
                TextSynchronizationCapability capability, ClientCapabilities clientCapabilities
            )
            {
                return RegistrationOptions ?? AssignRegistrationOptions(capability, clientCapabilities);
            }

            TextDocumentOpenRegistrationOptions IRegistration<TextDocumentOpenRegistrationOptions, TextSynchronizationCapability>.GetRegistrationOptions(
                TextSynchronizationCapability capability, ClientCapabilities clientCapabilities
            )
            {
                return RegistrationOptions ?? AssignRegistrationOptions(capability, clientCapabilities);
            }

            TextDocumentCloseRegistrationOptions IRegistration<TextDocumentCloseRegistrationOptions, TextSynchronizationCapability>.GetRegistrationOptions(
                TextSynchronizationCapability capability, ClientCapabilities clientCapabilities
            )
            {
                return RegistrationOptions ?? AssignRegistrationOptions(capability, clientCapabilities);
            }

            TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions, TextSynchronizationCapability>.GetRegistrationOptions(
                TextSynchronizationCapability capability, ClientCapabilities clientCapabilities
            )
            {
                return RegistrationOptions ?? AssignRegistrationOptions(capability, clientCapabilities);
            }
        }


        public static class TextDocumentSyncExtensions
        {
            public static ILanguageServerRegistry OnTextDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                Func<DidOpenTextDocumentParams, TextSynchronizationCapability, CancellationToken, Task> onOpenHandler,
                Func<DidCloseTextDocumentParams, TextSynchronizationCapability, CancellationToken, Task> onCloseHandler,
                Func<DidChangeTextDocumentParams, TextSynchronizationCapability, CancellationToken, Task> onChangeHandler,
                Func<DidSaveTextDocumentParams, TextSynchronizationCapability, CancellationToken, Task> onSaveHandler,
                RegistrationOptionsDelegate<TextDocumentSyncRegistrationOptions, TextSynchronizationCapability>? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        onOpenHandler,
                        onCloseHandler,
                        onChangeHandler,
                        onSaveHandler,
                        RegistrationAdapter<TextSynchronizationCapability>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnTextDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                Func<DidOpenTextDocumentParams, TextSynchronizationCapability, CancellationToken, Task> onOpenHandler,
                Func<DidCloseTextDocumentParams, TextSynchronizationCapability, CancellationToken, Task> onCloseHandler,
                Func<DidChangeTextDocumentParams, TextSynchronizationCapability, CancellationToken, Task> onChangeHandler,
                Func<DidSaveTextDocumentParams, TextSynchronizationCapability, CancellationToken, Task> onSaveHandler,
                TextDocumentSyncRegistrationOptions? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<TextSynchronizationCapability>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnTextDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                Action<DidOpenTextDocumentParams, TextSynchronizationCapability, CancellationToken> onOpenHandler,
                Action<DidCloseTextDocumentParams, TextSynchronizationCapability, CancellationToken> onCloseHandler,
                Action<DidChangeTextDocumentParams, TextSynchronizationCapability, CancellationToken> onChangeHandler,
                Action<DidSaveTextDocumentParams, TextSynchronizationCapability, CancellationToken> onSaveHandler,
                RegistrationOptionsDelegate<TextDocumentSyncRegistrationOptions, TextSynchronizationCapability>? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<TextSynchronizationCapability>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnTextDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                Action<DidOpenTextDocumentParams, TextSynchronizationCapability, CancellationToken> onOpenHandler,
                Action<DidCloseTextDocumentParams, TextSynchronizationCapability, CancellationToken> onCloseHandler,
                Action<DidChangeTextDocumentParams, TextSynchronizationCapability, CancellationToken> onChangeHandler,
                Action<DidSaveTextDocumentParams, TextSynchronizationCapability, CancellationToken> onSaveHandler,
                TextDocumentSyncRegistrationOptions? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<TextSynchronizationCapability>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnTextDocumentSync(
                this ILanguageServerRegistry registry,
                TextDocumentSyncKind kind,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                Action<DidOpenTextDocumentParams, TextSynchronizationCapability> onOpenHandler,
                Action<DidCloseTextDocumentParams, TextSynchronizationCapability> onCloseHandler,
                Action<DidChangeTextDocumentParams, TextSynchronizationCapability> onChangeHandler,
                Action<DidSaveTextDocumentParams, TextSynchronizationCapability> onSaveHandler,
                RegistrationOptionsDelegate<TextDocumentSyncRegistrationOptions, TextSynchronizationCapability>? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<TextSynchronizationCapability>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnTextDocumentSync(
                this ILanguageServerRegistry registry,
                TextDocumentSyncKind kind,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                Action<DidOpenTextDocumentParams, TextSynchronizationCapability> onOpenHandler,
                Action<DidCloseTextDocumentParams, TextSynchronizationCapability> onCloseHandler,
                Action<DidChangeTextDocumentParams, TextSynchronizationCapability> onChangeHandler,
                Action<DidSaveTextDocumentParams, TextSynchronizationCapability> onSaveHandler,
                TextDocumentSyncRegistrationOptions? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<TextSynchronizationCapability>.Adapt(registrationOptions)
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
                RegistrationOptionsDelegate<TextDocumentSyncRegistrationOptions, TextSynchronizationCapability>? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<TextSynchronizationCapability>.Adapt(registrationOptions)
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
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<TextSynchronizationCapability>.Adapt(registrationOptions)
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
                RegistrationOptionsDelegate<TextDocumentSyncRegistrationOptions, TextSynchronizationCapability>? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<TextSynchronizationCapability>.Adapt(registrationOptions)
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
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<TextSynchronizationCapability>.Adapt(registrationOptions)
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
                RegistrationOptionsDelegate<TextDocumentSyncRegistrationOptions, TextSynchronizationCapability>? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<TextSynchronizationCapability>.Adapt(registrationOptions)
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
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<TextSynchronizationCapability>.Adapt(registrationOptions)
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
                RegistrationOptionsDelegate<TextDocumentSyncRegistrationOptions, TextSynchronizationCapability>? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getTextDocumentAttributes,
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<TextSynchronizationCapability>.Adapt(registrationOptions)
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
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onOpenHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onCloseHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onChangeHandler),
                        HandlerAdapter<TextSynchronizationCapability>.Adapt(onSaveHandler),
                        RegistrationAdapter<TextSynchronizationCapability>.Adapt(registrationOptions)
                    )
                );
            }

            private class DelegatingHandler : TextDocumentSyncHandlerBase
            {
                private readonly Func<DidOpenTextDocumentParams, TextSynchronizationCapability, CancellationToken, Task> _onOpenHandler;
                private readonly Func<DidCloseTextDocumentParams, TextSynchronizationCapability, CancellationToken, Task> _onCloseHandler;
                private readonly Func<DidChangeTextDocumentParams, TextSynchronizationCapability, CancellationToken, Task> _onChangeHandler;
                private readonly Func<DidSaveTextDocumentParams, TextSynchronizationCapability, CancellationToken, Task> _onSaveHandler;
                private readonly RegistrationOptionsDelegate<TextDocumentSyncRegistrationOptions, TextSynchronizationCapability> _registrationOptionsFactory;
                private readonly Func<DocumentUri, TextDocumentAttributes> _getTextDocumentAttributes;

                public DelegatingHandler(
                    Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                    Func<DidOpenTextDocumentParams, TextSynchronizationCapability, CancellationToken, Task> onOpenHandler,
                    Func<DidCloseTextDocumentParams, TextSynchronizationCapability, CancellationToken, Task> onCloseHandler,
                    Func<DidChangeTextDocumentParams, TextSynchronizationCapability, CancellationToken, Task> onChangeHandler,
                    Func<DidSaveTextDocumentParams, TextSynchronizationCapability, CancellationToken, Task> onSaveHandler,
                    RegistrationOptionsDelegate<TextDocumentSyncRegistrationOptions, TextSynchronizationCapability> registrationOptionsFactory
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

                protected override TextDocumentSyncRegistrationOptions CreateRegistrationOptions(
                    TextSynchronizationCapability capability, ClientCapabilities clientCapabilities
                )
                {
                    return _registrationOptionsFactory(capability, clientCapabilities);
                }

                public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri)
                {
                    return _getTextDocumentAttributes.Invoke(uri);
                }
            }
        }
    }
}
