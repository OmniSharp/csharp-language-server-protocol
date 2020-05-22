using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bogus;
using Bogus.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit;
using Xunit.Abstractions;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

#pragma warning disable 618

namespace Lsp.Tests
{
    public class SemanticTokensDocumentTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly TestLoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private readonly SemanticTokensLegend _legend;

        public SemanticTokensDocumentTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _loggerFactory = new TestLoggerFactory(testOutputHelper);
            _logger = _loggerFactory.CreateLogger<SemanticTokensDocumentTests>();
            _legend = new SemanticTokensLegend() {
                TokenModifiers = SemanticTokenModifier.Defaults.Select(x => x.ToString()).ToArray(),
                TokenTypes = SemanticTokenType.Defaults.Select(x => x.ToString()).ToArray(),
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
            var expectedResponse = new NormalizedToken[] {
                "using (member:static|abstract)", "System (member:static|abstract)", "using (parameter:none)",
                "System (string:none)", "Collections (namespace:deprecated)", "Generic (type:none)",
                "using (property:none)", "System (documentation:definition|abstract)",
                "Linq (documentation:definition)", "using (documentation:none)", "System (label:none)",
                "Text (documentation:definition|abstract)", "using (documentation:none)",
                "System (documentation:documentation)", "Threading (documentation:definition|readonly)",
                "Tasks (documentation:none)", "namespace (documentation:abstract)",
                "CSharpTutorials (documentation:none)", "{ (type:documentation)", "class (label:none)",
                "Program (documentation:none)", "{ (documentation:none)", "static (number:documentation)",
                "void (function:none)", "Main (function:none)", "string[] (function:documentation|static)",
                "args (function:none)", "{ (struct:declaration|abstract)", "string (type:none)", "message (class:none)",
                "= (macro:none)", "Hello (documentation:none)", "World!! (class:none)", "Console (struct:definition)",
                "WriteLine (parameter:definition|readonly)", "message (struct:definition)", "} (regexp:none)",
                "} (class:static|abstract)", "} (enum:declaration|definition)"
            };
            data.Should().ContainInOrder(expectedResponse);
        }

