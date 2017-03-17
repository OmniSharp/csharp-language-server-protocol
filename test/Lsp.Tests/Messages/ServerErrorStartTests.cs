using System;
using FluentAssertions;
using JsonRpc.Server;
using Lsp.Messages;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Messages
{
    public class ServerErrorStartTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new ServerErrorStart("abcd");
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<Error>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
