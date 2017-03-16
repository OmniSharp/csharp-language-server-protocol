using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class CommandTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new Command() {
                Arguments = new object[] { 1, "2", true },
                Name = "abc",
                Title = "Cool story bro"
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<Command>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
