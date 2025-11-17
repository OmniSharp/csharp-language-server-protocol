using System.Diagnostics;
using System.Text;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit.Abstractions;


#pragma warning disable 618

namespace Lsp.Tests
{
    public class SemanticTokensDocumentTests
    {
        private readonly ILogger _logger;
        private readonly SemanticTokensLegend _legend;

        public SemanticTokensDocumentTests(ITestOutputHelper testOutputHelper)
        {
            var loggerFactory = new TestLoggerFactory(testOutputHelper);
            _logger = loggerFactory.CreateLogger<SemanticTokensDocumentTests>();
            _legend = new SemanticTokensLegend
            {
                // specify a specific set so that additions to the default list do not cause breaks in the tests.
                TokenModifiers = new[]
                    {
                        new SemanticTokenModifier("documentation"),
                        new SemanticTokenModifier("declaration"),
                        new SemanticTokenModifier("definition"),
                        new SemanticTokenModifier("static"),
                        new SemanticTokenModifier("async"),
                        new SemanticTokenModifier("abstract"),
                        new SemanticTokenModifier("deprecated"),
                        new SemanticTokenModifier("readonly"),
                        new SemanticTokenModifier("modification"),
                        new SemanticTokenModifier("defaultLibrary")
                    }
                   .ToArray(),
                TokenTypes = new[]
                    {
                        new SemanticTokenType("comment"),
                        new SemanticTokenType("keyword"),
                        new SemanticTokenType("string"),
                        new SemanticTokenType("number"),
                        new SemanticTokenType("regexp"),
                        new SemanticTokenType("operator"),
                        new SemanticTokenType("namespace"),
                        new SemanticTokenType("type"),
                        new SemanticTokenType("struct"),
                        new SemanticTokenType("class"),
                        new SemanticTokenType("interface"),
                        new SemanticTokenType("enum"),
                        new SemanticTokenType("typeParameter"),
                        new SemanticTokenType("function"),
                        new SemanticTokenType("member"),
                        new SemanticTokenType("property"),
                        new SemanticTokenType("macro"),
                        new SemanticTokenType("variable"),
                        new SemanticTokenType("parameter"),
                        new SemanticTokenType("label"),
                        new SemanticTokenType("modifier"),
                        new SemanticTokenType("event"),
                        new SemanticTokenType("enumMember"),
                    }
                   .ToArray(),
            };
        }

        [Fact]
        public void ReturnDocumentTokensFromScratch()
        {
            var document = new SemanticTokensDocument(_legend);

            {
                var builder = document.Create();
                Tokenize(ExampleDocumentText, builder);
                builder.Commit();
            }

            var result = document.GetSemanticTokens();
            result.ResultId.Should().Be(document.Id);
            var data = Normalize(ExampleDocumentText, result.Data).ToArray();
            _logger.LogInformation("Some Data {Data}", data.AsEnumerable());
            var expectedResponse = new NormalizedToken[]
            {
                "using (macro:async|deprecated)", "System (macro:async|deprecated)", "using (event:none)", "System (number:none)",
                "Collections (struct:readonly)",
                "Generic (class:none)", "using (variable:modification|defaultLibrary)", "System (comment:static|deprecated)", "Linq (comment:definition)",
                "using (comment:none)",
                "System (enumMember:none)", "Text (comment:static|deprecated)", "using (comment:none)", "System (comment:declaration)",
                "Threading (comment:static|defaultLibrary)",
                "Tasks (comment:none)", "namespace (comment:readonly)", "CSharpTutorials (comment:none)", "{ (struct:documentation)", "class (enumMember:none)",
                "Program (comment:none)", "{ (comment:none)", "static (regexp:documentation)", "void (macro:none)", "Main (macro:none)",
                "string[] (property:declaration|abstract)",
                "args (macro:none)", "{ (interface:documentation|declaration|deprecated)", "string (struct:none)", "message (enum:none)", "= (label:none)",
                "Hello (comment:none)",
                "World!! (enum:none)", "Console (interface:static)", "WriteLine (event:async|modification)", "message (interface:static)", "} (operator:none)",
                "} (enum:async|deprecated)", "} (function:declaration|async)"
            };
            data.Should().ContainInOrder(expectedResponse);
        }

