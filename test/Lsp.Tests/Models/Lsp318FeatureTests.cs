using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using TestingUtils;
using Xunit;

namespace Lsp.Tests.Models
{
    public class Lsp318FeatureTests
    {
        [Fact]
        public void InlineCompletionList_SupportsItemsAndSnippetInsertText()
        {
            var model = new InlineCompletionList(
                new[] {
                    new InlineCompletionItem {
                        InsertText = new StringValue { Value = "Console.WriteLine($1);$0" },
                        FilterText = "cw",
                        Range = new Range(new Position(1, 2), new Position(1, 4)),
                        Command = new Command { Title = "after", Name = "inline.after", Tooltip = "Run after insertion" }
                    }
                }
            );

            var jsonStr = Fixture.SerializeObject(model);
            using var doc = JsonDocument.Parse(jsonStr);
            var json = doc.RootElement;

            var items = json.GetProperty("items").EnumerateArray().ToArray();
            items.Should().HaveCount(1);
            items[0].GetProperty("insertText").GetProperty("kind").GetString().Should().Be("snippet");
            items[0].GetProperty("insertText").GetProperty("value").GetString().Should().Be("Console.WriteLine($1);$0");
            items[0].GetProperty("command").GetProperty("tooltip").GetString().Should().Be("Run after insertion");

            var result = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<InlineCompletionList>(jsonStr);
            result.Items.Single().InsertText.StringValue!.Value.Should().Be("Console.WriteLine($1);$0");
        }

        [Fact]
        public void InlineCompletionList_DeserializesArrayResultShape()
        {
            var json = """[{ "insertText": "hello", "filterText": "h" }]""";

            var result = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<InlineCompletionList>(json);

            result.Items.Single().InsertText.String.Should().Be("hello");
            result.Items.Single().FilterText.Should().Be("h");
        }

        [Fact]
        public void CompletionList_SerializesApplyKind()
        {
            var model = new CompletionList(new[] { new CompletionItem { Label = "abc" } }, true) {
                ApplyKind = new CompletionItemApplyKinds {
                    CommitCharacters = ApplyKind.Merge,
                    Data = ApplyKind.Replace
                }
            };

            var jsonStr = Fixture.SerializeObject(model);
            using var doc = JsonDocument.Parse(jsonStr);
            var json = doc.RootElement;

            json.GetProperty("applyKind").GetProperty("commitCharacters").GetString().Should().Be("merge");
            json.GetProperty("applyKind").GetProperty("data").GetString().Should().Be("replace");

            var result = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<CompletionList>(jsonStr);
            result.ApplyKind!.CommitCharacters.Should().Be(ApplyKind.Merge);
            result.ApplyKind!.Data.Should().Be(ApplyKind.Replace);
        }

        [Fact]
        public void WorkspaceEdit_SerializesMetadataAndSnippetTextEdit()
        {
            var model = new WorkspaceEdit {
                Metadata = new WorkspaceEditMetadata { IsRefactoring = true },
                DocumentChanges = new Container<WorkspaceEditDocumentChange>(
                    new TextDocumentEdit {
                        TextDocument = new OptionalVersionedTextDocumentIdentifier {
                            Uri = new Uri("file:///workspace/test.cs"),
                            Version = 1
                        },
                        Edits = new TextEditContainer(
                            new SnippetTextEdit {
                                Range = new Range(new Position(0, 0), new Position(0, 0)),
                                Snippet = new StringValue { Value = "class ${1:Name} {$0}" },
                                AnnotationId = "snippet-edit"
                            }
                        )
                    }
                )
            };

            var jsonStr = Fixture.SerializeObject(model);
            using var doc = JsonDocument.Parse(jsonStr);
            var json = doc.RootElement;

            json.GetProperty("metadata").GetProperty("isRefactoring").GetBoolean().Should().BeTrue();
            json.GetProperty("documentChanges")[0].GetProperty("edits")[0].GetProperty("snippet").GetProperty("kind").GetString().Should().Be("snippet");
            json.GetProperty("documentChanges")[0].GetProperty("edits")[0].GetProperty("snippet").GetProperty("value").GetString().Should().Be("class ${1:Name} {$0}");
            json.GetProperty("documentChanges")[0].GetProperty("edits")[0].GetProperty("annotationId").GetString().Should().Be("snippet-edit");

            var result = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<WorkspaceEdit>(jsonStr);
            result.Metadata!.IsRefactoring.Should().BeTrue();
            result.DocumentChanges!.Single().TextDocumentEdit!.Edits.Single().Should().BeOfType<SnippetTextEdit>();
        }

