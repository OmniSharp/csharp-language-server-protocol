using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class NotebookDocumentChangeEventTests
    {
        [Fact]
        public void Deserializes_Text_Content_As_Array()
        {
            const string expected = """
            {
                "cells": {
                    "textContent": [
                        {
                            "document": {
                                "uri": "file:///notebook/cell1.cs",
                                "version": 2
                            },
                            "changes": [
                                {
                                    "text": "one"
                                }
                            ]
                        },
                        {
                            "document": {
                                "uri": "file:///notebook/cell2.cs",
                                "version": 3
                            },
                            "changes": [
                                {
                                    "text": "two"
                                }
                            ]
                        }
                    ]
                }
            }
            """;

            var result = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<NotebookDocumentChangeEvent>(expected);

            result.Cells.Should().NotBeNull();
            result.Cells!.TextContent.Should().NotBeNull();
            result.Cells.TextContent!.ToArray().Should().HaveCount(2);
            JsonNode.DeepEquals(JsonNode.Parse(Fixture.SerializeObject(result)), JsonNode.Parse(expected)).Should().BeTrue();
        }

        [Fact]
        public void Serializes_Text_Content_As_Array()
        {
            var model = new NotebookDocumentChangeEvent
            {
                Cells = new NotebookDocumentChangeEventCells
                {
                    TextContent = new Container<NotebookDocumentChangeEventCellsTextContent>(
                        new NotebookDocumentChangeEventCellsTextContent
                        {
                            Document = new VersionedTextDocumentIdentifier
                            {
                                Uri = new Uri("file:///notebook/cell1.cs"),
                                Version = 2
                            },
                            Changes = new Container<TextDocumentContentChangeEvent>(
                                new TextDocumentContentChangeEvent
                                {
                                    Text = "one"
                                }
                            )
                        },
                        new NotebookDocumentChangeEventCellsTextContent
                        {
                            Document = new VersionedTextDocumentIdentifier
                            {
                                Uri = new Uri("file:///notebook/cell2.cs"),
                                Version = 3
                            },
                            Changes = new Container<TextDocumentContentChangeEvent>(
                                new TextDocumentContentChangeEvent
                                {
                                    Text = "two"
                                }
                            )
                        }
                    )
                }
            };

            using var resultDoc = JsonDocument.Parse(Fixture.SerializeObject(model));
            var result = resultDoc.RootElement;

            result.GetProperty("cells").GetProperty("textContent").ValueKind.Should().Be(JsonValueKind.Array);
            result.GetProperty("cells").GetProperty("textContent").GetArrayLength().Should().Be(2);
        }

        [Fact]
        public void Omits_Cells_When_Only_Metadata_Changes()
        {
            var model = new NotebookDocumentChangeEvent
            {
                Metadata = JsonSerializer.SerializeToElement(new { custom = true })
            };

            using var resultDoc = JsonDocument.Parse(Fixture.SerializeObject(model));
            var result = resultDoc.RootElement;

            result.GetProperty("metadata").GetProperty("custom").GetBoolean().Should().BeTrue();
            result.TryGetProperty("cells", out _).Should().BeFalse();
        }
    }
}
