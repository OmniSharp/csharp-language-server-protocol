using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServerProtocol.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class TextDocumentContentChangeEventTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new TextDocumentContentChangeEvent() {
                Range = new Range(new Position(1, 2), new Position(3, 4)),
                RangeLength = 12,
                Text = "abc"
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<TextDocumentContentChangeEvent>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
