using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class ShowMessageParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new ShowMessageParams() {
                Message = "message",
                Type = MessageType.Log
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<ShowMessageParams>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
