using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit;
using Xunit.Abstractions;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace OmniSharp.Extensions.LanguageServer.Client.Tests
{
    /// <summary>
    /// Tests for <see cref="LanguageClient" />.
    /// </summary>
    public class ClientTests : PipeServerTestBase
    {
        /// <summary>
        /// Create a new <see cref="LanguageClient" /> test suite.
        /// </summary>
        /// <param name="testOutput">
        /// Output for the current test.
        /// </param>
        public ClientTests(ITestOutputHelper testOutput)
            : base(testOutput)
        {
        }

        /// <summary>
        /// Get an absolute document path for use in tests.
        /// </summary>
        private string AbsoluteDocumentPath => IsWindows ? @"c:\Foo.txt" : "/Foo.txt";

        /// <summary>
        /// Ensure that the language client can successfully request Hover information.
        /// </summary>
        [Fact(DisplayName = "Language client can successfully request hover info")]
        public async Task Hover_Success()
        {
            const int line = 5;
            const int column = 5;
            var expectedHoverContent = new MarkedStringsOrMarkupContent("123", "456", "789");

            var (client, server) = await Initialize(
                client => {
                    client.WithCapability(
                        new HoverCapability {
                            ContentFormat = new Container<MarkupKind>(MarkupKind.Markdown, MarkupKind.PlainText),
                        }
                    );
                },
                server => {
                    server.OnHover(
                        (request, token) => {
                            Assert.NotNull(request.TextDocument);

                            Assert.Equal(
                                AbsoluteDocumentPath,
                                DocumentUri.GetFileSystemPath(request.TextDocument.Uri)
                            );

                            Assert.Equal(line, request.Position.Line);
                            Assert.Equal(column, request.Position.Character);

                            return Task.FromResult(
                                new Hover {
                                    Contents = expectedHoverContent,
                                    Range = new Range {
                                        Start = request.Position,
                                        End = request.Position
                                    }
                                }
                            );
                        }, new HoverRegistrationOptions()
                    );
                }
            );

            var hover = await client.TextDocument.RequestHover(
                new HoverParams {
                    TextDocument = AbsoluteDocumentPath,
                    Position = ( line, column )
                }
            );

            Assert.NotNull(hover.Range);
            Assert.NotNull(hover.Range.Start);
            Assert.NotNull(hover.Range.End);

            Assert.Equal(line, hover.Range.Start.Line);
            Assert.Equal(column, hover.Range.Start.Character);

            Assert.Equal(line, hover.Range.End.Line);
            Assert.Equal(column, hover.Range.End.Character);

            Assert.NotNull(hover.Contents);
            Assert.True(expectedHoverContent.HasMarkedStrings);
            Assert.Equal(
                expectedHoverContent.MarkedStrings
                                    .Select(markedString => markedString.Value),
                hover.Contents.MarkedStrings.Select(
                    markedString => markedString.Value
                )
            );
        }

        /// <summary>
        /// Ensure that the language client can successfully request Completions.
        /// </summary>
        [Fact(DisplayName = "Language client can successfully request completions")]
        public async Task Completions_Success()
        {
            const int line = 5;
            const int column = 5;
            var expectedDocumentPath = AbsoluteDocumentPath;
            var expectedDocumentUri = DocumentUri.FromFileSystemPath(expectedDocumentPath);
            var expectedCompletionItems = new[] {
                new CompletionItem {
                    Kind = CompletionItemKind.Class,
                    Label = "Class1",
                    TextEdit = new TextEdit {
                        Range = ( ( line, column ), ( line, column ) ),
                        NewText = "Class1",
                    }
                }
            };

            var (client, server) = await Initialize(
                client => {
                    client.WithCapability(
                        new CompletionCapability {
                            CompletionItem = new CompletionItemCapabilityOptions {
                                DeprecatedSupport = true,
                                DocumentationFormat = new Container<MarkupKind>(MarkupKind.Markdown, MarkupKind.PlainText),
                                PreselectSupport = true,
                                SnippetSupport = true,
                                TagSupport = new CompletionItemTagSupportCapabilityOptions {
                                    ValueSet = new[] { CompletionItemTag.Deprecated }
                                },
                                CommitCharactersSupport = true
                            }
                        }
                    );
                },
                server => {
                    server.OnCompletion(
                        (request, cancellationToken) => {
                            Assert.NotNull(request.TextDocument);

                            Assert.Equal(expectedDocumentUri, request.TextDocument.Uri);

                            Assert.Equal(line, request.Position.Line);
                            Assert.Equal(column, request.Position.Character);

                            return Task.FromResult(
                                new CompletionList(
                                    expectedCompletionItems,
                                    true
                                )
                            );
                        }, new CompletionRegistrationOptions()
                    );
                }
            );

            var actualCompletions = await client.TextDocument.RequestCompletion(
                new CompletionParams {
                    TextDocument = AbsoluteDocumentPath,
                    Position = ( line, column ),
                }, CancellationToken
            );

            Assert.True(actualCompletions.IsIncomplete, "completions.IsIncomplete");
            Assert.NotNull(actualCompletions.Items);

            var actualCompletionItems = actualCompletions.Items.ToArray();
            Assert.Collection(
                actualCompletionItems, actualCompletionItem => {
                    var expectedCompletionItem = expectedCompletionItems[0];

                    Assert.Equal(expectedCompletionItem.Kind, actualCompletionItem.Kind);
                    Assert.Equal(expectedCompletionItem.Label, actualCompletionItem.Label);

                    Assert.NotNull(actualCompletionItem.TextEdit);
                    Assert.Equal(expectedCompletionItem.TextEdit.NewText, actualCompletionItem.TextEdit.NewText);

                    Assert.NotNull(actualCompletionItem.TextEdit.Range);
                    Assert.NotNull(actualCompletionItem.TextEdit.Range.Start);
                    Assert.NotNull(actualCompletionItem.TextEdit.Range.End);
                    Assert.Equal(
                        expectedCompletionItem.TextEdit.Range.Start.Line,
                        actualCompletionItem.TextEdit.Range.Start.Line
                    );
                    Assert.Equal(
                        expectedCompletionItem.TextEdit.Range.Start.Character,
                        actualCompletionItem.TextEdit.Range.Start.Character
                    );
                    Assert.Equal(
                        expectedCompletionItem.TextEdit.Range.End.Line,
                        actualCompletionItem.TextEdit.Range.End.Line
                    );
                    Assert.Equal(
                        expectedCompletionItem.TextEdit.Range.End.Character,
                        actualCompletionItem.TextEdit.Range.End.Character
                    );
                }
            );
        }

        /// <summary>
        /// Ensure that the language client can successfully request SignatureHelp.
        /// </summary>
        [Fact(DisplayName = "Language client can successfully request signature help")]
        public async Task SignatureHelp_Success()
        {
            const int line = 5;
            const int column = 5;
            var expectedDocumentPath = AbsoluteDocumentPath;
            var expectedDocumentUri = DocumentUri.FromFileSystemPath(expectedDocumentPath);

            var expectedSignatureHelp = new SignatureHelp {
                ActiveParameter = 0,
                ActiveSignature = 0,
                Signatures = new[] {
                    new SignatureInformation {
                        Documentation = new StringOrMarkupContent("test documentation"),
                        Label = "TestSignature",
                        Parameters = new[] {
                            new ParameterInformation {
                                Documentation = "test parameter documentation",
                                Label = "parameter label"
                            }
                        }
                    }
                }
            };

            var (client, server) = await Initialize(
                client => {
                    client.WithCapability(
                        new SignatureHelpCapability {
                            ContextSupport = true,
                            SignatureInformation = new SignatureInformationCapabilityOptions {
                                DocumentationFormat = new Container<MarkupKind>(MarkupKind.Markdown),
                                ParameterInformation = new SignatureParameterInformationCapabilityOptions {
                                    LabelOffsetSupport = true
                                }
                            }
                        }
                    );
                },
                server => {
                    server.OnSignatureHelp(
                        (request, cancellationToken) => {
                            Assert.NotNull(request.TextDocument);

                            Assert.Equal(expectedDocumentUri, request.TextDocument.Uri);

                            Assert.Equal(line, request.Position.Line);
                            Assert.Equal(column, request.Position.Character);

                            return Task.FromResult(expectedSignatureHelp);
                        }, new SignatureHelpRegistrationOptions()
                    );
                }
            );

            var actualSignatureHelp = await client.TextDocument.RequestSignatureHelp(
                new SignatureHelpParams {
                    TextDocument = AbsoluteDocumentPath,
                    Position = ( line, column ),
                }, CancellationToken
            );

            Assert.Equal(expectedSignatureHelp.ActiveParameter, actualSignatureHelp.ActiveParameter);
            Assert.Equal(expectedSignatureHelp.ActiveSignature, actualSignatureHelp.ActiveSignature);

            var actualSignatures = actualSignatureHelp.Signatures.ToArray();
            Assert.Collection(
                actualSignatures, actualSignature => {
                    var expectedSignature = expectedSignatureHelp.Signatures.ToArray()[0];

                    Assert.True(actualSignature.Documentation.HasString);
                    Assert.Equal(expectedSignature.Documentation.String, actualSignature.Documentation.String);

                    Assert.Equal(expectedSignature.Label, actualSignature.Label);

                    var expectedParameters = expectedSignature.Parameters.ToArray();
                    var actualParameters = actualSignature.Parameters.ToArray();

                    Assert.Collection(
                        actualParameters, actualParameter => {
                            var expectedParameter = expectedParameters[0];
                            Assert.True(actualParameter.Documentation.HasString);
                            Assert.Equal(expectedParameter.Documentation.String, actualParameter.Documentation.String);
                            Assert.Equal(expectedParameter.Label.Label, actualParameter.Label.Label);
                        }
                    );
                }
            );
        }

        /// <summary>
        /// Ensure that the language client can successfully request Definition.
        /// </summary>
        [Fact(DisplayName = "Language client can successfully request definition")]
        public async Task Definition_Success()
        {
            const int line = 5;
            const int column = 5;
            var expectedDocumentPath = AbsoluteDocumentPath;
            var expectedDocumentUri = DocumentUri.FromFileSystemPath(expectedDocumentPath);

            var expectedDefinitions = new LocationOrLocationLinks(
                new LocationOrLocationLink(
                    new Location {
                        Uri = expectedDocumentUri,
                        Range = ( ( line, column ), ( line, column ) ),
                    }
                )
            );

            var (client, server) = await Initialize(
                client => {
                    client.WithCapability(
                        new DefinitionCapability {
                            LinkSupport = true
                        }
                    );
                },
                server => {
                    server.OnDefinition(
                        (request, cancellationToken) => {
                            Assert.NotNull(request.TextDocument);

                            Assert.Equal(expectedDocumentUri, request.TextDocument.Uri);

                            Assert.Equal(line, request.Position.Line);
                            Assert.Equal(column, request.Position.Character);

                            return Task.FromResult(expectedDefinitions);
                        }, new DefinitionRegistrationOptions()
                    );
                }
            );

            var definitions = await client.TextDocument.RequestDefinition(
                new DefinitionParams {
                    TextDocument = AbsoluteDocumentPath,
                    Position = ( line, column ),
                }, CancellationToken
            );

            var actualDefinitions = definitions.ToArray();
            Assert.Collection(
                actualDefinitions, actualDefinition => {
                    var expectedDefinition = expectedDefinitions.Single();

                    Assert.NotNull(actualDefinition.Location);
                    Assert.Equal(expectedDefinition.Location.Uri, actualDefinition.Location.Uri);

                    Assert.NotNull(actualDefinition.Location.Range);
                    Assert.NotNull(actualDefinition.Location.Range.Start);
                    Assert.NotNull(actualDefinition.Location.Range.End);
                    Assert.Equal(expectedDefinition.Location.Range.Start.Line, actualDefinition.Location.Range.Start.Line);
                    Assert.Equal(
                        expectedDefinition.Location.Range.Start.Character,
                        actualDefinition.Location.Range.Start.Character
                    );
                    Assert.Equal(expectedDefinition.Location.Range.End.Line, actualDefinition.Location.Range.End.Line);
                    Assert.Equal(
                        expectedDefinition.Location.Range.End.Character,
                        actualDefinition.Location.Range.End.Character
                    );
                }
            );
        }

        /// <summary>
        /// Ensure that the language client can successfully request DocumentHighlight.
        /// </summary>
        [Fact(DisplayName = "Language client can successfully request document highlights")]
        public async Task DocumentHighlights_Success()
        {
            const int line = 5;
            const int column = 5;
            var expectedDocumentPath = AbsoluteDocumentPath;
            var expectedDocumentUri = DocumentUri.FromFileSystemPath(expectedDocumentPath);

            var expectedHighlights = new DocumentHighlightContainer(
                new DocumentHighlight {
                    Kind = DocumentHighlightKind.Write,
                    Range = ( ( line, column ), ( line, column ) ),
                }
            );

            var (client, server) = await Initialize(
                client => {
                    client.WithCapability(
                        new DocumentHighlightCapability()
                    );
                },
                server => {
                    server.OnDocumentHighlight(
                        (request, cancellationToken) => {
                            Assert.NotNull(request.TextDocument);

                            Assert.Equal(expectedDocumentUri, request.TextDocument.Uri);

                            Assert.Equal(line, request.Position.Line);
                            Assert.Equal(column, request.Position.Character);

                            return Task.FromResult(expectedHighlights);
                        }, new DocumentHighlightRegistrationOptions()
                    );
                }
            );

            var definitions = await client.TextDocument.RequestDocumentHighlight(
                new DocumentHighlightParams {
                    TextDocument = AbsoluteDocumentPath,
                    Position = ( line, column ),
                }, CancellationToken
            );

            var actualDefinitions = definitions.ToArray();
            Assert.Collection(
                actualDefinitions, actualHighlight => {
                    var expectedHighlight = expectedHighlights.Single();

                    Assert.Equal(DocumentHighlightKind.Write, expectedHighlight.Kind);

                    Assert.NotNull(actualHighlight.Range);
                    Assert.NotNull(actualHighlight.Range.Start);
                    Assert.NotNull(actualHighlight.Range.End);
                    Assert.Equal(expectedHighlight.Range.Start.Line, actualHighlight.Range.Start.Line);
                    Assert.Equal(expectedHighlight.Range.Start.Character, actualHighlight.Range.Start.Character);
                    Assert.Equal(expectedHighlight.Range.End.Line, actualHighlight.Range.End.Line);
                    Assert.Equal(expectedHighlight.Range.End.Character, actualHighlight.Range.End.Character);
                }
            );
        }

        /// <summary>
        /// Ensure that the language client can successfully request DocumentHighlight.
        /// </summary>
        [Fact(DisplayName = "Language client can successfully request document symbols")]
        public async Task DocumentSymbols_DocumentSymbol_Success()
        {
            const int line = 5;
            const int character = 6;
            var expectedDocumentPath = AbsoluteDocumentPath;
            var expectedDocumentUri = DocumentUri.FromFileSystemPath(expectedDocumentPath);
            var detail = "some detail";

            var documentSymbol = new DocumentSymbol {
                Detail = detail,
                Kind = SymbolKind.Class,
                Range = new Range(new Position(line, character), new Position(line, character))
            };
            var expectedSymbols = new SymbolInformationOrDocumentSymbolContainer(
                new SymbolInformationOrDocumentSymbol(documentSymbol)
            );

            var (client, server) = await Initialize(
                client => {
                    client.WithCapability(
                        new DocumentSymbolCapability {
                            DynamicRegistration = true,
                            SymbolKind = new SymbolKindCapabilityOptions {
                                ValueSet = new Container<SymbolKind>(
                                    Enum.GetValues(typeof(SymbolKind)).Cast<SymbolKind>()
                                        .ToArray()
                                )
                            },
                            TagSupport = new TagSupportCapabilityOptions {
                                ValueSet = new[] { SymbolTag.Deprecated }
                            },
                            HierarchicalDocumentSymbolSupport = true
                        }
                    );
                },
                server => {
                    server.OnDocumentSymbol(
                        (request, cancellationToken) => {
                            Assert.NotNull(request.TextDocument);

                            Assert.Equal(expectedDocumentUri, request.TextDocument.Uri);

                            return Task.FromResult(expectedSymbols);
                        }, new DocumentSymbolRegistrationOptions()
                    );
                }
            );

            var symbols = await client.TextDocument.RequestDocumentSymbol(
                new DocumentSymbolParams {
                    TextDocument = expectedDocumentUri
                }, CancellationToken
            );

            var actualSymbols = symbols.ToArray();
            Assert.Collection(
                actualSymbols, actualSymbol => {
                    var expectedSymbol = expectedSymbols.Single();

                    Assert.True(expectedSymbol.IsDocumentSymbol);

                    Assert.NotNull(actualSymbol.DocumentSymbol);
                    Assert.Equal(expectedSymbol.DocumentSymbol.Detail, actualSymbol.DocumentSymbol.Detail);
                    Assert.Equal(expectedSymbol.DocumentSymbol.Kind, actualSymbol.DocumentSymbol.Kind);
                    Assert.Equal(
                        expectedSymbol.DocumentSymbol.Range.Start.Line,
                        actualSymbol.DocumentSymbol.Range.Start.Line
                    );
                    Assert.Equal(
                        expectedSymbol.DocumentSymbol.Range.Start.Character,
                        actualSymbol.DocumentSymbol.Range.Start.Character
                    );
                    Assert.Equal(expectedSymbol.DocumentSymbol.Range.End.Line, actualSymbol.DocumentSymbol.Range.End.Line);
                    Assert.Equal(
                        expectedSymbol.DocumentSymbol.Range.End.Character,
                        actualSymbol.DocumentSymbol.Range.End.Character
                    );
                }
            );
        }

        /// <summary>
        /// Ensure that the language client can successfully request FoldingRanges.
        /// </summary>
        [Fact(DisplayName = "Language client can successfully request document folding ranges")]
        public async Task FoldingRanges_Success()
        {
            var expectedDocumentPath = AbsoluteDocumentPath;
            var expectedDocumentUri = DocumentUri.FromFileSystemPath(expectedDocumentPath);

            var expectedFoldingRanges = new Container<FoldingRange>(
                new FoldingRange {
                    Kind = FoldingRangeKind.Region,
                    StartLine = 5,
                    StartCharacter = 1,
                    EndLine = 7,
                    EndCharacter = 2,
                }
            );

            var (client, server) = await Initialize(
                client => {
                    client.WithCapability(
                        new FoldingRangeCapability {
                            RangeLimit = 100,
                            LineFoldingOnly = true
                        }
                    );
                },
                server => {
                    server.OnFoldingRange(
                        (request, cancellationToken) => {
                            Assert.NotNull(request.TextDocument);
                            Assert.Equal(expectedDocumentUri, request.TextDocument.Uri);
                            return Task.FromResult(expectedFoldingRanges);
                        }, new FoldingRangeRegistrationOptions()
                    );
                }
            );

            var foldingRanges = await client.TextDocument.RequestFoldingRange(
                new FoldingRangeRequestParam {
                    TextDocument = AbsoluteDocumentPath
                }, CancellationToken
            );

            var actualFoldingRanges = foldingRanges.ToArray();
            Assert.Collection(
                actualFoldingRanges, actualFoldingRange => {
                    var expectedFoldingRange = expectedFoldingRanges.Single();

                    Assert.Equal(FoldingRangeKind.Region, expectedFoldingRange.Kind);

                    Assert.Equal(expectedFoldingRange.StartLine, actualFoldingRange.StartLine);
                    Assert.Equal(expectedFoldingRange.StartCharacter, actualFoldingRange.StartCharacter);
                    Assert.Equal(expectedFoldingRange.EndLine, actualFoldingRange.EndLine);
                    Assert.Equal(expectedFoldingRange.EndCharacter, actualFoldingRange.EndCharacter);
                }
            );
        }

        /// <summary>
        /// Ensure that the language client can successfully receive Diagnostics from the server.
        /// </summary>
        [Fact(DisplayName = "Language client can successfully receive diagnostics")]
        public async Task Diagnostics_Success()
        {
            var documentPath = AbsoluteDocumentPath;
            var expectedDocumentUri = DocumentUri.FromFileSystemPath(documentPath);
            var expectedDiagnostics = new List<Diagnostic> {
                new Diagnostic {
                    Source = "Test",
                    Code = new DiagnosticCode(1234),
                    Message = "This is a diagnostic message.",
                    Range = new Range {
                        Start = new Position {
                            Line = 2,
                            Character = 5
                        },
                        End = new Position {
                            Line = 3,
                            Character = 7
                        }
                    },
                    Severity = DiagnosticSeverity.Warning
                }
            };

            var receivedDiagnosticsNotification = new TaskCompletionSource<object>();

            DocumentUri actualDocumentUri = null;
            List<Diagnostic> actualDiagnostics = null;

            var (client, server) = await Initialize(
                client => {
                    client.OnPublishDiagnostics(
                        request => {
                            actualDocumentUri = request.Uri;
                            actualDiagnostics = request.Diagnostics.ToList();

                            receivedDiagnosticsNotification.TrySetResult(null);
                            return Unit.Task;
                        }
                    );
                },
                server => { }
            );

            server.TextDocument.PublishDiagnostics(
                new PublishDiagnosticsParams {
                    Uri = DocumentUri.FromFileSystemPath(documentPath),
                    Diagnostics = expectedDiagnostics
                }
            );

            CancellationToken.Register(() => receivedDiagnosticsNotification.TrySetCanceled());

            // Timeout.
            var winner = await Task.WhenAny(
                receivedDiagnosticsNotification.Task,
                Task.Delay(
                    TimeSpan.FromSeconds(2),
                    CancellationToken
                )
            );
            Assert.Same(receivedDiagnosticsNotification.Task, winner);

            Assert.NotNull(actualDocumentUri);
            Assert.Equal(expectedDocumentUri, actualDocumentUri);

            Assert.NotNull(actualDiagnostics);
            Assert.Equal(1, actualDiagnostics.Count);

            var expectedDiagnostic = expectedDiagnostics[0];
            var actualDiagnostic = actualDiagnostics[0];

            Assert.Equal(expectedDiagnostic.Code, actualDiagnostic.Code);
            Assert.Equal(expectedDiagnostic.Message, actualDiagnostic.Message);
            Assert.Equal(expectedDiagnostic.Range.Start.Line, actualDiagnostic.Range.Start.Line);
            Assert.Equal(expectedDiagnostic.Range.Start.Character, actualDiagnostic.Range.Start.Character);
            Assert.Equal(expectedDiagnostic.Range.End.Line, actualDiagnostic.Range.End.Line);
            Assert.Equal(expectedDiagnostic.Range.End.Character, actualDiagnostic.Range.End.Character);
            Assert.Equal(expectedDiagnostic.Severity, actualDiagnostic.Severity);
            Assert.Equal(expectedDiagnostic.Source, actualDiagnostic.Source);
        }
    }
}
