using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class SignatureInformationTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new SignatureInformation() {
                Documentation = "ab",
                Label = "ab",
                Parameters = new[] { new ParameterInformation() {
                        Documentation = "param",
                        Label = "param"
                    } }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<SignatureInformation>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
