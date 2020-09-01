using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
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
    public interface ISemanticTokensHandler : IJsonRpcRequestHandler<SemanticTokensParams, SemanticTokens>,
                                              IRegistration<SemanticTokensRegistrationOptions>, ICapability<SemanticTokensCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    [Parallel]
    [Method(TextDocumentNames.SemanticTokensFullDelta, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    public interface ISemanticTokensDeltaHandler :
        IJsonRpcRequestHandler<SemanticTokensDeltaParams, SemanticTokensFullOrDelta?>,
        IRegistration<SemanticTokensRegistrationOptions>, ICapability<SemanticTokensCapability>, IDoesNotParticipateInRegistration
    {
    }

    [Obsolete(Constants.Proposal)]
    [Parallel]
    [Method(TextDocumentNames.SemanticTokensRange, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    public interface ISemanticTokensRangeHandler : IJsonRpcRequestHandler<SemanticTokensRangeParams, SemanticTokens>,
                                                   IRegistration<SemanticTokensRegistrationOptions>, ICapability<SemanticTokensCapability>, IDoesNotParticipateInRegistration
    {
    }

    [Obsolete(Constants.Proposal)]
    public abstract class SemanticTokensHandlerBase : ISemanticTokensHandler, ISemanticTokensDeltaHandler,
                                                      ISemanticTokensRangeHandler
    {
        private readonly SemanticTokensRegistrationOptions _options;

        public SemanticTokensHandlerBase(SemanticTokensRegistrationOptions registrationOptions) => _options = registrationOptions;

        public SemanticTokensRegistrationOptions GetRegistrationOptions() => _options;

        public virtual async Task<SemanticTokens> Handle(SemanticTokensParams request, CancellationToken cancellationToken)
        {
            var document = await GetSemanticTokensDocument(request, cancellationToken);
            var builder = document.Create();
            await Tokenize(builder, request, cancellationToken);
            return builder.Commit().GetSemanticTokens();
        }

        public virtual async Task<SemanticTokensFullOrDelta?> Handle(SemanticTokensDeltaParams request, CancellationToken cancellationToken)
        {
            var document = await GetSemanticTokensDocument(request, cancellationToken);
            var builder = document.Edit(request);
            await Tokenize(builder, request, cancellationToken);
            return builder.Commit().GetSemanticTokensEdits();
        }

        public virtual async Task<SemanticTokens> Handle(SemanticTokensRangeParams request, CancellationToken cancellationToken)
        {
            var document = await GetSemanticTokensDocument(request, cancellationToken);
            var builder = document.Create();
            await Tokenize(builder, request, cancellationToken);
            return builder.Commit().GetSemanticTokens(request.Range);
        }

        public virtual void SetCapability(SemanticTokensCapability capability) => Capability = capability;
        protected SemanticTokensCapability Capability { get; private set; }

        protected abstract Task Tokenize(SemanticTokensBuilder builder, ITextDocumentIdentifierParams identifier, CancellationToken cancellationToken);

        protected abstract Task<SemanticTokensDocument> GetSemanticTokensDocument(ITextDocumentIdentifierParams @params, CancellationToken cancellationToken);
    }

    [Obsolete(Constants.Proposal)]
    public static partial class SemanticTokensExtensions
    {
        public static ILanguageServerRegistry OnSemanticTokens(
            this ILanguageServerRegistry registry,
            Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task> tokenize,
            Func<ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task<SemanticTokensDocument>> getSemanticTokensDocument,
            SemanticTokensRegistrationOptions registrationOptions
        )
        {
            registrationOptions ??= new SemanticTokensRegistrationOptions {
                Full = new SemanticTokensCapabilityRequestFull()
            };
            registrationOptions.Range ??= new SemanticTokensCapabilityRequestRange();
            if (registrationOptions?.Full?.IsValue == true)
            {
                registrationOptions.Full.Value.Delta = true;
            }

            return registry.AddHandlers(
                new DelegatingHandlerBase(tokenize, getSemanticTokensDocument, registrationOptions)
            );
        }

        public static ILanguageServerRegistry OnSemanticTokens(
            this ILanguageServerRegistry registry,
            Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, CancellationToken, Task> tokenize,
            Func<ITextDocumentIdentifierParams, CancellationToken, Task<SemanticTokensDocument>> getSemanticTokensDocument,
            SemanticTokensRegistrationOptions registrationOptions
        )
        {
            registrationOptions ??= new SemanticTokensRegistrationOptions {
                Full = new SemanticTokensCapabilityRequestFull()
            };
            registrationOptions.Range ??= new SemanticTokensCapabilityRequestRange();
            if (registrationOptions?.Full?.IsValue == true)
            {
                registrationOptions.Full.Value.Delta = true;
            }

            return registry.AddHandlers(
                new DelegatingHandlerBase(
                    (a, t, c, ct) => tokenize(a, t, ct),
                    (a, c, ct) => getSemanticTokensDocument(a, ct),
                    registrationOptions
                )
            );
        }

        public static ILanguageServerRegistry OnSemanticTokens(
            this ILanguageServerRegistry registry,
            Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, Task> tokenize,
            Func<ITextDocumentIdentifierParams, Task<SemanticTokensDocument>> getSemanticTokensDocument,
            SemanticTokensRegistrationOptions registrationOptions
        )
        {
            registrationOptions ??= new SemanticTokensRegistrationOptions {
                Full = new SemanticTokensCapabilityRequestFull()
            };
            registrationOptions.Range ??= new SemanticTokensCapabilityRequestRange();
            if (registrationOptions?.Full?.IsValue == true)
            {
                registrationOptions.Full.Value.Delta = true;
            }

            return registry.AddHandlers(
                new DelegatingHandlerBase(
                    (a, t, c, ct) => tokenize(a, t),
                    (a, c, ct) => getSemanticTokensDocument(a),
                    registrationOptions
                )
            );
        }

        private class DelegatingHandlerBase : SemanticTokensHandlerBase
        {
            private readonly Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task> _tokenize;
            private readonly Func<ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task<SemanticTokensDocument>> _getSemanticTokensDocument;

            private SemanticTokensCapability _capability;

            public DelegatingHandlerBase(
                Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task> tokenize,
                Func<ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task<SemanticTokensDocument>> getSemanticTokensDocument,
                SemanticTokensRegistrationOptions registrationOptions
            ) : base(registrationOptions)
            {
                _tokenize = tokenize;
                _getSemanticTokensDocument = getSemanticTokensDocument;
            }

            public DelegatingHandlerBase(
                Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, SemanticTokensCapability, Task> tokenize,
                Func<ITextDocumentIdentifierParams, SemanticTokensCapability, Task<SemanticTokensDocument>> getSemanticTokensDocument,
                SemanticTokensRegistrationOptions registrationOptions
            ) : this(
                (s, t, c, ct) => tokenize(s, t, c),
                (t, c, ct) => getSemanticTokensDocument(t, c),
                registrationOptions
            )
            {
            }

            protected override Task Tokenize(
                SemanticTokensBuilder builder, ITextDocumentIdentifierParams identifier,
                CancellationToken cancellationToken
            ) =>
                _tokenize(builder, identifier, _capability, cancellationToken);

            protected override Task<SemanticTokensDocument> GetSemanticTokensDocument(
                ITextDocumentIdentifierParams @params, CancellationToken cancellationToken
            ) =>
                _getSemanticTokensDocument(@params, _capability, cancellationToken);

            public override void SetCapability(SemanticTokensCapability capability) => _capability = capability;
        }

        public static IRequestProgressObservable<SemanticTokensPartialResult, SemanticTokens> RequestSemanticTokens(
            this ITextDocumentLanguageClient mediator,
            SemanticTokensParams @params, CancellationToken cancellationToken = default
        ) =>
            mediator.ProgressManager.MonitorUntil(
                @params, (partial, result) => new SemanticTokens {
                    Data = partial.Data,
                    ResultId = result.ResultId
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
                                Edits = partial.Delta.Edits,
                                ResultId = result?.Delta?.ResultId ?? result?.Full?.ResultId
                            }
                        );
                    }

                    if (partial.IsFull)
                    {
                        return new SemanticTokensFullOrDelta(
                            new SemanticTokens {
                                Data = partial.Full.Data,
                                ResultId = result?.Full?.ResultId ?? result?.Delta?.ResultId
                            }
                        );
                    }

                    return new SemanticTokensFullOrDelta(new SemanticTokens());
                }, cancellationToken
            );

        public static IRequestProgressObservable<SemanticTokensPartialResult, SemanticTokens> RequestSemanticTokensRange(
            this ITextDocumentLanguageClient mediator,
            SemanticTokensRangeParams @params, CancellationToken cancellationToken = default
        ) =>
            mediator.ProgressManager.MonitorUntil(
                @params, (partial, result) => new SemanticTokens {
                    Data = partial.Data,
                    ResultId = result.ResultId
                }, cancellationToken
            );
    }
}
