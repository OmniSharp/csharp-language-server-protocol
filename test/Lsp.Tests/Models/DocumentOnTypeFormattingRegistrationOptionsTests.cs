using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DocumentOnTypeFormattingRegistrationOptionsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DocumentOnTypeFormattingRegistrationOptions() {
                DocumentSelector = new DocumentSelector(new DocumentFilter() {
                    Language = "csharp"
                }),
                FirstTriggerCharacter = "1",
                MoreTriggerCharacter = new [] { "1", "2" }
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DocumentOnTypeFormattingRegistrationOptions>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
