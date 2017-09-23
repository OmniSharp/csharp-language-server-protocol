using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class LocationTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new Location() {
                Range = new Range(new Position(1,2), new Position(3,4)),
                Uri = new Uri("file:///abc/123.cs")
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<Location>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