        [Fact]
        public void Capabilities_Serialize318Fields()
        {
            var capabilities = new ClientCapabilities {
                Workspace = new WorkspaceClientCapabilities {
                    WorkspaceEdit = new WorkspaceEditCapability {
                        MetadataSupport = true,
                        SnippetEditSupport = true
                    },
                    FoldingRange = new FoldingRangeWorkspaceClientCapabilities { RefreshSupport = true },
                    TextDocumentContent = new TextDocumentContentClientCapabilities { DynamicRegistration = true }
                },
                TextDocument = new TextDocumentClientCapabilities {
                    Filters = new TextDocumentFilterClientCapabilities { RelativePatternSupport = true },
                    InlineCompletion = new InlineCompletionClientCapabilities { DynamicRegistration = true },
                    RangeFormatting = new DocumentRangeFormattingCapability { RangesSupport = true },
                    Completion = new CompletionCapability {
                        CompletionList = new CompletionListCapabilityOptions { ApplyKindSupport = true }
                    },
                    CodeAction = new CodeActionCapability { DocumentationSupport = true },
                    CodeLens = new CodeLensCapability {
                        ResolveSupport = new CodeLensCapabilityResolveSupport { Properties = new Container<string>("command") }
                    },
                    SignatureHelp = new SignatureHelpCapability {
                        SignatureInformation = new SignatureInformationCapabilityOptions { NoActiveParameterSupport = true }
                    },
                    Diagnostic = new DiagnosticClientCapabilities { MarkupMessageSupport = true }
                }
            };

            using var capDoc = JsonDocument.Parse(Fixture.SerializeObject(capabilities));
            var json = capDoc.RootElement;

            json.GetProperty("workspace").GetProperty("workspaceEdit").GetProperty("metadataSupport").GetBoolean().Should().BeTrue();
            json.GetProperty("workspace").GetProperty("workspaceEdit").GetProperty("snippetEditSupport").GetBoolean().Should().BeTrue();
            json.GetProperty("workspace").GetProperty("foldingRange").GetProperty("refreshSupport").GetBoolean().Should().BeTrue();
            json.GetProperty("workspace").GetProperty("textDocumentContent").GetProperty("dynamicRegistration").GetBoolean().Should().BeTrue();
            json.GetProperty("textDocument").GetProperty("filters").GetProperty("relativePatternSupport").GetBoolean().Should().BeTrue();
            json.GetProperty("textDocument").GetProperty("inlineCompletion").GetProperty("dynamicRegistration").GetBoolean().Should().BeTrue();
            json.GetProperty("textDocument").GetProperty("rangeFormatting").GetProperty("rangesSupport").GetBoolean().Should().BeTrue();
            json.GetProperty("textDocument").GetProperty("completion").GetProperty("completionList").GetProperty("applyKindSupport").GetBoolean().Should().BeTrue();
            json.GetProperty("textDocument").GetProperty("codeAction").GetProperty("documentationSupport").GetBoolean().Should().BeTrue();
            json.GetProperty("textDocument").GetProperty("codeLens").GetProperty("resolveSupport").GetProperty("properties").EnumerateArray().Select(x => x.GetString()).Should().Contain("command");
            json.GetProperty("textDocument").GetProperty("signatureHelp").GetProperty("signatureInformation").GetProperty("noActiveParameterSupport").GetBoolean().Should().BeTrue();
            json.GetProperty("textDocument").GetProperty("diagnostic").GetProperty("markupMessageSupport").GetBoolean().Should().BeTrue();
        }

        [Fact]
        public void ServerCapabilities_Serialize318Providers()
        {
            var capabilities = new ServerCapabilities {
                InlineCompletionProvider = new InlineCompletionRegistrationOptions.StaticOptions {
                    WorkDoneProgress = true
                },
                DocumentRangeFormattingProvider = new DocumentRangeFormattingRegistrationOptions.StaticOptions {
                    RangesSupport = true
                },
                Workspace = new WorkspaceServerCapabilities {
                    TextDocumentContent = new TextDocumentContentRegistrationOptions.StaticOptions {
                        Schemes = new Container<string>("git", "vscode-notebook-cell")
                    }
                },
                CodeActionProvider = new CodeActionRegistrationOptions.StaticOptions {
                    Documentation = new Container<CodeActionKindDocumentation>(
                        new CodeActionKindDocumentation {
                            Kind = CodeActionKind.RefactorMove,
                            Command = new Command { Title = "Move help", Name = "help.move", Tooltip = "Learn about move refactorings" }
                        }
                    )
                }
            };

            using var srvDoc = JsonDocument.Parse(Fixture.SerializeObject(capabilities));
            var json = srvDoc.RootElement;

            json.GetProperty("inlineCompletionProvider").GetProperty("workDoneProgress").GetBoolean().Should().BeTrue();
            json.GetProperty("documentRangeFormattingProvider").GetProperty("rangesSupport").GetBoolean().Should().BeTrue();
            json.GetProperty("workspace").GetProperty("textDocumentContent").GetProperty("schemes").EnumerateArray().Select(x => x.GetString()).Should().Contain(new[] { "git", "vscode-notebook-cell" });
            json.GetProperty("codeActionProvider").GetProperty("documentation").EnumerateArray().First().GetProperty("kind").GetString().Should().Be("refactor.move");
            json.GetProperty("codeActionProvider").GetProperty("documentation").EnumerateArray().First().GetProperty("command").GetProperty("tooltip").GetString().Should().Be("Learn about move refactorings");
        }

