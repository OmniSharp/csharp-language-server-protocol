using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class CompletionItemTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CompletionItem()
            {
                CommitCharacters = new[] { ";", "/", "." },
                AdditionalTextEdits = new[] {
                    new TextEdit() {
                        NewText = "new text"
                    }
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<CompletionItem>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
