using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Bogus;
using Bogus.Extensions;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Protocol.Document.Server.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
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
                TokenModifiers = Enum.GetNames(typeof(SemanticTokenModifiers)),
                TokenTypes = Enum.GetNames(typeof(SemanticTokenTypes)),
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
            data.Should().ContainInOrder(
                (NormalizedToken) "using (Member:Declaration|Abstract)",
                (NormalizedToken) "System (Member:Declaration|Abstract)",
                (NormalizedToken) "using (Interface:None)",
                (NormalizedToken) "System (Namespace:Documentation)",
                (NormalizedToken) "Collections (String:None)",
                (NormalizedToken) "Generic (Keyword:None)",
                (NormalizedToken) "using (Label:Documentation|Readonly)",
                (NormalizedToken) "System (Macro:None)",
                (NormalizedToken) "Linq (String:None)",
                (NormalizedToken) "using (TypeParameter:Definition|Abstract|Readonly)",
                (NormalizedToken) "System (Comment:None)",
                (NormalizedToken) "Text (Macro:None)",
                (NormalizedToken) "using (Number:None)",
                (NormalizedToken) "System (Comment:None)",
                (NormalizedToken) "Threading (Parameter:Documentation|Deprecated)",
                (NormalizedToken) "Tasks (Number:None)",
                (NormalizedToken) "namespace (Regexp:None)",
                (NormalizedToken) "CSharpTutorials (Regexp:Documentation)",
                (NormalizedToken) "{ (Comment:Abstract)",
                (NormalizedToken) "class (Comment:None)",
                (NormalizedToken) "Program (Namespace:Abstract)",
                (NormalizedToken) "{ (Parameter:Documentation|Abstract)",
                (NormalizedToken) "static (Comment:Deprecated)",
                (NormalizedToken) "void (TypeParameter:None)",
                (NormalizedToken) "Main (TypeParameter:None)",
                (NormalizedToken) "string[] (Class:None)",
                (NormalizedToken) "args (TypeParameter:None)",
                (NormalizedToken) "{ (Comment:Documentation|Readonly)",
                (NormalizedToken) "string (Label:Abstract|Readonly)",
                (NormalizedToken) "message (Type:Definition|Deprecated)",
                (NormalizedToken) "= (String:Static)",
                (NormalizedToken) "Hello (TypeParameter:Definition|Static|Abstract)",
                (NormalizedToken) "World!! (Type:Definition|Deprecated)",
                (NormalizedToken) "Console (Comment:Documentation|Definition)",
                (NormalizedToken) "WriteLine (Member:None)",
                (NormalizedToken) "message (Comment:Documentation|Definition)",
                (NormalizedToken) "} (Keyword:Abstract|Readonly)",
                (NormalizedToken) "} (Variable:Documentation|Abstract)",
                (NormalizedToken) "} (Class:Static|Deprecated)"
            );
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
                        "ssage (Type:Definition|Deprecated)",
                        "= (String:Static)",
                        "Hello (TypeParameter:Definition|Static|Abstract)",
                        "World!! (Type:Definition|Deprecated)",
                        "Console (Comment:Documentation|Definition)",
                        "WriteLi (Member:None)"
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
                        "using (Member:Declaration|Abstract)",
                        "System (Member:Declaration|Abstract)",
                        "using (Interface:None)",
                        "System (Namespace:Documentation)",
                        "Collections (String:None)",
                        "Generic (Keyword:None)",
                        "using (Label:Documentation|Readonly)",
                        "System (Macro:None)",
                        "Linq (String:None)",
                        "using (TypeParameter:Definition|Abstract|Readonly)",
                        "System (Comment:None)",
                        "Text (Macro:None)",
                        "using (Number:None)",
                        "System (Comment:None)",
                        "Threading (Parameter:Documentation|Deprecated)",
                        "Tasks (Number:None)"
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
                            Character = 31
                        }
                    },
                    new NormalizedToken[] {
                        "Console (Comment:Documentation|Definition)",
                        "WriteLine (Member:None)",
                        "message (Comment:Documentation|Definition)"
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
                        "using (Member:Declaration|Abstract)",
                        "System (Member:Declaration|Abstract)",
                        "using (Interface:None)",
                        "System (Namespace:Documentation)",
                        "Collections (String:None)",
                        "Generic (Keyword:None)",
                        "using (Label:Documentation|Readonly)",
                        "System (Macro:None)",
                        "Linq (String:None)",
                        "using (TypeParameter:Definition|Abstract|Readonly)",
                        "System (Comment:None)",
                        "Text (Macro:None)",
                        "using (Number:None)",
                        "System (Comment:None)",
                        "Threading (Parameter:Documentation|Deprecated)",
                        "Tasks (Number:None)",
                        "namespace (Variable:None)",
                        "Something (Variable:None)",
                        "Else (Comment:None)",
                        "Entirely (Namespace:Documentation|Readonly)",
                        "{ (Comment:Abstract)",
                        "class (Comment:None)",
                        "Program (Namespace:Abstract)",
                        "{ (Parameter:Documentation|Abstract)",
                        "static (Comment:Deprecated)",
                        "void (TypeParameter:None)",
                        "Main (TypeParameter:None)",
                        "string[] (Class:None)",
                        "args (TypeParameter:None)",
                        "{ (Comment:Documentation|Readonly)",
                        "string (Label:Abstract|Readonly)",
                        "message (Type:Definition|Deprecated)",
                        "= (String:Static)",
                        "Hello (TypeParameter:Definition|Static|Abstract)",
                        "World!! (Type:Definition|Deprecated)",
                        "Console (Comment:Documentation|Definition)",
                        "WriteLine (Member:None)",
                        "message (Comment:Documentation|Definition)",
                        "} (Keyword:Abstract|Readonly)",
                        "} (Variable:Documentation|Abstract)",
                        "} (Class:Static|Deprecated)"
                    });
                Add("using", "using System;", new NormalizedToken[] { });
                Add("using System;", "using", new NormalizedToken[] { });
            }
        }


        private class TokenizationValue
        {
            public SemanticTokenTypes? Type { get; set; }
            public SemanticTokenModifiers[] Modifiers { get; set; }
        }

        private void Tokenize(string document, SemanticTokensBuilder builder)
        {
            var faker = new Faker<TokenizationValue>()
                .RuleFor(z => z.Type, f => f.PickRandom<SemanticTokenTypes>().OrNull(f, 0.2f))
                .RuleFor(x => x.Modifiers,
                    f => Enumerable.Range(0, f.Random.Int(0, 3))
                        .Select(z => f.PickRandom<SemanticTokenModifiers>())
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
                    builder.Push(line, index, part.Length, item.Type, item.Modifiers);
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
            public NormalizedToken(string text, SemanticTokenTypes? type, params SemanticTokenModifiers[] modifiers)
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
                return Text == other.Text && Type == other.Type &&
                       Modifiers.SequenceEqual(other.Modifiers ?? Array.Empty<SemanticTokenModifiers>()) == true;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == GetType() && Equals((NormalizedToken) obj);
            }

            public override int GetHashCode() => HashCode.Combine(Text, Type ?? 0, Modifiers);

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append(Text);
                sb.Append(" (");
                if (Type.HasValue)
                {
                    sb.Append(Type);
                    sb.Append(":");
                }
                else
                {
                    sb.Append("None");
                    sb.Append(":");
                }

                if (Modifiers?.Any() == true)
                {
                    sb.Append(string.Join("|", Modifiers));
                }
                else
                {
                    sb.Append("None");
                }

                sb.Append(")");

                return sb.ToString();
            }

            public static implicit operator NormalizedToken(string value)
            {
                var item = value.Split(' ');
                var other = item[1].Trim('(', ')').Split(':');


                return new NormalizedToken(item[0],
                    Enum.TryParse<SemanticTokenTypes>(other[0], out var t) ? new SemanticTokenTypes?(t) : null,
                    other[1] == "None"
                        ? null
                        : other[1].Split('|')
                            .Select(Enum.Parse<SemanticTokenModifiers>)
                            .ToArray()
                );
                //System (Struct:Declaration|Abstract)
            }

            public static bool operator ==(NormalizedToken left, NormalizedToken right) => Equals(left, right);

            public static bool operator !=(NormalizedToken left, NormalizedToken right) => !Equals(left, right);

            public string Text { get; }
            public SemanticTokenTypes? Type { get; }
            public SemanticTokenModifiers[] Modifiers { get; }
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
                    .Select(Enum.Parse<SemanticTokenTypes>)
                    .First(),
                _legend.TokenModifiers
                    .Where((x, i) =>
                        (textToken.modifiers & Convert.ToInt32(Math.Pow(2, i))) == Convert.ToInt32(Math.Pow(2, i))
                    )
                    .Select(Enum.Parse<SemanticTokenModifiers>)
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

        internal const string ExampleDocumentText = @"using System;
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
";
    }
}
