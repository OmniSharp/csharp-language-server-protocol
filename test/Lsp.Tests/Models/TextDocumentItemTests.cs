using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class TextDocumentItemTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new TextDocumentItem() {
                Uri = new Uri("file:///abc/def.cs"),
                LanguageId = "csharp",
                Text = "content",
                Version = 1
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<TextDocumentItem>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
