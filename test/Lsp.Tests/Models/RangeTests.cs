using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using TestingUtils;

namespace Lsp.Tests.Models
{
    public class RangeTests
    {
        [Theory]
        [JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new Range(new Position(1, 1), new Position(2, 2));
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<Range>(expected);
            deresult.Should().BeEquivalentTo(model, x => x.UsingStructuralRecordEquality());
        }

        [Fact]
        public void Range_Is_Empty()
        {
            var s = new Range(1, 1, 1, 1);
            s.IsEmpty().Should().BeTrue();
        }

        [Fact]
        public void Range_Equality()
        {
            var a = new Range(1, 1, 1, 1);
            var b = new Range(1, 1, 1, 1);
            var c = new Range(1, 1, 1, 2);
            a.Should().Be(b);
            a.Should().NotBe(c);
        }

        [Theory]
        [InlineData(1, 2, 1, 1)]
        [InlineData(2, 1, 1, 2)]
        [InlineData(1, 1, 1, 2)]
        [InlineData(1, 1, 2, 1)]
        public void Range_Is_Not_Empty(int startLine, int startCharacter, int endLine, int endCharacter)
        {
            var s = new Range(startLine, startCharacter, endLine, endCharacter);
            s.IsEmpty().Should().BeFalse();
        }

        [Theory]
        [InlineData(1, 1, 1, 3, 1, 2, 1, 4, "lt", "a.start < b.start, a.end < b.end")]
        [InlineData(1, 1, 1, 3, 1, 1, 1, 4, "lt", "a.start = b.start, a.end < b.end")]
        [InlineData(1, 2, 1, 3, 1, 1, 1, 4, "lt", "a.start > b.start, a.end < b.end")]
        [InlineData(1, 1, 1, 4, 1, 2, 1, 4, "lt", "a.start < b.start, a.end = b.end")]
        [InlineData(1, 1, 1, 4, 1, 1, 1, 4, "eq", "a.start = b.start, a.end = b.end")]
        [InlineData(1, 2, 1, 4, 1, 1, 1, 4, "gt", "a.start > b.start, a.end = b.end")]
        [InlineData(1, 1, 1, 5, 1, 2, 1, 4, "gt", "a.start < b.start, a.end > b.end")]
        [InlineData(1, 1, 2, 4, 1, 1, 1, 4, "gt", "a.start = b.start, a.end > b.end")]
        [InlineData(1, 2, 5, 1, 1, 1, 1, 4, "gt", "a.start > b.start, a.end > b.end")]
        public void Compare_Ranges_Using_Ends(int startLineA, int startCharacterA, int endLineA, int endCharacterA, int startLineB, int startCharacterB, int endLineB, int endCharacterB, string @operator, string because)
        {
            var a = new Range(startLineA, startCharacterA, endLineA, endCharacterA);
            var b = new Range(startLineB, startCharacterB, endLineB, endCharacterB);

            ( @operator switch {
                "lt" => Range.CompareRangesUsingEnds(a, b) < 0,
                "eq" => Range.CompareRangesUsingEnds(a, b) == 0,
                "gt" => Range.CompareRangesUsingEnds(a, b) > 0,
                _ => false
            } ).Should().BeTrue(because);

        }

        [Theory]
        [InlineData(1, 1, 1, 3, 1, 2, 1, 4, "lt", "a.start < b.start, a.end < b.end")]
        [InlineData(1, 1, 1, 5, 1, 2, 1, 4, "lt", "a.start < b.start, a.end > b.end")]
        [InlineData(1, 1, 1, 4, 1, 2, 1, 4, "lt", "a.start < b.start, a.end = b.end")]
        [InlineData(1, 1, 1, 3, 1, 1, 1, 4, "lt", "a.start = b.start, a.end < b.end")]
        [InlineData(1, 1, 1, 4, 1, 1, 1, 4, "eq", "a.start = b.start, a.end = b.end")]
        [InlineData(1, 1, 2, 4, 1, 1, 1, 4, "gt", "a.start = b.start, a.end > b.end")]
        [InlineData(1, 2, 1, 4, 1, 1, 1, 4, "gt", "a.start > b.start, a.end = b.end")]
        [InlineData(1, 2, 1, 3, 1, 1, 1, 4, "gt", "a.start > b.start, a.end < b.end")]
        [InlineData(1, 2, 5, 1, 1, 1, 1, 4, "gt", "a.start > b.start, a.end > b.end")]
        public void Compare_Ranges_Using_Starts(int startLineA, int startCharacterA, int endLineA, int endCharacterA, int startLineB, int startCharacterB, int endLineB, int endCharacterB, string @operator, string because)
        {
            var a = new Range(startLineA, startCharacterA, endLineA, endCharacterA);
            var b = new Range(startLineB, startCharacterB, endLineB, endCharacterB);

            ( @operator switch {
                "lt" => Range.CompareRangesUsingStarts(a, b) < 0,
                "eq" => Range.CompareRangesUsingStarts(a, b) == 0,
                "gt" => Range.CompareRangesUsingStarts(a, b) > 0,
                _ => false
            } ).Should().BeTrue(because);

        }

