using System.Text.Json;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class CommandTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new Command() {
                Arguments = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(new object[] { 1, "2", true })),
                Name = "abc",
                Title = "Cool story bro"
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonSerializer.Deserialize<Command>(expected, Serializer.Instance.Options);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
