using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit.Abstractions;

namespace Lsp.Tests.Testing
{
    public class TestContentTests : AutoTestBase
    {
        public TestContentTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void Should_Parse_Locations_1()
        {
            var content = TestContent.Parse(@"012$$3456789");
            content.Code.Should().Be("0123456789");
            content.Index.Should().Be(3);
            content.Lines.Should().HaveCount(1);

            content.GetPositionAtIndex().Should().Be(new Position(0, 3));
            content.GetIndexAtPosition(new Position(0, 5)).Should().Be(5);
        }

        [Theory]
        [InlineData(@"01|]2345", "Saw |] without matching [|")]
        [InlineData(@"01|}name:2345", "Saw |} without matching {|")]
        public void Throw_Error_On_Range(string text, string message)
        {
            Action a = () => TestContent.Parse(text);
            a.Should().Throw<ArgumentException>().WithMessage(message);
        }

        [Fact]
        public void Should_Parse_Locations_2()
        {
            var content = TestContent.Parse(
                @"0
1
2
$$3
4
5
6
7
8
9"
            );
            content.Code.Should().Be(
                @"0
1
2
3
4
5
6
7
8
9".NormalizeLineEndings()
            );
            content.Index.Should().Be(6);
            content.Lines.Should().HaveCount(10);

            content.GetPositionAtIndex().Should().Be(new Position(3, 0));
            content.GetIndexAtPosition(new Position(4, 0)).Should().Be(8);
        }

        [Theory]
        [InlineData('{', '}')]
        [InlineData('[', ']')]
        [InlineData('(', ')')]
        public void Position_Marker_Should_be_configurable(char first, char end)
        {
            var content = $"hello {first}{end}this is a test";
            var testContent = TestContent.Parse(content, new TestContentOptions() {
                PositionMarker = (first, end)
            });
            testContent.HasIndex.Should().BeTrue();
            testContent.Index.Should().Be(6);
        }

        [Theory]
        [InlineData('{', '}', '|')]
        [InlineData('[', ']', '|')]
        [InlineData('(', ')', '|')]
        public void Ranges_Should_be_configurable(char start, char end, char term)
        {
            var content = $"hello {start}{term}this is a {term}{end}test";
            var testContent = TestContent.Parse(content, new TestContentOptions() {
                RangeMarker = ((start, term), (term, end))
            });
            testContent.GetRanges().Should().HaveCount(1);
            testContent.ExtractRange(testContent.GetRanges()[0]).Should().Be("this is a ");
        }

        [Theory]
        [InlineData('{', '}', '|', '?')]
        [InlineData('-', '-', '|', ':')]
        [InlineData('(', ')', '|', '-')]
        public void Named_Ranges_Should_be_configurable(char start, char end, char term, char nameEnd)
        {
            var content = $"hello {start}{term}test{nameEnd}this is a {term}{end}test";
            var testContent = TestContent.Parse(content, new TestContentOptions() {
                NamedRangeMarker = ((start, term), nameEnd, (term, end))
            });
            testContent.GetRanges("test").Should().HaveCount(1);
            testContent.ExtractRange(testContent.GetRanges("test")[0]).Should().Be("this is a ");
        }

        [Fact]
        public void Should_Parse_Spans_1()
        {
            var content = TestContent.Parse(
                @"
[|if (true) {
  [|var a = 1;|]
  [|var b = 2;|]
  [|var c = 3;|]
[|var other = new {
  [|value = true|]
};|]
|]"
            );
            content.Code.Should().Be(
                @"
if (true) {
  var a = 1;
  var b = 2;
  var c = 3;
var other = new {
  value = true
};
".NormalizeLineEndings()
            );
            content.Index.Should().Be(-1);
            content.Lines.Should().HaveCount(9);


            var ranges = content.GetRanges();
            ranges.Should().HaveCount(6);
            content.ExtractRange(ranges[0]).Should().Be(
                @"if (true) {
  var a = 1;
  var b = 2;
  var c = 3;
var other = new {
  value = true
};
".NormalizeLineEndings()
            );
            content.ExtractRange(ranges[1]).Should().Be(@"var a = 1;");
            content.ExtractRange(ranges[2]).Should().Be(@"var b = 2;");
            content.ExtractRange(ranges[3]).Should().Be(@"var c = 3;");
            content.ExtractRange(ranges[4]).Should().Be(
                @"var other = new {
  value = true
};".NormalizeLineEndings()
            );
            content.ExtractRange(ranges[5]).Should().Be(@"value = true");
        }

        [Fact]
        public void Should_Parse_Named_Spans_1()
        {
            var content = TestContent.Parse(
                @"
{|first:if (true) {
  {|a:var a = 1;|}
  {|b:var b = 2;|}
  {|c:var c = 3;|}
{|other:var other = new {
  {|other:value = true|}
};|}
|}"
            );
            content.Code.Should().Be(
                @"
if (true) {
  var a = 1;
  var b = 2;
  var c = 3;
var other = new {
  value = true
};
".NormalizeLineEndings()
            );
            content.Index.Should().Be(-1);
            content.Lines.Should().HaveCount(9);


            var ranges = content.GetRanges("first");
            ranges.Should().HaveCount(1);
            content.ExtractRange(ranges[0]).Should().Be(
                @"if (true) {
  var a = 1;
  var b = 2;
  var c = 3;
var other = new {
  value = true
};
".NormalizeLineEndings()
            );
            ranges = content.GetRanges("a");
            ranges.Should().HaveCount(1);
            content.ExtractRange(ranges[0]).Should().Be(@"var a = 1;");

            ranges = content.GetRanges("b");
            ranges.Should().HaveCount(1);
            content.ExtractRange(ranges[0]).Should().Be(@"var b = 2;");

            ranges = content.GetRanges("c");
            ranges.Should().HaveCount(1);
            content.ExtractRange(ranges[0]).Should().Be(@"var c = 3;");

            ranges = content.GetRanges("other");
            ranges.Should().HaveCount(2);
            content.ExtractRange(ranges[0]).Should().Be(
                @"var other = new {
  value = true
};".NormalizeLineEndings()
            );
            content.ExtractRange(ranges[1]).Should().Be(@"value = true");
        }
    }
}
