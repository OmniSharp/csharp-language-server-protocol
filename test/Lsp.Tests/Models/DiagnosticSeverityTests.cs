using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DiagnosticSeverityTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DiagnosticSeverity();
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<DiagnosticSeverity>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
