using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using TestingUtils;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DidChangeConfigurationParamsTests
    {
        [Theory]
        [JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DidChangeConfigurationParams {
                Settings = JsonSerializer.SerializeToElement(
                    new Dictionary<string, object> {
                        { "abc", 1 },
                        { "def", "a" },
                        { "ghi", true },
                    }
                )
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<DidChangeConfigurationParams>(expected);
            JsonElement.DeepEquals(deresult.Settings!.Value, model.Settings!.Value).Should().BeTrue();
        }
    }
}
