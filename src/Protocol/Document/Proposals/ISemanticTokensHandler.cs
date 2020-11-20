using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    [Method(TextDocumentNames.SemanticTokensFull, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    public interface ISemanticTokensHandler : IJsonRpcRequestHandler<SemanticTokensParams, SemanticTokens?>,
                                              IRegistration<SemanticTokensRegistrationOptions, SemanticTokensCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    [Parallel]
    [Method(TextDocumentNames.SemanticTokensFullDelta, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    public interface ISemanticTokensDeltaHandler :
        IJsonRpcRequestHandler<SemanticTokensDeltaParams, SemanticTokensFullOrDelta?>,
        IRegistration<SemanticTokensRegistrationOptions, SemanticTokensCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    [Parallel]
    [Method(TextDocumentNames.SemanticTokensRange, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    public interface ISemanticTokensRangeHandler : IJsonRpcRequestHandler<SemanticTokensRangeParams, SemanticTokens?>,
                                                   IRegistration<SemanticTokensRegistrationOptions, SemanticTokensCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    public abstract class SemanticTokensHandlerBase :
        AbstractHandlers.Base<SemanticTokensRegistrationOptions, SemanticTokensCapability>,
        ISemanticTokensHandler,
        ISemanticTokensDeltaHandler,
        ISemanticTokensRangeHandler
    {
        public virtual async Task<SemanticTokens?> Handle(SemanticTokensParams request, CancellationToken cancellationToken)
        {
            var document = await GetSemanticTokensDocument(request, cancellationToken).ConfigureAwait(false);
            var builder = document.Create();
            await Tokenize(builder, request, cancellationToken).ConfigureAwait(false);
            return builder.Commit().GetSemanticTokens();
        }

        public virtual async Task<SemanticTokensFullOrDelta?> Handle(SemanticTokensDeltaParams request, CancellationToken cancellationToken)
        {
            var document = await GetSemanticTokensDocument(request, cancellationToken).ConfigureAwait(false);
            var builder = document.Edit(request);
            await Tokenize(builder, request, cancellationToken).ConfigureAwait(false);
            return builder.Commit().GetSemanticTokensEdits();
        }

        public virtual async Task<SemanticTokens?> Handle(SemanticTokensRangeParams request, CancellationToken cancellationToken)
        {
            var document = await GetSemanticTokensDocument(request, cancellationToken).ConfigureAwait(false);
            var builder = document.Create();
            await Tokenize(builder, request, cancellationToken).ConfigureAwait(false);
            return builder.Commit().GetSemanticTokens(request.Range);
        }

        public virtual void SetCapability(SemanticTokensCapability capability) => Capability = capability;
        protected SemanticTokensCapability Capability { get; private set; } = null!;
        protected abstract Task Tokenize(SemanticTokensBuilder builder, ITextDocumentIdentifierParams identifier, CancellationToken cancellationToken);
        protected abstract Task<SemanticTokensDocument> GetSemanticTokensDocument(ITextDocumentIdentifierParams @params, CancellationToken cancellationToken);
    }

    public static partial class SemanticTokensExtensions
    {
        private static SemanticTokensRegistrationOptions DefaultRegistrationOptionsFactory(SemanticTokensCapability capability) {
            var registrationOptions = new SemanticTokensRegistrationOptions {
                Full = new SemanticTokensCapabilityRequestFull()
            };
            registrationOptions.Range ??= new SemanticTokensCapabilityRequestRange();
            if (registrationOptions.Full?.IsValue == true)
            {
                registrationOptions.Full.Value.Delta = true;
            }

            // Ensure the legend is created properly.
            registrationOptions.Legend = new SemanticTokensLegend() {
                TokenModifiers = SemanticTokenModifier.Defaults.Join(capability.TokenModifiers, z => z, z => z, (a, b) => a).ToArray(),
                TokenTypes = SemanticTokenType.Defaults.Join(capability.TokenTypes, z => z, z => z, (a, b) => a).ToArray(),
            };

            return registrationOptions;
        }
        public static ILanguageServerRegistry OnSemanticTokens(
            this ILanguageServerRegistry registry,
            Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task> tokenize,
            Func<ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task<SemanticTokensDocument>> getSemanticTokensDocument,
            Func<SemanticTokensCapability, SemanticTokensRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= DefaultRegistrationOptionsFactory;
            return registry.AddHandlers(
                new DelegatingHandlerBase(tokenize, getSemanticTokensDocument, registrationOptionsFactory)
            );
        }

        public static ILanguageServerRegistry OnSemanticTokens(
            this ILanguageServerRegistry registry,
            Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, CancellationToken, Task> tokenize,
            Func<ITextDocumentIdentifierParams, CancellationToken, Task<SemanticTokensDocument>> getSemanticTokensDocument,
            Func<SemanticTokensCapability, SemanticTokensRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= DefaultRegistrationOptionsFactory;
            return registry.AddHandlers(
                new DelegatingHandlerBase(
                    (a, t, c, ct) => tokenize(a, t, ct),
                    (a, c, ct) => getSemanticTokensDocument(a, ct),
                    registrationOptionsFactory
                )
            );
        }

        public static ILanguageServerRegistry OnSemanticTokens(
            this ILanguageServerRegistry registry,
            Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, Task> tokenize,
            Func<ITextDocumentIdentifierParams, Task<SemanticTokensDocument>> getSemanticTokensDocument,
            Func<SemanticTokensCapability, SemanticTokensRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= DefaultRegistrationOptionsFactory;
            return registry.AddHandlers(
                new DelegatingHandlerBase(
                    (a, t, c, ct) => tokenize(a, t),
                    (a, c, ct) => getSemanticTokensDocument(a),
                    registrationOptionsFactory
                )
            );
        }

        private class DelegatingHandlerBase : SemanticTokensHandlerBase
        {
            private readonly Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task> _tokenize;
            private readonly Func<ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task<SemanticTokensDocument>> _getSemanticTokensDocument;
            private readonly Func<SemanticTokensCapability, SemanticTokensRegistrationOptions> _registrationOptionsFactory;

            public DelegatingHandlerBase(
                Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task> tokenize,
                Func<ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task<SemanticTokensDocument>> getSemanticTokensDocument,
                Func<SemanticTokensCapability, SemanticTokensRegistrationOptions> registrationOptionsFactory
            ) : base()
            {
                _tokenize = tokenize;
                _getSemanticTokensDocument = getSemanticTokensDocument;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            protected override Task Tokenize(SemanticTokensBuilder builder, ITextDocumentIdentifierParams identifier, CancellationToken cancellationToken)
                => _tokenize(builder, identifier, Capability, cancellationToken);

            protected override Task<SemanticTokensDocument> GetSemanticTokensDocument(ITextDocumentIdentifierParams @params, CancellationToken cancellationToken)
                => _getSemanticTokensDocument(@params, Capability, cancellationToken);

            protected override SemanticTokensRegistrationOptions CreateRegistrationOptions(SemanticTokensCapability capability) => _registrationOptionsFactory(capability);
        }

        public static IRequestProgressObservable<SemanticTokensPartialResult, SemanticTokens?> RequestSemanticTokens(
            this ITextDocumentLanguageClient mediator,
            SemanticTokensParams @params, CancellationToken cancellationToken = default
        ) =>
            mediator.ProgressManager.MonitorUntil(
                @params, (partial, result) => new SemanticTokens {
                    Data = partial.Data,
                    ResultId = result?.ResultId
                }, cancellationToken
            );

        public static IRequestProgressObservable<SemanticTokensFullOrDeltaPartialResult, SemanticTokensFullOrDelta?> RequestSemanticTokensDelta(
            this ITextDocumentLanguageClient mediator, SemanticTokensDeltaParams @params, CancellationToken cancellationToken = default
        ) =>
            mediator.ProgressManager.MonitorUntil(
                @params, (partial, result) => {
                    if (partial.IsDelta)
                    {
                        return new SemanticTokensFullOrDelta(
                            new SemanticTokensDelta {
                                Edits = partial.Delta!.Edits,
                                ResultId = result?.Delta?.ResultId ?? result?.Full?.ResultId
                            }
                        );
                    }

                    if (partial.IsFull)
                    {
                        return new SemanticTokensFullOrDelta(
                            new SemanticTokens {
                                Data = partial.Full!.Data,
                                ResultId = result?.Full?.ResultId ?? result?.Delta?.ResultId
                            }
                        );
                    }

                    return new SemanticTokensFullOrDelta(new SemanticTokens());
                }, cancellationToken
            );

        public static IRequestProgressObservable<SemanticTokensPartialResult, SemanticTokens?> RequestSemanticTokensRange(
            this ITextDocumentLanguageClient mediator,
            SemanticTokensRangeParams @params, CancellationToken cancellationToken = default
        ) =>
            mediator.ProgressManager.MonitorUntil(
                @params, (partial, result) => new SemanticTokens {
                    Data = partial.Data,
                    ResultId = result?.ResultId
                }, cancellationToken
            );
    }
}
