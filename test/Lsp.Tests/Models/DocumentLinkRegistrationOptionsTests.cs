using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DocumentLinkRegistrationOptionsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DocumentLinkRegistrationOptions() {
                DocumentSelector = new DocumentSelector(new DocumentFilter() {
                    Language = "csharp"
                }),
                ResolveProvider = true
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DocumentLinkRegistrationOptions>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
