using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class ExecuteCommandRegistrationOptionsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new ExecuteCommandRegistrationOptions() {
                Commands = new [] { "1", "2" }                
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<ExecuteCommandRegistrationOptions>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
