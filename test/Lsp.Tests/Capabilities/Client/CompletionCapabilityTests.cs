using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Client;
using Xunit;

namespace Lsp.Tests.Capabilities.Client
{
    public class CompletionCapabilityTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CompletionCapability() { DynamicRegistration = false, CompletionItem = new CompletionItemCapability() { SnippetSupport = false } };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<CompletionCapability>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
