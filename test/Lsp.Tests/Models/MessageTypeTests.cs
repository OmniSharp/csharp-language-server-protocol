using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class MessageTypeTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new MessageType();
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<MessageType>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