        [Theory]
        [ClassData(typeof(ReturnDocumentTokensFromScratchForRangeData))]
        public void ReturnDocumentTokensFromScratch_ForRange(Range range, IEnumerable<NormalizedToken> expectedTokens)
        {
            var document = new SemanticTokensDocument(_legend);

            {
                var builder = document.Create();
                Tokenize(ExampleDocumentText, builder);
                builder.Commit();
            }

            var result = document.GetSemanticTokens(range);
            result.ResultId.Should().Be(document.Id);
            var data = Normalize(ExtractRange(ExampleDocumentText, range), result.Data).ToArray();
            _logger.LogDebug("Some Data {Data}", data.AsEnumerable());
            data.Should().ContainInOrder(expectedTokens);
        }

        public class ReturnDocumentTokensFromScratchForRangeData : TheoryData<Range, IEnumerable<NormalizedToken>>
        {
            public ReturnDocumentTokensFromScratchForRangeData()
            {
                Add(
                    new Range
                    {
                        Start = new Position
                        {
                            Line = 12,
                            Character = 21,
                        },
                        End = new Position
                        {
                            Line = 14,
                            Character = 27
                        }
                    },
                    new NormalizedToken[]
                    {
                        "ssage (enum:none)", "= (label:none)", "Hello (comment:none)", "World!! (enum:none)", "Console (interface:static)",
                        "WriteLi (event:async|modification)"
                    }
                );
                Add(
                    new Range
                    {
                        Start = new Position
                        {
                            Line = 0,
                            Character = 0,
                        },
                        End = new Position
                        {
                            Line = 5,
                            Character = 0
                        }
                    },
                    new NormalizedToken[]
                    {
                        "using (macro:async|deprecated)", "System (macro:async|deprecated)", "using (event:none)", "System (number:none)",
                        "Collections (struct:readonly)",
                        "Generic (class:none)", "using (variable:modification|defaultLibrary)", "System (comment:static|deprecated)",
                        "Linq (comment:definition)",
                        "using (comment:none)", "System (enumMember:none)", "Text (comment:static|deprecated)", "using (comment:none)",
                        "System (comment:declaration)",
                        "Threading (comment:static|defaultLibrary)", "Tasks (comment:none)"
                    }
                );
                Add(
                    new Range
                    {
                        Start = new Position
                        {
                            Line = 14,
                            Character = 0,
                        },
                        End = new Position
                        {
                            Line = 14,
                            Character = 30
                        }
                    },
                    new NormalizedToken[]
                    {
                        "Console (interface:static)", "WriteLine (event:async|modification)", "message (interface:static)"
                    }
                );
            }
        }

        [Theory]
        [ClassData(typeof(ReturnDocumentEditsData))]
        public void ReturnDocumentEdits(
            string originalText, string modifiedText,
            IEnumerable<NormalizedToken> expectedTokens
        )
        {
            var document = new SemanticTokensDocument(_legend);

            SemanticTokens originalTokens;

            {
                var builder = document.Create();
                Tokenize(originalText, builder);
                builder.Commit();
                originalTokens = document.GetSemanticTokens();
                builder = document.Edit(
                    new SemanticTokensDeltaParams
                    {
                        PreviousResultId = document.Id,
                    }
                );
                Tokenize(modifiedText, builder);
                builder.Commit();
            }

            var result = document.GetSemanticTokensEdits();
            result.IsDelta.Should().BeTrue();
            var edits = result.Delta!;

            edits.ResultId.Should().Be(document.Id);
            edits.Edits.Should().HaveCount(1);
            var edit1 = edits.Edits.First();

            var edit1Tokens = originalTokens.Data
                                            .RemoveRange(edit1.Start, edit1.DeleteCount)
                                            .InsertRange(edit1.Start, edit1.Data!);

            var edit1Data = Normalize(modifiedText, edit1Tokens).ToArray();
            _logger.LogDebug("Some Data {Data}", edit1Data.AsEnumerable());
            edit1Data.Should().ContainInOrder(expectedTokens);
        }

