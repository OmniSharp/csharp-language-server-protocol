using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using TestingUtils;

namespace Lsp.Tests.Models
{
    public class RegistrationParamsTests
    {
        [Theory]
        [JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new RegistrationParams {
                Registrations = new[] {
                    new Registration {
                        Id = "abc",
                        Method = "method",
                        RegisterOptions = new Dictionary<string, object>()
                    }
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<RegistrationParams>(expected);
            deresult.Should().BeEquivalentTo(model, x => x.UsingStructuralRecordEquality());
        }
    }
}
