using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class UnregistrationParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new UnregistrationParams() {
                Unregisterations = new UnregistrationContainer(new Unregistration() {
                    Id = "abc",
                    Method = "ads"
                })
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<UnregistrationParams>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