        public class ReturnDocumentEditsData : TheoryData<string, string, IEnumerable<NormalizedToken>>
        {
            public ReturnDocumentEditsData()
            {
                Add(
                    ExampleDocumentText,
                    ExampleDocumentText.Replace("namespace CSharpTutorials", "namespace Something.Else.Entirely"),
                    new NormalizedToken[]
                    {
                        "using (macro:async|deprecated)", "System (macro:async|deprecated)", "using (event:none)", "System (number:none)",
                        "Collections (struct:readonly)",
                        "Generic (class:none)", "using (variable:modification|defaultLibrary)", "System (comment:static|deprecated)",
                        "Linq (comment:definition)",
                        "using (comment:none)", "System (enumMember:none)", "Text (comment:static|deprecated)", "using (comment:none)",
                        "System (comment:declaration)",
                        "Threading (comment:static|defaultLibrary)", "Tasks (comment:none)", "namespace (property:defaultLibrary)",
                        "Something (property:defaultLibrary)",
                        "Else (enum:deprecated)", "Entirely (operator:declaration|deprecated|modification)", "{ (struct:documentation)",
                        "class (enumMember:none)",
                        "Program (comment:none)", "{ (comment:none)", "static (regexp:documentation)", "void (macro:none)", "Main (macro:none)",
                        "string[] (property:declaration|abstract)", "args (macro:none)", "{ (interface:documentation|declaration|deprecated)",
                        "string (struct:none)",
                        "message (enum:none)", "= (label:none)", "Hello (comment:none)", "World!! (enum:none)", "Console (interface:static)",
                        "WriteLine (event:async|modification)", "message (interface:static)", "} (operator:none)", "} (enum:async|deprecated)",
                        "} (function:declaration|async)"
                    }
                );
                Add(
                    "using", "using System;",
                    new NormalizedToken[]
                    {
                        "using (macro:async|deprecated)", "System (macro:async|deprecated)"
                    }
                );
                Add(
                    "using System;", "using", new NormalizedToken[]
                    {
                        "using (macro:async|deprecated)"
                    }
                );
            }
        }

        private class TokenizationValue
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public SemanticTokenType Type { get; set; }

            // ReSharper disable once UnusedAutoPropertyAccessor.Local
#pragma warning disable CS8618
            public SemanticTokenModifier[] Modifiers { get; set; }
#pragma warning restore CS8618
        }

        private void Tokenize(string document, SemanticTokensBuilder builder)
        {
            var faker = new Faker<TokenizationValue>()
                       .RuleFor(
                            z => z.Type,
                            f => f.PickRandom(_legend.TokenTypes.AsEnumerable()).OrNull(f, 0.2f) ?? new SemanticTokenType("none")
                        )
                       .RuleFor(
                            x => x.Modifiers,
                            f => Enumerable.Range(0, f.Random.Int(0, 3))
                                           .Select(
                                                z =>
                                                    f.PickRandom(_legend.TokenModifiers.AsEnumerable()).OrNull(f, 0.2f) ??
                                                    new SemanticTokenModifier("none")
                                            )
                                           .ToArray()
                                           .OrNull(f, 0.2f)
                        );

            foreach (var (line, text) in document.Split('\n').Select((text, line) => ( line, text )))
            {
                var parts = text.TrimEnd().Split(';', ' ', '.', '"', '(', ')');
                var index = 0;
                foreach (var part in parts)
                {
                    faker.UseSeed(part.Length * line * text.Length);
                    if (string.IsNullOrWhiteSpace(part)) continue;
                    // _logger.LogWarning("Index for part before {Index}: {Text}", index, part);
                    index = text.IndexOf(part, index, StringComparison.Ordinal);
                    // _logger.LogDebug("Index for part after {Index}: {Text}", index, part);
                    var item = faker.Generate();
                    if (index % 2 == 0)
                    {
                        builder.Push(line, index, part.Length, item.Type, item.Modifiers);
                    }
                    else
                    {
                        // ensure range gets some love
                        builder.Push(( ( line, index ), ( line, part.Length + index ) ), item.Type, item.Modifiers);
                    }
                }
            }
        }

