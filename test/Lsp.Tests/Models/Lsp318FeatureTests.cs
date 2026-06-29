using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json.Linq;
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

            var json = JObject.Parse(Fixture.SerializeObject(model));

            json["items"]!.Should().HaveCount(1);
            json["items"]![0]!["insertText"]!["kind"]!.Value<string>().Should().Be("snippet");
            json["items"]![0]!["insertText"]!["value"]!.Value<string>().Should().Be("Console.WriteLine($1);$0");
            json["items"]![0]!["command"]!["tooltip"]!.Value<string>().Should().Be("Run after insertion");

            var result = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<InlineCompletionList>(json.ToString());
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

            var json = JObject.Parse(Fixture.SerializeObject(model));

            json["applyKind"]!["commitCharacters"]!.Value<string>().Should().Be("merge");
            json["applyKind"]!["data"]!.Value<string>().Should().Be("replace");

            var result = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<CompletionList>(json.ToString());
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

            var json = JObject.Parse(Fixture.SerializeObject(model));

            json["metadata"]!["isRefactoring"]!.Value<bool>().Should().BeTrue();
            json["documentChanges"]![0]!["edits"]![0]!["snippet"]!["kind"]!.Value<string>().Should().Be("snippet");
            json["documentChanges"]![0]!["edits"]![0]!["snippet"]!["value"]!.Value<string>().Should().Be("class ${1:Name} {$0}");
            json["documentChanges"]![0]!["edits"]![0]!["annotationId"]!.Value<string>().Should().Be("snippet-edit");

            var result = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<WorkspaceEdit>(json.ToString());
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

            var json = JObject.Parse(Fixture.SerializeObject(capabilities));

            json["workspace"]!["workspaceEdit"]!["metadataSupport"]!.Value<bool>().Should().BeTrue();
            json["workspace"]!["workspaceEdit"]!["snippetEditSupport"]!.Value<bool>().Should().BeTrue();
            json["workspace"]!["foldingRange"]!["refreshSupport"]!.Value<bool>().Should().BeTrue();
            json["workspace"]!["textDocumentContent"]!["dynamicRegistration"]!.Value<bool>().Should().BeTrue();
            json["textDocument"]!["filters"]!["relativePatternSupport"]!.Value<bool>().Should().BeTrue();
            json["textDocument"]!["inlineCompletion"]!["dynamicRegistration"]!.Value<bool>().Should().BeTrue();
            json["textDocument"]!["rangeFormatting"]!["rangesSupport"]!.Value<bool>().Should().BeTrue();
            json["textDocument"]!["completion"]!["completionList"]!["applyKindSupport"]!.Value<bool>().Should().BeTrue();
            json["textDocument"]!["codeAction"]!["documentationSupport"]!.Value<bool>().Should().BeTrue();
            json["textDocument"]!["codeLens"]!["resolveSupport"]!["properties"]!.Values<string>().Should().Contain("command");
            json["textDocument"]!["signatureHelp"]!["signatureInformation"]!["noActiveParameterSupport"]!.Value<bool>().Should().BeTrue();
            json["textDocument"]!["diagnostic"]!["markupMessageSupport"]!.Value<bool>().Should().BeTrue();
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

            var json = JObject.Parse(Fixture.SerializeObject(capabilities));

            json["inlineCompletionProvider"]!["workDoneProgress"]!.Value<bool>().Should().BeTrue();
            json["documentRangeFormattingProvider"]!["rangesSupport"]!.Value<bool>().Should().BeTrue();
            json["workspace"]!["textDocumentContent"]!["schemes"]!.Values<string>().Should().Contain(new[] { "git", "vscode-notebook-cell" });
            json["codeActionProvider"]!["documentation"]![0]!["kind"]!.Value<string>().Should().Be("refactor.move");
            json["codeActionProvider"]!["documentation"]![0]!["command"]!["tooltip"]!.Value<string>().Should().Be("Learn about move refactorings");
        }

        [Fact]
        public void Enumerations_Expose318Values()
        {
            ((int)MessageType.Debug).Should().Be(5);
            CodeActionKind.RefactorMove.ToString().Should().Be("refactor.move");
            CodeActionKind.Notebook.ToString().Should().Be("notebook");
            SemanticTokenType.Label.ToString().Should().Be("label");
        }
    }
}
