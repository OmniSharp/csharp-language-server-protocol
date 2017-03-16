using System;
using FluentAssertions;
using Lsp.Capabilities.Server;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Capabilities.Server
{
    public class ExecuteCommandOptionsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new ExecuteCommandOptions() {
                Commands = new string[] { "command1", "command2" }
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<ExecuteCommandOptions>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
