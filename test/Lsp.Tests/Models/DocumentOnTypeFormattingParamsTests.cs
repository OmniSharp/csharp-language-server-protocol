using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DocumentOnTypeFormattingParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DocumentOnTypeFormattingParams() {
                Character = "c",
                Options = new FormattingOptions() {
                    { "abc", 1 }
                },
                Position = new Position(1,2),
                TextDocument = new TextDocumentIdentifier(new Uri("file:///abc/123.cs"))
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DocumentOnTypeFormattingParams>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
