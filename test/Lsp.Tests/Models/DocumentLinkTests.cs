using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DocumentLinkTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DocumentLink() {
                Range = new Range(new Position(1, 2), new Position(3, 4)),
                Target = new Uri("file:///abc/123.cs")
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DocumentLink>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
