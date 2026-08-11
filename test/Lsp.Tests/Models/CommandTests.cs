using System.Linq;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using TestingUtils;
using Xunit;

namespace Lsp.Tests.Models
{
    public class CommandTests
    {
        [Theory]
        [JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new Command {
                Arguments = Command.CreateArguments(1, "2", true),
                Name = "abc",
                Title = "Cool story bro"
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<Command>(expected);
            deresult.Arguments.Should().NotBeNull();
            deresult.Arguments!.Select(z => z.GetRawText()).Should().Equal(model.Arguments!.Select(z => z.GetRawText()));
        }
    }
}