        private string ExtractRange(string document, Range range)
        {
            var sb = new StringBuilder();
            var lines = document.Split('\n');
            sb.AppendLine(lines[range.Start.Line].Substring(range.Start.Character));
            for (var i = range.Start.Line + 1; i < range.End.Line; i++)
            {
                sb.AppendLine(lines[i]);
            }

            sb.Append(lines[range.End.Line].Substring(0, range.End.Character));

            return sb.ToString();
        }

        [DebuggerDisplay("{ToString()}")]
        public class NormalizedToken : IEquatable<NormalizedToken>, IEquatable<string>
        {
            public NormalizedToken(string text, SemanticTokenType type, params SemanticTokenModifier[] modifiers)
            {
                Text = text;
                Type = type;
                Modifiers = modifiers;
            }

            public bool Equals(string? other)
            {
                return string.Equals(ToString(), other);
            }

            public bool Equals(NormalizedToken? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;
                return other.ToString() == ToString();
            }

            public override bool Equals(object? obj)
            {
                if (obj is null) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == GetType() && Equals((NormalizedToken)obj);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Text, Type, Modifiers);
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append(Text);
                sb.Append(" (");
                sb.Append(Type);
                sb.Append(":");

                sb.Append(Modifiers.Any() ? string.Join("|", Modifiers) : "none");

                sb.Append(")");

                return sb.ToString();
            }

            public static implicit operator NormalizedToken(string value)
            {
                var item = value.Split(' ');
                var other = item[1].Trim('(', ')').Split(':');

                return new NormalizedToken(
                    item[0],
                    other[0],
                    other[1].Split('|')
                            .Select(x => new SemanticTokenModifier(x))
                            .ToArray()
                );
            }

            public static bool operator ==(NormalizedToken left, NormalizedToken right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(NormalizedToken left, NormalizedToken right)
            {
                return !Equals(left, right);
            }

            public string Text { get; }
            public SemanticTokenType Type { get; }
            public SemanticTokenModifier[] Modifiers { get; }
        }

        private IReadOnlyList<NormalizedToken?> Normalize(string document, IReadOnlyList<int> values)
        {
            var parts = Decompose(values).ToArray();
            return parts
                  .Select((item, index) => GetNormalizedToken(document, parts, index)!)
                  .Where(z => z != null!)
                  .ToArray();
        }

        private NormalizedToken? GetNormalizedToken(
            string document,
            IReadOnlyList<(int lineOffset, int charOffset, int length, int type, int modifiers)> tokens, int tokenIndex
        )
        {
            var lines = document.Split('\n');
            var lineIndex = 0;
            var characterOffset = 0;
            for (var i = 0; i <= tokenIndex; i++)
            {
                var token = tokens[i];
                lineIndex += token.lineOffset;
                characterOffset = token.lineOffset == 0 ? characterOffset + token.charOffset : token.charOffset;
            }

            //empty line
            if (lines.Length <= lineIndex) return null;
            var line = lines[lineIndex];
            var textToken = tokens[tokenIndex];
            return new NormalizedToken(
                line.Substring(characterOffset, textToken.length),
                _legend.TokenTypes
                       .Where((x, i) => i == textToken.type)
                       .Select(x => new SemanticTokenType(x))
                       .First(),
                _legend.TokenModifiers
                       .Where(
                            (x, i) =>
                                ( textToken.modifiers & Convert.ToInt32(Math.Pow(2, i)) ) == Convert.ToInt32(Math.Pow(2, i))
                        )
                       .Select(x => new SemanticTokenModifier(x))
                       .ToArray()
            );
        }

        private static IEnumerable<(int lineOffset, int charOffset, int length, int type, int modifiers)> Decompose(
            IReadOnlyList<int> values
        )
        {
            for (var i = 0; i < values.Count; i += 5)
            {
                yield return ( values[i], values[i + 1], values[i + 2], values[i + 3], values[i + 4] );
            }
        }

        internal static string ExampleDocumentText = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpTutorials
{
    class Program
    {
        static void Main(string[] args)
        {
            string message = ""Hello World!!"";

            Console.WriteLine(message);
        }
    }
}
".Replace("\r\n", "\n", StringComparison.Ordinal);
    }
}