        [Theory]
        [InlineData(2, 2, 5, 10, 1, 3, false)]
        [InlineData(2, 2, 5, 10, 2, 1, false)]
        [InlineData(2, 2, 5, 10, 2, 2, true)]
        [InlineData(2, 2, 5, 10, 2, 3, true)]
        [InlineData(2, 2, 5, 10, 3, 1, true)]
        [InlineData(2, 2, 5, 10, 5, 9, true)]
        [InlineData(2, 2, 5, 10, 5, 10, true)]
        [InlineData(2, 2, 5, 10, 5, 11, false)]
        [InlineData(2, 2, 5, 10, 6, 1, false)]
        public void Range_Contains_Position(int startLine, int startCharacter, int endLine, int endCharacter, int positionLine, int positionCharacter, bool result)
        {
            var range = new Range(startLine, startCharacter, endLine, endCharacter);
            range.Contains(new Position(positionLine, positionCharacter)).Should().Be(result);
        }

        [Theory]
        [InlineData(2, 2, 5, 10, 1, 3, 2, 2, false)]
        [InlineData(2, 2, 5, 10, 2, 1, 2, 2, false)]
        [InlineData(2, 2, 5, 10, 2, 2, 5, 11, false)]
        [InlineData(2, 2, 5, 10, 2, 2, 6, 1, false)]
        [InlineData(2, 2, 5, 10, 5, 9, 6, 1, false)]
        [InlineData(2, 2, 5, 10, 5, 10, 6, 1, false)]
        [InlineData(2, 2, 5, 10, 2, 2, 5, 10, true)]
        [InlineData(2, 2, 5, 10, 2, 3, 5, 9, true)]
        [InlineData(2, 2, 5, 10, 3, 100, 4, 100, true)]
        public void Range_Contains_Range(int startLine, int startCharacter, int endLine, int endCharacter, int startLineB, int startCharacterB, int endLineB, int endCharacterB, bool result)
        {
            var rangeA = new Range(startLine, startCharacter, endLine, endCharacter);
            var rangeB = new Range(startLineB, startCharacterB, endLineB, endCharacterB);
            rangeA.Contains(rangeB).Should().Be(result);
        }

        [Theory]
        [InlineData(2, 2, 5, 10, 1, 3, 2, 2, false)]
        [InlineData(2, 2, 5, 10, 2, 1, 2, 2, false)]
        [InlineData(2, 2, 5, 10, 2, 2, 5, 11, false)]
        [InlineData(2, 2, 5, 10, 2, 2, 6, 1, false)]
        [InlineData(2, 2, 5, 10, 5, 9, 6, 1, false)]
        [InlineData(2, 2, 5, 10, 5, 10, 6, 1, false)]
        [InlineData(2, 2, 5, 9, 2, 2, 5, 10, false)]
        [InlineData(2, 2, 5, 10, 2, 2, 5, 10, false)]
        [InlineData(2, 1, 5, 10, 2, 2, 5, 10, false)]
        [InlineData(2, 2, 5, 10, 2, 3, 5, 9, true)]
        [InlineData(2, 2, 5, 10, 3, 100, 4, 100, true)]
        public void Range_Strictly_Contains_Range(int startLine, int startCharacter, int endLine, int endCharacter, int startLineB, int startCharacterB, int endLineB, int endCharacterB, bool result)
        {
            var rangeA = new Range(startLine, startCharacter, endLine, endCharacter);
            var rangeB = new Range(startLineB, startCharacterB, endLineB, endCharacterB);
            rangeA.StrictContains(rangeB).Should().Be(result);
        }

