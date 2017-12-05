using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using Xunit;

namespace Lsp.Tests.Capabilities.Client
{
    public class CompletionItemCapabilityTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CompletionItemCapability() {
                SnippetSupport = true
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<CompletionItemCapability>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
