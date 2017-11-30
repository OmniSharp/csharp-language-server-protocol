using System;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class CommandTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new Command() {
                Arguments = new JArray { 1, "2", true },
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
