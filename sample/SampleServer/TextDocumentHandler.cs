using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Server;
using ILanguageServer = OmniSharp.Extensions.LanguageServer.Server.ILanguageServer;

namespace SampleServer
{
    class TextDocumentHandler : ITextDocumentSyncHandler
    {
        private readonly ILogger<TextDocumentHandler> _logger;

        private readonly DocumentSelector _documentSelector = new DocumentSelector(
            new DocumentFilter()
            {
                Pattern = "**/*.cs"
            }
        );

        private TextDocumentSyncCapability _capability;

        public TextDocumentHandler(ILogger<TextDocumentHandler> logger, Foo foo)
        {
            _logger = logger;
            foo.SayFoo();
        }

        public TextDocumentSyncKind Change { get; } = TextDocumentSyncKind.Full;

        public Task<Unit> Handle(DidChangeTextDocumentParams notification, CancellationToken token)
        {
            _logger.LogInformation("Hello world!");
            return Unit.Task;
        }

        TextDocumentChangeRegistrationOptions IRegistration<TextDocumentChangeRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentChangeRegistrationOptions()
            {
                DocumentSelector = _documentSelector,
                SyncKind = Change
            };
        }

        public void SetCapability(TextDocumentSyncCapability capability)
        {
            _capability = capability;
        }

        public async Task<Unit> Handle(DidOpenTextDocumentParams notification, CancellationToken token)
        {
            await Task.Yield();
            _logger.LogInformation("Hello world!");
            return Unit.Value;
        }

        TextDocumentRegistrationOptions IRegistration<TextDocumentRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentRegistrationOptions()
            {
                DocumentSelector = _documentSelector,
            };
        }

        public Task<Unit> Handle(DidCloseTextDocumentParams notification, CancellationToken token)
        {
            return Unit.Task;
        }

        public Task<Unit> Handle(DidSaveTextDocumentParams notification, CancellationToken token)
        {
            return Unit.Task;
        }

        TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentSaveRegistrationOptions()
            {
                DocumentSelector = _documentSelector,
                IncludeText = true
            };
        }
        public TextDocumentAttributes GetTextDocumentAttributes(Uri uri)
        {
            return new TextDocumentAttributes(uri, "csharp");
        }
    }

    class FoldingRangeHandler : OmniSharp.Extensions.LanguageServer.Protocol.Server.FoldingRangeHandler
    {
        private FoldingRangeCapability _capability;

        public FoldingRangeHandler(ProgressManager progressManager) : base(new FoldingRangeRegistrationOptions()
        {
            DocumentSelector = DocumentSelector.ForLanguage("csharp")
        }, progressManager)
        {
        }

        public override Task<Container<FoldingRange>> Handle(FoldingRangeParam request, IObserver<Container<FoldingRange>> partialResults, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Container<FoldingRange>(new FoldingRange()
            {
                StartLine = 10,
                EndLine = 20,
                Kind = FoldingRangeKind.Region,
                EndCharacter = 0,
                StartCharacter = 0
            }));
        }
    }

    class DidChangeWatchedFilesHandler : OmniSharp.Extensions.LanguageServer.Protocol.Server.DidChangeWatchedFilesHandler
    {
        private DidChangeWatchedFilesCapability _capability;

        public DidChangeWatchedFilesHandler() : base(new DidChangeWatchedFilesRegistrationOptions() { })
        {
        }

        public override Task<Unit> Handle(DidChangeWatchedFilesParams request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }
    }
}
