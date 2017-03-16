using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
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

            var deresult = JsonConvert.DeserializeObject<ParameterInformation>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
