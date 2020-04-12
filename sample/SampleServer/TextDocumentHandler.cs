using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Server;

namespace SampleServer
{
    class TextDocumentHandler : ITextDocumentSyncHandler
    {
        private readonly ILogger<TextDocumentHandler> _logger;
        private readonly ILanguageServerConfiguration _configuration;

        private readonly DocumentSelector _documentSelector = new DocumentSelector(
            new DocumentFilter() {
                Pattern = "**/*.cs"
            }
        );

        private SynchronizationCapability _capability;

        public TextDocumentHandler(ILogger<TextDocumentHandler> logger, Foo foo,
            ILanguageServerConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            foo.SayFoo();
        }

        public TextDocumentSyncKind Change { get; } = TextDocumentSyncKind.Full;

        public Task<Unit> Handle(DidChangeTextDocumentParams notification, CancellationToken token)
        {
            _logger.LogCritical("Critical");
            _logger.LogDebug("Debug");
            _logger.LogTrace("Trace");
            _logger.LogInformation("Hello world!");
            return Unit.Task;
        }

        TextDocumentChangeRegistrationOptions IRegistration<TextDocumentChangeRegistrationOptions>.
            GetRegistrationOptions()
        {
            return new TextDocumentChangeRegistrationOptions() {
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
            await _configuration.GetScopedConfiguration(notification.TextDocument.Uri);
            return Unit.Value;
        }

        TextDocumentRegistrationOptions IRegistration<TextDocumentRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentRegistrationOptions() {
                DocumentSelector = _documentSelector,
            };
        }

        public Task<Unit> Handle(DidCloseTextDocumentParams notification, CancellationToken token)
        {
            if (_configuration.TryGetScopedConfiguration(notification.TextDocument.Uri, out var disposable))
            {
                disposable.Dispose();
            }

            return Unit.Task;
        }

        public Task<Unit> Handle(DidSaveTextDocumentParams notification, CancellationToken token)
        {
            return Unit.Task;
        }

        TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentSaveRegistrationOptions() {
                DocumentSelector = _documentSelector,
                IncludeText = true
            };
        }

        public TextDocumentAttributes GetTextDocumentAttributes(Uri uri)
        {
            return new TextDocumentAttributes(uri, "csharp");
        }
    }

    class MyDocumentSymbolHandler : OmniSharp.Extensions.LanguageServer.Protocol.Server.DocumentSymbolHandler
    {
        public MyDocumentSymbolHandler(ProgressManager progressManager) : base(new DocumentSymbolRegistrationOptions() {
            DocumentSelector = DocumentSelector.ForLanguage("csharp")
        }, progressManager)
        {
        }

        public override async Task<SymbolInformationOrDocumentSymbolContainer> Handle(DocumentSymbolParams request,
            CancellationToken cancellationToken)
        {
            // you would normally get this from a common source that is managed by current open editor, current active editor, etc.
            var content = await File.ReadAllTextAsync(DocumentUri.GetFileSystemPath(request), cancellationToken);
            var lines = content.Split('\n');
            var symbols = new List<SymbolInformationOrDocumentSymbol>();
            for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                var line = lines[lineIndex];
                var parts = line.Split(' ', '.', '(', ')', '{', '}', '[', ']', ';');
                var currentCharacter = 0;
                foreach (var part in parts)
                {
                    if (string.IsNullOrWhiteSpace(part))
                    {
                        currentCharacter += part.Length + 1;
                        continue;
                    }

                    symbols.Add(new DocumentSymbol() {
                        Detail = part,
                        Deprecated = true,
                        Kind = SymbolKind.Field,
                        Tags = new [] { SymbolTag.Deprecated },
                        Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(
                            new Position(lineIndex, currentCharacter),
                            new Position(lineIndex, currentCharacter + part.Length)),
                        SelectionRange =
                            new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(
                                new Position(lineIndex, currentCharacter),
                                new Position(lineIndex, currentCharacter + part.Length)),
                        Name = part
                    });
                    currentCharacter += part.Length + 1;
                }
            }

            // await Task.Delay(2000, cancellationToken);
            return symbols;
        }
    }

    class MyWorkspaceSymbolsHandler : OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkspaceSymbolsHandler
    {
        private readonly ILogger<MyWorkspaceSymbolsHandler> logger;

        public MyWorkspaceSymbolsHandler(ProgressManager progressManager, ILogger<MyWorkspaceSymbolsHandler> logger) :
            base(new WorkspaceSymbolRegistrationOptions() { }, progressManager)
        {
            this.logger = logger;
        }

        public override async Task<Container<SymbolInformation>> Handle(WorkspaceSymbolParams request,
            CancellationToken cancellationToken)
        {
            using var reporter = ProgressManager.WorkDone(request, new WorkDoneProgressBegin() {
                Cancellable = true,
                Message = "This might take a while...",
                Title = "Some long task....",
                Percentage = 0
            });
            using var partialResults = ProgressManager.For(request, cancellationToken);
            if (partialResults != null)
            {
                await Task.Delay(2000, cancellationToken);

                reporter.OnNext(new WorkDoneProgressReport() {
                    Cancellable = true,
                    Percentage = 20
                });
                await Task.Delay(500, cancellationToken);

                reporter.OnNext(new WorkDoneProgressReport() {
                    Cancellable = true,
                    Percentage = 40
                });
                await Task.Delay(500, cancellationToken);

                reporter.OnNext(new WorkDoneProgressReport() {
                    Cancellable = true,
                    Percentage = 50
                });
                await Task.Delay(500, cancellationToken);

                partialResults.OnNext(new[] {
                    new SymbolInformation() {
                        ContainerName = "Partial Container",
                        Deprecated = true,
                        Kind = SymbolKind.Constant,
                        Location = new Location() {
                            Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(new Position(2, 1),
                                new Position(2, 10)) { }
                        },
                        Name = "Partial name"
                    }
                });

                reporter.OnNext(new WorkDoneProgressReport() {
                    Cancellable = true,
                    Percentage = 70
                });
                await Task.Delay(500, cancellationToken);

                reporter.OnNext(new WorkDoneProgressReport() {
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
                        Location = new Location() {
                            Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(new Position(1, 1),
                                new Position(1, 10)) { }
                        },
                        Name = "name"
                    }
                };
            }
            finally
            {
                reporter.OnNext(new WorkDoneProgressReport() {
                    Cancellable = true,
                    Percentage = 100
                });
            }
        }
    }
}
