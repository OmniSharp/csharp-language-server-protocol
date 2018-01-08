using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class ParameterInformationTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new ParameterInformation() {
                Documentation = "docs",
                Label = "label"
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<ParameterInformation>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
