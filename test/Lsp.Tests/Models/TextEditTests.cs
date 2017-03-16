using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class TextEditTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new TextEdit() {
                NewText = "new text",
                Range = new Range(new Position(1, 1), new Position(2, 2))
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<TextEdit>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
