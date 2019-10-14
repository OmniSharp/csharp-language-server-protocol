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

        private SynchronizationCapability _capability;

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

        public void SetCapability(SynchronizationCapability capability)
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
        public FoldingRangeHandler(ProgressManager progressManager) : base(new FoldingRangeRegistrationOptions()
        {
            DocumentSelector = DocumentSelector.ForLanguage("csharp")
        }, progressManager)
        {
        }

        public override Task<Container<FoldingRange>> Handle(FoldingRangeParam request, CancellationToken cancellationToken)
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

    class MyDocumentSymbolHandler : OmniSharp.Extensions.LanguageServer.Protocol.Server.DocumentSymbolHandler
    {
        public MyDocumentSymbolHandler(ProgressManager progressManager) : base(new DocumentSymbolRegistrationOptions()
        {
            DocumentSelector = DocumentSelector.ForLanguage("csharp")
        }, progressManager)
        {
        }

        public async override Task<SymbolInformationOrDocumentSymbolContainer> Handle(DocumentSymbolParams request, CancellationToken cancellationToken)
        {
            await Task.Delay(2000, cancellationToken);
            return new[] {
                    new SymbolInformation() {
                        ContainerName = "Container",
                        Deprecated = true,
                        Kind = SymbolKind.Constant,
                        Location = new Location() { Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(new Position(1, 1), new Position(1, 10)) {} },
                        Name = "name"
                    }
                };
        }
    }

    class MyWorkspaceSymbolsHandler : OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkspaceSymbolsHandler
    {
        private readonly ILogger<MyWorkspaceSymbolsHandler> logger;

        public MyWorkspaceSymbolsHandler(ProgressManager progressManager, ILogger<MyWorkspaceSymbolsHandler> logger) : base(new WorkspaceSymbolRegistrationOptions() { }, progressManager)
        {
            this.logger = logger;
        }

        public async override Task<Container<SymbolInformation>> Handle(WorkspaceSymbolParams request, CancellationToken cancellationToken)
        {
            using var reporter = ProgressManager.WorkDone(request, new WorkDoneProgressBegin()
            {
                Cancellable = true,
                Message = "This might take a while...",
                Title = "Some long task....",
                Percentage = 0
            });
            using var partialResults = ProgressManager.For(request, cancellationToken);
            if (partialResults != null)
            {
                await Task.Delay(2000, cancellationToken);

                reporter.OnNext(new WorkDoneProgressReport()
                {
                    Cancellable = true,
                    Percentage = 20
                });
                await Task.Delay(500, cancellationToken);

                reporter.OnNext(new WorkDoneProgressReport()
                {
                    Cancellable = true,
                    Percentage = 40
                });
                await Task.Delay(500, cancellationToken);

                reporter.OnNext(new WorkDoneProgressReport()
                {
                    Cancellable = true,
                    Percentage = 50
                });
                await Task.Delay(500, cancellationToken);

                partialResults.OnNext(new[] {
                    new SymbolInformation() {
                        ContainerName = "Partial Container",
                        Deprecated = true,
                        Kind = SymbolKind.Constant,
                        Location = new Location() { Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(new Position(2, 1), new Position(2, 10)) {} },
                        Name = "Partial name"
                    }
                });

                reporter.OnNext(new WorkDoneProgressReport()
                {
                    Cancellable = true,
                    Percentage = 70
                });
                await Task.Delay(500, cancellationToken);

                reporter.OnNext(new WorkDoneProgressReport()
                {
                    Cancellable = true,
                    Percentage = 90
                });

                partialResults.OnCompleted();
                return new SymbolInformation[] { };
            }

            try
            {
                return new[] {
                    new SymbolInformation() {
                        ContainerName = "Container",
                        Deprecated = true,
                        Kind = SymbolKind.Constant,
                        Location = new Location() { Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(new Position(1, 1), new Position(1, 10)) {} },
                        Name = "name"
                    }
                };
            }
            finally
            {
                reporter.OnNext(new WorkDoneProgressReport()
                {
                    Cancellable = true,
                    Percentage = 100
                });
            }
        }
    }

    class DidChangeWatchedFilesHandler : OmniSharp.Extensions.LanguageServer.Protocol.Server.DidChangeWatchedFilesHandler
    {
        public DidChangeWatchedFilesHandler() : base(new DidChangeWatchedFilesRegistrationOptions() { })
        {
        }

        public override Task<Unit> Handle(DidChangeWatchedFilesParams request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }
    }
}
