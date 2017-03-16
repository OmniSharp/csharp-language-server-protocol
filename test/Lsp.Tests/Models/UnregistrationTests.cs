using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class UnregistrationTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new Unregistration() {
                Id = "abc",
                Method = "ads"
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<Unregistration>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
