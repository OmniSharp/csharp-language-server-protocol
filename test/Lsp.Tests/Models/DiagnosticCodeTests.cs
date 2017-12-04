using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DiagnosticCodeTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DiagnosticCode();
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DiagnosticCode>(expected, Serializer.CreateSerializerSettings(ClientVersion.Lsp3));
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
