using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class ApplyWorkspaceEditResponseTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new ApplyWorkspaceEditResponse() {
                Applied = true,
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<ApplyWorkspaceEditResponse>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