        [Fact]
        public void Enumerations_Expose318Values()
        {
            ((int)MessageType.Debug).Should().Be(5);
            CodeActionKind.RefactorMove.ToString().Should().Be("refactor.move");
            CodeActionKind.Notebook.ToString().Should().Be("notebook");
            SemanticTokenType.Label.ToString().Should().Be("label");
            LanguageKind.D.ToString().Should().Be("d");
            LanguageKind.Delphi.ToString().Should().Be("pascal");
            LanguageKind.Pascal.ToString().Should().Be("pascal");
        }

        [Fact]
        public void ProtocolMethods_ExposeAll318Requests()
        {
            TextDocumentNames.InlineCompletion.Should().Be("textDocument/inlineCompletion");
            WorkspaceNames.TextDocumentContent.Should().Be("workspace/textDocumentContent");
            WorkspaceNames.TextDocumentContentRefresh.Should().Be("workspace/textDocumentContent/refresh");
            WorkspaceNames.FoldingRangeRefresh.Should().Be("workspace/foldingRange/refresh");
            TextDocumentNames.RangesFormatting.Should().Be("textDocument/rangesFormatting");
        }

        [Fact]
        public void CodeAction_Serializes318TagsAndCapability()
        {
            var action = new CodeAction {
                Title = "Generated fix",
                Tags = new Container<CodeActionTag>(CodeActionTag.LLMGenerated)
            };
            var capabilities = new CodeActionCapability {
                TagSupport = new CodeActionTagSupportOptions {
                    ValueSet = new Container<CodeActionTag>(CodeActionTag.LLMGenerated)
                }
            };

            using var actionDocument = JsonDocument.Parse(Fixture.SerializeObject(action));
            using var capabilityDocument = JsonDocument.Parse(Fixture.SerializeObject(capabilities));

            actionDocument.RootElement.GetProperty("tags")[0].GetInt32().Should().Be(1);
            capabilityDocument.RootElement.GetProperty("tagSupport").GetProperty("valueSet")[0].GetInt32().Should().Be(1);
        }

        [Fact]
        public void SignatureHelp_SerializesNullActiveParameters()
        {
            var signatureHelp = new SignatureHelp {
                Signatures = new Container<SignatureInformation>(
                    new SignatureInformation {
                        Label = "M(value)",
                        ActiveParameter = null
                    }
                ),
                ActiveParameter = null
            };

            using var document = JsonDocument.Parse(Fixture.SerializeObject(signatureHelp));

            document.RootElement.GetProperty("activeParameter").ValueKind.Should().Be(JsonValueKind.Null);
            document.RootElement.GetProperty("signatures")[0].GetProperty("activeParameter").ValueKind.Should().Be(JsonValueKind.Null);
        }

        [Fact]
        public void SignatureHelp_OmitsUnsetActiveParameters()
        {
            var signatureHelp = new SignatureHelp {
                Signatures = new Container<SignatureInformation>(
                    new SignatureInformation { Label = "M()" }
                )
            };

            using var document = JsonDocument.Parse(Fixture.SerializeObject(signatureHelp));

            document.RootElement.TryGetProperty("activeParameter", out _).Should().BeFalse();
            document.RootElement.GetProperty("signatures")[0].TryGetProperty("activeParameter", out _).Should().BeFalse();
        }

        [Fact]
        public void DocumentFilters_Serialize318RelativePatterns()
        {
            DocumentUri baseUri = new Uri("file:///workspace/");
            var textFilter = new TextDocumentFilter {
                Language = "csharp",
                Pattern = new RelativePattern { BaseUri = baseUri, Pattern = "**/*.cs" }
            };
            var notebookFilter = new NotebookDocumentFilter {
                NotebookType = "jupyter-notebook",
                Pattern = new RelativePattern { BaseUri = baseUri, Pattern = "**/*.ipynb" }
            };

            using var textDocument = JsonDocument.Parse(Fixture.SerializeObject(textFilter));
            using var notebookDocument = JsonDocument.Parse(Fixture.SerializeObject(notebookFilter));

            textDocument.RootElement.GetProperty("pattern").GetProperty("baseUri").GetString().Should().Be("file:///workspace/");
            textDocument.RootElement.GetProperty("pattern").GetProperty("pattern").GetString().Should().Be("**/*.cs");
            notebookDocument.RootElement.GetProperty("pattern").GetProperty("baseUri").GetString().Should().Be("file:///workspace/");
            notebookDocument.RootElement.GetProperty("pattern").GetProperty("pattern").GetString().Should().Be("**/*.ipynb");
        }
    }
}
