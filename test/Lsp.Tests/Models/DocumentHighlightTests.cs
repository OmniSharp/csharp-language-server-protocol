using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DocumentHighlightTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DocumentHighlight() {
                Kind = DocumentHighlightKind.Text,
                Range = new Range(new Position(1, 1), new Position(2, 2))
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DocumentHighlight>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
