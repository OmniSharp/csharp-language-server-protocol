using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DidChangeConfigurationParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DidChangeConfigurationParams() {
                Settings = JObject.FromObject(new Dictionary<string, object>() {
                    { "abc", 1 },
                    { "def", "a" },
                    { "ghi", true },
                })
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<DidChangeConfigurationParams>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
