using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DocumentFormattingParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DocumentFormattingParams() {
                Options = new FormattingOptions() {
                    { "abc", 1 }
                },
                TextDocument = new TextDocumentIdentifier(new Uri("file:///abc123.cs"))
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DocumentFormattingParams>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
