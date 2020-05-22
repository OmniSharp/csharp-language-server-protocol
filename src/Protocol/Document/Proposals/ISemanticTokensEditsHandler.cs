using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals
{
    [Obsolete(Constants.Proposal)]
    [Parallel, Method(TextDocumentNames.SemanticTokens, Direction.ClientToServer)]
    public interface ISemanticTokensHandler : IJsonRpcRequestHandler<SemanticTokensParams, SemanticTokens>,
        IRegistration<SemanticTokensRegistrationOptions>, ICapability<SemanticTokensCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    [Parallel, Method(TextDocumentNames.SemanticTokensEdits, Direction.ClientToServer)]
    public interface ISemanticTokensEditsHandler :
        IJsonRpcRequestHandler<SemanticTokensEditsParams, SemanticTokensOrSemanticTokensEdits>,
        IRegistration<SemanticTokensRegistrationOptions>, ICapability<SemanticTokensCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    [Parallel, Method(TextDocumentNames.SemanticTokensRange, Direction.ClientToServer)]
    public interface ISemanticTokensRangeHandler : IJsonRpcRequestHandler<SemanticTokensRangeParams, SemanticTokens>,
        IRegistration<SemanticTokensRegistrationOptions>, ICapability<SemanticTokensCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    public abstract class SemanticTokensHandler : ISemanticTokensHandler, ISemanticTokensEditsHandler,
        ISemanticTokensRangeHandler
    {
        private readonly SemanticTokensRegistrationOptions _options;

        public SemanticTokensHandler(SemanticTokensRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public SemanticTokensRegistrationOptions GetRegistrationOptions() => _options;

        public virtual async Task<SemanticTokens> Handle(SemanticTokensParams request, CancellationToken cancellationToken)
        {
            var document = await GetSemanticTokensDocument(request, cancellationToken);
            var builder = document.Create();
            await Tokenize(builder, request, cancellationToken);
            return builder.Commit().GetSemanticTokens();
        }

        public virtual async Task<SemanticTokensOrSemanticTokensEdits> Handle(SemanticTokensEditsParams request, CancellationToken cancellationToken)
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
    public static class SemanticTokensExtensions
    {
        public static IDisposable OnSemanticTokens(
            this ILanguageServerRegistry registry,
            Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task> tokenize,
            Func<ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task<SemanticTokensDocument>> getSemanticTokensDocument,
            SemanticTokensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new SemanticTokensRegistrationOptions() {
                DocumentProvider =
                    new Supports<SemanticTokensDocumentProviderOptions>(true,
                        new SemanticTokensDocumentProviderOptions() { })
            };
            registrationOptions.RangeProvider = true;
            if (registrationOptions.DocumentProvider != null)
            {
                registrationOptions.DocumentProvider.Edits = true;
            }

            return registry.AddHandlers(
                new DelegatingHandler(tokenize, getSemanticTokensDocument, registrationOptions));
        }

        public static IDisposable OnSemanticTokens(
            this ILanguageServerRegistry registry,
            Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, CancellationToken, Task> tokenize,
            Func<ITextDocumentIdentifierParams, CancellationToken, Task<SemanticTokensDocument>> getSemanticTokensDocument,
            SemanticTokensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new SemanticTokensRegistrationOptions();
            registrationOptions.DocumentProvider =
                new Supports<SemanticTokensDocumentProviderOptions>(true,
                    new SemanticTokensDocumentProviderOptions() { });
            registrationOptions.RangeProvider = true;
            if (registrationOptions.DocumentProvider != null)
            {
                registrationOptions.DocumentProvider.Edits = true;
            }

            return registry.AddHandlers(
                new DelegatingHandler(
                    (a, t, c, ct) => tokenize(a, t, ct),
                    (a, c, ct) => getSemanticTokensDocument(a, ct),
                    registrationOptions));
        }

        public static IDisposable OnSemanticTokens(
            this ILanguageServerRegistry registry,
            Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, Task> tokenize,
            Func<ITextDocumentIdentifierParams, Task<SemanticTokensDocument>> getSemanticTokensDocument,
            SemanticTokensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new SemanticTokensRegistrationOptions();
            registrationOptions.DocumentProvider =
                new Supports<SemanticTokensDocumentProviderOptions>(true,
                    new SemanticTokensDocumentProviderOptions() { });
            registrationOptions.RangeProvider = true;
            if (registrationOptions.DocumentProvider != null)
            {
                registrationOptions.DocumentProvider.Edits = true;
            }

            return registry.AddHandlers(
                new DelegatingHandler(
                    (a, t, c, ct) => tokenize(a, t),
                    (a, c, ct) => getSemanticTokensDocument(a),
                    registrationOptions));
        }

        class DelegatingHandler : SemanticTokensHandler
        {
            private readonly Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task> _tokenize;
            private readonly Func<ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task<SemanticTokensDocument>> _getSemanticTokensDocument;

            private SemanticTokensCapability _capability;

            public DelegatingHandler(
                Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task> tokenize,
                Func<ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task<SemanticTokensDocument>> getSemanticTokensDocument,
                SemanticTokensRegistrationOptions registrationOptions
            ) : base(registrationOptions)
            {
                _tokenize = tokenize;
                _getSemanticTokensDocument = getSemanticTokensDocument;
            }

            public DelegatingHandler(
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

            protected override Task Tokenize(SemanticTokensBuilder builder, ITextDocumentIdentifierParams identifier,
                CancellationToken cancellationToken) =>
                _tokenize(builder, identifier, _capability, cancellationToken);

            protected override Task<SemanticTokensDocument> GetSemanticTokensDocument(
                ITextDocumentIdentifierParams @params, CancellationToken cancellationToken) =>
                _getSemanticTokensDocument(@params, _capability, cancellationToken);

            public override void SetCapability(SemanticTokensCapability capability) => _capability = capability;
        }

        public static IRequestProgressObservable<SemanticTokensPartialResult, SemanticTokens> RequestSemanticTokens(this ITextDocumentLanguageClient mediator,
            SemanticTokensParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.ProgressManager.MonitorUntil(@params, (partial, result) => new SemanticTokens() {
                Data = partial.Data,
                ResultId = result.ResultId
            }, cancellationToken);
        }

        public static IRequestProgressObservable<SemanticTokensPartialResultOrSemanticTokensEditsPartialResult, SemanticTokensOrSemanticTokensEdits> RequestSemanticTokensEdits(
            this ITextDocumentLanguageClient mediator, SemanticTokensEditsParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.ProgressManager.MonitorUntil(@params, (partial, result) => {
                if (partial.IsSemanticTokensPartialResult)
                {
                    return new SemanticTokensOrSemanticTokensEdits(new SemanticTokens() {
                        Data = partial.SemanticTokensPartialResult.Data,
                        ResultId = result.SemanticTokens?.ResultId ?? result.SemanticTokensEdits?.ResultId
                    });
                }

                if (partial.IsSemanticTokensEditsPartialResult)
                {
                    return new SemanticTokensOrSemanticTokensEdits(new SemanticTokensEdits() {
                        Edits = partial.SemanticTokensEditsPartialResult.Edits,
                        ResultId = result.SemanticTokensEdits?.ResultId ?? result.SemanticTokens?.ResultId
                    });
                }

                return new SemanticTokensOrSemanticTokensEdits(new SemanticTokens());
            }, cancellationToken);
        }

        public static IRequestProgressObservable<SemanticTokensPartialResult, SemanticTokens> RequestSemanticTokensRange(this ITextDocumentLanguageClient mediator,
            SemanticTokensRangeParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.ProgressManager.MonitorUntil(@params, (partial, result) => new SemanticTokens() {
                Data = partial.Data,
                ResultId = result.ResultId
            }, cancellationToken);
        }
    }
}