        [Theory]
        [InlineData(2, 2, 3, 2, 4, 2, 5, 2, false)]
        [InlineData(4, 2, 5, 2, 2, 2, 3, 2, false)]
        [InlineData(4, 2, 5, 2, 5, 2, 6, 2, false)]
        [InlineData(5, 2, 6, 2, 4, 2, 5, 2, false)]
        [InlineData(2, 2, 2, 7, 2, 4, 2, 6, true)]
        [InlineData(2, 2, 2, 7, 2, 4, 2, 9, true)]
        [InlineData(2, 4, 2, 9, 2, 2, 2, 7, true)]
        public void Range_Are_Intersecting(int startLine, int startCharacter, int endLine, int endCharacter, int startLineB, int startCharacterB, int endLineB, int endCharacterB, bool result)
        {
            var rangeA = new Range(startLine, startCharacter, endLine, endCharacter);
            var rangeB = new Range(startLineB, startCharacterB, endLineB, endCharacterB);
            Range.AreIntersecting(rangeA, rangeB).Should().Be(result);
        }

        [Fact]
        public void Ranges_Can_Be_Added()
        {
            var a = new Range(1, 1, 2, 2);
            var b = new Range(3, 3, 4, 0);

            ( a + b ).Should().Be(new Range(1, 1, 4, 0));
        }

        [Fact]
        public void Ranges_Can_Span_Lines()
        {
            var a = new Range(1, 1, 1, 2);
            var b = new Range(3, 3, 4, 0);

            a.SpansMultipleLines().Should().BeFalse();
            b.SpansMultipleLines().Should().BeTrue();
        }

        [Fact]
        public void Ranges_Can_Collapse()
        {
            var a = new Range(1, 1, 2, 2);

            a.CollapseToStart().Should().Be(new Range(a.Start, a.Start));
            a.CollapseToEnd().Should().Be(new Range(a.End, a.End));
        }

        [Fact]
        public void Ranges_Can_Be_Intersected()
        {
            var a = new Range(1, 1, 1, 3);
            var b = new Range(2, 1, 2, 2);
            var c = new Range(1, 2, 4, 0);

            a.Intersection(b).Should().BeNull();
            b.Intersection(a).Should().BeNull();
            a.Intersection(a).Should().Be(a);

            c.Intersection(b).Should().Be(b);
            b.Intersection(c).Should().Be(b);
            c.Intersection(a).Should().Be(new Range(1, 2, 1, 3));
            a.Intersection(c).Should().Be(new Range(1, 2, 1, 3));
        }

        [Fact]
        public void Ranges_Compare_Compare_With_Or_Without_Touching()
        {
            var a = new Range(1, 1, 1, 3);
            var b = new Range(2, 1, 2, 3);
            var c = new Range(1, 3, 2, 1);

            a.IsBefore(b).Should().BeTrue();
            b.IsAfter(a).Should().BeTrue();
            a.IsBefore(c).Should().BeFalse();
            b.IsAfter(c).Should().BeFalse();

            a.IsBeforeOrTouching(c).Should().BeTrue();
            b.IsAfterOrTouching(c).Should().BeTrue();
        }

        [Fact]
        public void Ranges_Check_For_Intersections()
        {
            var a = new Range(1, 1, 1, 3);
            var b = new Range(2, 1, 2, 3);
            var c = new Range(1, 3, 2, 1);

            a.Intersects(b).Should().BeFalse();
            a.IntersectsOrTouches(b).Should().BeFalse();

            a.Intersects(c).Should().BeFalse();
            a.IntersectsOrTouches(c).Should().BeTrue();

            b.Intersects(c).Should().BeFalse();
            b.IntersectsOrTouches(c).Should().BeTrue();

        }

        [Fact]
        public void Ranges_Can_Be_Sorted_By_Start()
        {
            var a = new Range(0, 0, 0, 1);
            var b = new Range(0, 0, 0, 5);
            var c = new Range(1, 0, 1, 1);
            var d = new Range(2, 0, 2, 1);
            var e = new Range(3, 0, 3, 1);

            var list = new[] { b, a, d, e, c };

            list.OrderBy(z => z, Range.CompareUsingStarts)
                .Should()
                .ContainInOrder(a, b, c, d);
        }

        [Fact]
        public void Ranges_Can_Be_Sorted_By_End()
        {
            var a = new Range(0, 0, 0, 1);
            var b = new Range(0, 0, 0, 5);
            var c = new Range(1, 0, 1, 1);
            var d = new Range(2, 0, 2, 1);
            var e = new Range(3, 0, 3, 1);

            var list = new[] { b, a, d, e, c };

            list.OrderByDescending(z => z, Range.CompareUsingEnds)
                .Should()
                .ContainInOrder(e, d, c, b, a);
        }
    }
}
