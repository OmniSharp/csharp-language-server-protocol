using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class TextDocumentIdentifierTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new TextDocumentIdentifier(new Uri("file:///abc/123/d.cs"));
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<TextDocumentIdentifier>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
