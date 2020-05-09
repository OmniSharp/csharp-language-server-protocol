using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document.Server.Proposals
{
    [Obsolete(Constants.Proposal)]
    [Parallel, Method(DocumentNames.SemanticTokens)]
    public interface ISemanticTokensHandler : IJsonRpcRequestHandler<SemanticTokensParams, SemanticTokens>,
        IRegistration<SemanticTokensRegistrationOptions>, ICapability<SemanticTokensCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    [Parallel, Method(DocumentNames.SemanticTokensEdits)]
    public interface ISemanticTokensEditsHandler :
        IJsonRpcRequestHandler<SemanticTokensEditsParams, SemanticTokensOrSemanticTokensEdits>,
        IRegistration<SemanticTokensRegistrationOptions>, ICapability<SemanticTokensCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    [Parallel, Method(DocumentNames.SemanticTokensRange)]
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

        public virtual async Task<SemanticTokens> Handle(SemanticTokensParams request,
            CancellationToken cancellationToken)
        {
            var document = await GetSemanticTokensDocument(request, cancellationToken);
            var builder = document.Create();
            await Tokenize(builder, request, cancellationToken);
            return builder.Commit().GetSemanticTokens();
        }

        public virtual async Task<SemanticTokensOrSemanticTokensEdits> Handle(SemanticTokensEditsParams request,
            CancellationToken cancellationToken)
        {
            var document = await GetSemanticTokensDocument(request, cancellationToken);
            var builder = document.Edit(request);
            await Tokenize(builder, request, cancellationToken);
            return builder.Commit().GetSemanticTokensEdits();
        }

        public virtual async Task<SemanticTokens> Handle(SemanticTokensRangeParams request,
            CancellationToken cancellationToken)
        {
            var document = await GetSemanticTokensDocument(request, cancellationToken);
            var builder = document.Create();
            await Tokenize(builder, request, cancellationToken);

            return builder.Commit().GetSemanticTokens(request.Range);
        }

        public virtual void SetCapability(SemanticTokensCapability capability) => Capability = capability;
        protected SemanticTokensCapability Capability { get; private set; }

        protected abstract Task Tokenize(
            SemanticTokensBuilder builder,
            ITextDocumentIdentifierParams identifier,
            CancellationToken cancellationToken);

        protected abstract Task<SemanticTokensDocument> GetSemanticTokensDocument(
            ITextDocumentIdentifierParams @params, CancellationToken cancellationToken);
    }

    [Obsolete(Constants.Proposal)]
    public static class SemanticTokensEditsHandlerExtensions
    {
        public static IDisposable OnSemanticTokensEdits(
            this ILanguageServerRegistry registry,
            Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, CancellationToken, Task> tokenize,
            Func<ITextDocumentIdentifierParams, CancellationToken, Task<SemanticTokensDocument>>
                getSemanticTokensDocument,
            SemanticTokensRegistrationOptions registrationOptions = null,
            Action<SemanticTokensCapability> setCapability = null)
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
                new DelegatingHandler(tokenize, getSemanticTokensDocument, setCapability, registrationOptions));
        }

        class DelegatingHandler : SemanticTokensHandler
        {
            private readonly Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, CancellationToken, Task>
                _tokenize;

            private readonly Func<ITextDocumentIdentifierParams, CancellationToken, Task<SemanticTokensDocument>>
                _getSemanticTokensDocument;

            private readonly Action<SemanticTokensCapability> _setCapability;

            public DelegatingHandler(
                Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, CancellationToken, Task> tokenize,
                Func<ITextDocumentIdentifierParams, CancellationToken, Task<SemanticTokensDocument>>
                    getSemanticTokensDocument,
                Action<SemanticTokensCapability> setCapability,
                SemanticTokensRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _tokenize = tokenize;
                _getSemanticTokensDocument = getSemanticTokensDocument;
                _setCapability = setCapability;
            }

            protected override Task Tokenize(SemanticTokensBuilder builder, ITextDocumentIdentifierParams identifier,
                CancellationToken cancellationToken) =>
                _tokenize(builder, identifier, cancellationToken);

            protected override Task<SemanticTokensDocument> GetSemanticTokensDocument(
                ITextDocumentIdentifierParams @params, CancellationToken cancellationToken) =>
                _getSemanticTokensDocument(@params, cancellationToken);

            public override void SetCapability(SemanticTokensCapability capability) =>
                _setCapability?.Invoke(capability);
        }
    }
}
