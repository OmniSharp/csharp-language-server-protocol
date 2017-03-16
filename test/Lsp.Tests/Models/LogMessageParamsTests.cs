using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class LogMessageParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new LogMessageParams() {
                Message = "message",
                Type = MessageType.Error
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<LogMessageParams>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
