using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class TextDocumentSaveRegistrationOptionsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new TextDocumentSaveRegistrationOptions() {
                DocumentSelector = new DocumentSelector(new DocumentFilter() {
                    Language = "csharp"
                }),
                IncludeText = true
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<TextDocumentSaveRegistrationOptions>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