        [Theory]
        [ClassData(typeof(ReturnDocumentTokensFromScratch_ForRange_Data))]
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
            _logger.LogInformation("Some Data {Data}", data.AsEnumerable());
            data.Should().ContainInOrder(expectedTokens);
        }

        public class ReturnDocumentTokensFromScratch_ForRange_Data : TheoryData<Range, IEnumerable<NormalizedToken>>
        {
            public ReturnDocumentTokensFromScratch_ForRange_Data()
            {
                Add(
                    new Range() {
                        Start = new Position() {
                            Line = 12,
                            Character = 21,
                        },
                        End = new Position() {
                            Line = 14,
                            Character = 27
                        }
                    },
                    new NormalizedToken[] {
                        "ssage (class:none)", "= (macro:none)", "Hello (documentation:none)", "World!! (class:none)",
                        "Console (struct:definition)", "WriteLi (parameter:definition|readonly)"
                    }
                );
                Add(
                    new Range() {
                        Start = new Position() {
                            Line = 0,
                            Character = 0,
                        },
                        End = new Position() {
                            Line = 5,
                            Character = 0
                        }
                    },
                    new NormalizedToken[] {
                        "using (member:static|abstract)", "System (member:static|abstract)", "using (parameter:none)",
                        "System (string:none)", "Collections (namespace:deprecated)", "Generic (type:none)",
                        "using (property:none)", "System (documentation:definition|abstract)",
                        "Linq (documentation:definition)", "using (documentation:none)", "System (label:none)",
                        "Text (documentation:definition|abstract)", "using (documentation:none)",
                        "System (documentation:documentation)", "Threading (documentation:definition|readonly)",
                        "Tasks (documentation:none)"
                    }
                );
                Add(
                    new Range() {
                        Start = new Position() {
                            Line = 14,
                            Character = 0,
                        },
                        End = new Position() {
                            Line = 14,
                            Character = 30
                        }
                    },
                    new NormalizedToken[] {
                        "Console (struct:definition)", "WriteLine (parameter:definition|readonly)",
                        "message (struct:definition)"
                    }
                );
            }
        }

        [Theory]
        [ClassData(typeof(ReturnDocumentEdits_Data))]
        public void ReturnDocumentEdits(string originalText, string modifiedText,
            IEnumerable<NormalizedToken> expectedTokens)
        {
            var document = new SemanticTokensDocument(_legend);

            SemanticTokens originalTokens;

            {
                var builder = document.Create();
                Tokenize(originalText, builder);
                builder.Commit();
                originalTokens = document.GetSemanticTokens();
                builder = document.Edit(new SemanticTokensEditsParams() {
                    PreviousResultId = document.Id,
                });
                Tokenize(modifiedText, builder);
                builder.Commit();
            }

            var result = document.GetSemanticTokensEdits();
            result.IsSemanticTokensEdits.Should().BeTrue();
            var edits = result.SemanticTokensEdits;

            edits.ResultId.Should().Be(document.Id);
            edits.Edits.Should().HaveCount(1);
            var edit1 = edits.Edits.First();

            var edit1Tokens = originalTokens.Data
                .RemoveRange(edit1.Start, edit1.DeleteCount)
                .InsertRange(edit1.Start, edit1.Data);

            var edit1Data = Normalize(modifiedText, edit1Tokens).ToArray();
            _logger.LogInformation("Some Data {Data}", edit1Data.AsEnumerable());
            edit1Data.Should().ContainInOrder(expectedTokens);
        }

        public class ReturnDocumentEdits_Data : TheoryData<string, string, IEnumerable<NormalizedToken>>
        {
            public ReturnDocumentEdits_Data()
            {
                Add(
                    ExampleDocumentText,
                    ExampleDocumentText.Replace("namespace CSharpTutorials", "namespace Something.Else.Entirely"),
                    new NormalizedToken[] {
                        "using (member:static|abstract)", "System (member:static|abstract)", "using (parameter:none)",
                        "System (string:none)", "Collections (namespace:deprecated)", "Generic (type:none)",
                        "using (property:none)", "System (documentation:definition|abstract)",
                        "Linq (documentation:definition)", "using (documentation:none)", "System (label:none)",
                        "Text (documentation:definition|abstract)", "using (documentation:none)",
                        "System (documentation:documentation)", "Threading (documentation:definition|readonly)",
                        "Tasks (documentation:none)", "namespace (function:readonly)", "Something (function:readonly)",
                        "Else (class:abstract)", "Entirely (regexp:documentation|abstract|deprecated)",
                        "{ (type:documentation)", "class (label:none)", "Program (documentation:none)",
                        "{ (documentation:none)", "static (number:documentation)", "void (function:none)",
                        "Main (function:none)", "string[] (function:documentation|static)", "args (function:none)",
                        "{ (struct:declaration|abstract)", "string (type:none)", "message (class:none)",
                        "= (macro:none)", "Hello (documentation:none)", "World!! (class:none)",
                        "Console (struct:definition)", "WriteLine (parameter:definition|readonly)",
                        "message (struct:definition)", "} (regexp:none)", "} (class:static|abstract)",
                        "} (enum:declaration|definition)"
                    });
                Add("using", "using System;",
                    new NormalizedToken[] {
                        "using (member:static|abstract)", "System (member:static|abstract)"
                    });
                Add("using System;", "using", new NormalizedToken[] {
                    "using (member:static|abstract)"
                });
            }
        }


        private class TokenizationValue
        {
            public SemanticTokenType type { get; set; }
            public SemanticTokenModifier[] Modifiers { get; set; }
        }

        private void Tokenize(string document, SemanticTokensBuilder builder)
        {
            var faker = new Faker<TokenizationValue>()
                .RuleFor(z => z.type,
                    f => f.PickRandom(SemanticTokenType.Defaults).OrNull(f, 0.2f) ?? new SemanticTokenType("none")
                )
                .RuleFor(x => x.Modifiers,
                    f => Enumerable.Range(0, f.Random.Int(0, 3))
                        .Select(z =>
                            f.PickRandom(SemanticTokenModifier.Defaults).OrNull(f, 0.2f) ??
                            new SemanticTokenModifier("none")
                        )
                        .ToArray()
                        .OrNull(f, 0.2f)
                );

            foreach (var (line, text) in document.Split('\n').Select((text, line) => (line, text)))
            {
                var parts = text.TrimEnd().Split(';', ' ', '.', '"', '(', ')');
                var index = 0;
                foreach (var part in parts)
                {
                    faker.UseSeed(part.Length * line * text.Length);
                    if (string.IsNullOrWhiteSpace(part)) continue;
                    // _logger.LogWarning("Index for part before {Index}: {Text}", index, part);
                    index = text.IndexOf(part, index, StringComparison.Ordinal);
                    // _logger.LogInformation("Index for part after {Index}: {Text}", index, part);
                    var item = faker.Generate();
                    builder.Push(line, index, part.Length, item.type, item.Modifiers);
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

        public class NormalizedToken : IEquatable<NormalizedToken>, IEquatable<string>
        {
            public NormalizedToken(string text, SemanticTokenType type, params SemanticTokenModifier[] modifiers)
            {
                Text = text;
                Type = type;
                Modifiers = modifiers;
            }

            public bool Equals(string other)
            {
                return string.Equals(ToString(), other);
            }

            public bool Equals(NormalizedToken other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other.ToString() == this.ToString();
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == GetType() && Equals((NormalizedToken) obj);
            }

            public override int GetHashCode() => HashCode.Combine(Text, Type, Modifiers);

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append(Text);
                sb.Append(" (");
                sb.Append(Type);
                sb.Append(":");

                if (Modifiers?.Any() == true)
                {
                    sb.Append(string.Join("|", Modifiers));
                }
                else
                {
                    sb.Append("none");
                }

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

            public static bool operator ==(NormalizedToken left, NormalizedToken right) => Equals(left, right);

            public static bool operator !=(NormalizedToken left, NormalizedToken right) => !Equals(left, right);

            public string Text { get; }
            public SemanticTokenType Type { get; }
            public SemanticTokenModifier[] Modifiers { get; }
        }

        private IReadOnlyList<NormalizedToken> Normalize(string document, IReadOnlyList<int> values)
        {
            var parts = Decompose(values).ToArray();
            return parts
                .Select((item, index) => GetNormalizedToken(document, parts, index))
                .Where(z => z != null)
                .ToArray();
        }

        private NormalizedToken GetNormalizedToken(string document,
            IReadOnlyList<(int lineOffset, int charOffset, int length, int type, int modifiers)> tokens, int tokenIndex)
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
                    .Where((x, i) =>
                        (textToken.modifiers & Convert.ToInt32(Math.Pow(2, i))) == Convert.ToInt32(Math.Pow(2, i))
                    )
                    .Select(x => new SemanticTokenModifier(x))
                    .ToArray()
            );
        }

        private static IEnumerable<(int lineOffset, int charOffset, int length, int type, int modifiers)> Decompose(
            IReadOnlyList<int> values)
        {
            for (var i = 0; i < values.Count; i += 5)
            {
                yield return (values[i], values[i + 1], values[i + 2], values[i + 3], values[i + 4]);
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
".Replace("\r\n", "\n");
    }
}
