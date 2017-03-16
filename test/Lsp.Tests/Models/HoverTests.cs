using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class HoverTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new Hover() {
                Contents = new MarkedStringContainer("abc"),
                Range = new Range(new Position(1, 2), new Position(3, 4))
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<Hover>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
