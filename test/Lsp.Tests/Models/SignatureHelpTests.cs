using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class SignatureHelpTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new SignatureHelp() {
                ActiveParameter = 1,
                ActiveSignature = 2,
                Signatures = new[] { new SignatureInformation() {
                    Documentation = "ab",
                    Label = "ab",
                    Parameters = new[] { new ParameterInformation() {
                        Documentation = "param",
                        Label = "param"
                    } }
                }
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<SignatureHelp>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
