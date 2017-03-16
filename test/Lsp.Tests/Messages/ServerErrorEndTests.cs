using System;
using FluentAssertions;
using Lsp.Messages;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Messages
{
    public class ServerErrorEndTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new ServerErrorEnd();
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<ServerErrorEnd>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
