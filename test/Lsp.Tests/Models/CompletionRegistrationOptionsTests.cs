using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class CompletionRegistrationOptionsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CompletionRegistrationOptions() {
                DocumentSelector = new DocumentSelector(new DocumentFilter() {
                    Language = "csharp"
                }),
                ResolveProvider = true,
                TriggerCharacters = new[] { "." }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<CompletionRegistrationOptions>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
