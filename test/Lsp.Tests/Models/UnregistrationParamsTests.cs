using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class UnregistrationParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new UnregistrationParams()
            {
                Unregisterations = new UnregistrationContainer(new Unregistration()
                {
                    Id = "abc",
                    Method = "ads"
                })
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<UnregistrationParams>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
