using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace OmniSharp.Extensions.LanguageProtocol.Testing
{
    /// <summary>
    /// MarkupCode allows encoding additional pieces of information along with a piece of source code
    /// that are useful for testing. The following information can be encoded:
    ///
    /// $$ - The index in the code. There can be no more than one of these.
    ///
    /// [| ... |] - A span in the code. There can be many of these and they can be nested.
    ///
    /// {|Name| ... |} - A span of code that is annotated with a name. There can be many of these and
    /// they can be nested.
    ///
    /// This is similar the MarkupTestFile used in Roslyn:
    ///     https://github.com/dotnet/roslyn/blob/master/src/Test/Utilities/Shared/MarkedSource/MarkupTestFile.cs
    /// </summary>
    /// <remarks>
    /// Taken from OmniSharp source and modified
    /// </remarks>
    public class TestContent
    {
        private readonly int? _index;
        private readonly ImmutableDictionary<string, ImmutableList<Range>> _spans;

        private TestContent(string code, int? index, ImmutableDictionary<string, ImmutableList<Range>> spans)
        {
            Code = code;
            Lines = ParseLines(code).ToImmutableArray();
            _index = index;
            _spans = spans;
        }

        public string Code { get; }
        public ImmutableArray<string> Lines { get; }
        public int Index => _index ?? -1;
        public bool HasIndex => _index.HasValue;

        public ImmutableList<Range> GetRanges(string? name = null)
        {
            if (_spans.TryGetValue(name ?? string.Empty, out var result))
            {
                return result;
            }

            return ImmutableList<Range>.Empty;
        }

        public Position GetPositionAtIndex(int? index = null) => TestSourceHelpers.GetPositionAtIndex(Code, index ?? Index);
        public int GetIndexAtPosition(Position position) => TestSourceHelpers.GetIndexAtPosition(Lines, position);

        private static IEnumerable<string> ParseLines(string source)
        {
            var lastStart = 0;
            var length = source.Length;
            for (var index = 0; index < length; index++)
            {
                if (source[index] == '\n')
                {
                    yield return source.Substring(lastStart, Math.Min(index + 1, length) - lastStart);
                    lastStart = index + 1;
                }
            }

            yield return source.Substring(lastStart);
        }

        public static TestContent Parse(string input, TestContentOptions? options = null)
        {
            options ??= new TestContentOptions();
            // TODO: Should this be configurable?
            input = input.NormalizeLineEndings();
            var markupLength = input.Length;
            var codeBuilder = new StringBuilder(markupLength);

            int? position = null;
            var spanStartStack = new Stack<int>();
            var namedSpanStartStack = new Stack<(int spanStart, string spanName)>();
            var spans = new Dictionary<string, List<(int start, int end)>>();

            var codeIndex = 0;
            var markupIndex = 0;

            var positionMarker = options.PositionMarker;
            var rangeMarker = options.RangeMarker;
            var namedMarker = options.NamedRangeMarker;

            while (markupIndex < markupLength)
            {
                var ch = input[markupIndex];

                if (ch == positionMarker.First)
                {
                    if (position == null &&
                        markupIndex + 1 < markupLength &&
                        input[markupIndex + 1] == positionMarker.Second)
                    {
                        position = codeIndex;
                        markupIndex += 2;
                        continue;
                    }
                }
                else if (ch == rangeMarker.open.First)
                {
                    if (markupIndex + 1 < markupLength &&
                        input[markupIndex + 1] == rangeMarker.open.Second)
                    {
                        spanStartStack.Push(codeIndex);
                        markupIndex += 2;
                        continue;
                    }
                }
                else if (ch == namedMarker.open.First)
                {
                    if (markupIndex + 1 < markupLength &&
                        input[markupIndex + 1] == namedMarker.open.Second)
                    {
                        var nameIndex = markupIndex + 2;
                        var nameStartIndex = nameIndex;
                        var nameLength = 0;
                        var found = false;

                        // Parse out name
                        while (nameIndex < markupLength)
                        {
                            if (input[nameIndex] == namedMarker.labelStop)
                            {
                                found = true;
                                break;
                            }

                            nameLength++;
                            nameIndex++;
                        }

                        if (found)
                        {
                            var name = input.Substring(nameStartIndex, nameLength);
                            namedSpanStartStack.Push(( codeIndex, name ));
                            markupIndex = nameIndex + 1; // Move after ':'
                            continue;
                        }

                        // We didn't find a ':'. In this case, we just carry on...
                    }
                }
                else if (ch == rangeMarker.close.First || ch == namedMarker.close.First)
                {
                    if (markupIndex + 1 < markupLength)
                    {
                        if (ch == rangeMarker.close.First && input[markupIndex + 1] == rangeMarker.close.Second)
                        {
                            if (spanStartStack.Count == 0)
                            {
                                throw new ArgumentException($"Saw {rangeMarker.close} without matching {rangeMarker.open}");
                            }

                            var spanStart = spanStartStack.Pop();

                            AddSpan(spans, string.Empty, spanStart, codeIndex);
                            markupIndex += 2;

                            continue;
                        }

                        if (ch == namedMarker.close.First && input[markupIndex + 1] == namedMarker.close.Second)
                        {
                            if (namedSpanStartStack.Count == 0)
                            {
                                throw new ArgumentException($"Saw {namedMarker.close} without matching {namedMarker.open}");
                            }

                            var tuple = namedSpanStartStack.Pop();
                            var spanStart = tuple.Item1;
                            var spanName = tuple.Item2;

                            AddSpan(spans, spanName, spanStart, codeIndex);
                            markupIndex += 2;

                            continue;
                        }
                    }
                }

                codeBuilder.Append(ch);
                codeIndex++;
                markupIndex++;
            }

            var source = codeBuilder.ToString();
            var finalSpans = spans
               .ToImmutableDictionary(
                    keySelector: kvp => kvp.Key,
                    elementSelector: kvp => kvp.Value
                                               .Select(z => new Range(TestSourceHelpers.GetPositionAtIndex(source, z.start), TestSourceHelpers.GetPositionAtIndex(source, z.end)))
                                               .ToImmutableList()
                                               .Sort(Range.AscendingComparer)
                );

            return new TestContent(source, position, finalSpans);
        }

        private static void AddSpan(Dictionary<string, List<(int start, int end)>> spans, string spanName, int spanStart, int spanEnd)
        {
            if (!spans.TryGetValue(spanName, out var spanList))
            {
                spanList = new List<(int start, int end)>();
                spans.Add(spanName, spanList);
            }

            spanList.Add(( spanStart, spanEnd ));
        }
    }
}
