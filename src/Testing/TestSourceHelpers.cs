using System;
using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;


namespace OmniSharp.Extensions.LanguageProtocol.Testing
{
    public static class TestSourceHelpers
    {
        public static Position GetPositionAtIndex(string source, int index)
        {
            var line = 0;
            var span = source.AsSpan();
            var rollingIndex = 0;
            do
            {
                var location = span.IndexOf('\n');
                if (location == -1)
                {
                    if (rollingIndex + span.Length >= index)
                    {
                        return new Position(line, index - rollingIndex);
                    }

                    return ( line, span.Length );
                }

                if (rollingIndex + location >= index)
                {
                    return new Position(line, index - rollingIndex);
                }

                span = span.Slice(location + 1);
                rollingIndex += location + 1;
                line++;
                if (rollingIndex == index)
                {
                    return new Position(line, 0);
                }
            } while (!span.IsEmpty);

            return ( line, 0 );
        }

        public static string NormalizeLineEndings(this string value) => value.Replace("\r\n", "\n");

        public static string ExtractRange(this TestContent source, Range range)
        {
            var start = source.GetIndexAtPosition(range.Start);
            return source.Code.Substring(start, source.GetIndexAtPosition(range.End) - start);
        }

        public static IEnumerable<string> ExtractRanges(this TestContent source, IEnumerable<Range> ranges)
        {
            foreach (var range in ranges)
            {
                var start = source.GetIndexAtPosition(range.Start);
                yield return source.Code.Substring(start, source.GetIndexAtPosition(range.End) - start);
            }
        }

        public static int GetIndexAtPosition(IReadOnlyList<string> lines, Position position)
        {
            if (position.Line >= lines.Count) return -1;
            var characterCount = lines
                                .Take(position.Line)
                                .Aggregate(0, (acc, v) => acc + v.Length);
            return characterCount + position.Character;
        }

        public static int GetIndexAtPosition(in string[] lines, Position position)
        {
            if (position.Line >= lines.Length) return -1;
            var characterCount = lines
                                .Take(position.Line)
                                .Aggregate(0, (acc, v) => acc + v.Length);
            return characterCount + position.Character;
        }
    }
}
